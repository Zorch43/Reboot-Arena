using Assets.Scripts.Data_Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rules for the King-of-the-Hill game mode:
/// While a player is "In Control" of a plurality of the points, tick down their timer
/// When a timer tick down all the way, that team wins once they are in "Full Control"
/// </summary>
public class GameRulesKoth : GameRulesBase
{
    #region constants

    #endregion
    #region public fields

    public GameTimerController GameStatusUI;//the prefab for timers to add to the ui;

    //rule modifiers
    public int TimerLength;//length of the timer in seconds
    public float MaxTeamToPointRatio;//maximum number of teams per active point.  Excess points are de-activated from the bottom of the list up.  Must always be at least one active point.
    public float TimerPlayerMod;//multiply timer length by this amount for each player beyond the second.  Used to adjust the timer for team count
    public bool Endless;//if set to true, game cannot be won by anyone
    //TODO: Macro AI controller for these rules
    #endregion
    #region private fields
    private GameTimerController timers;
    private List<CapturePointController> activeCapturePoints;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
    #region public methods
    public override void Setup()
    {
        //add timers to UI
        timers = Instantiate(GameStatusUI, Game.GameStateUI.transform);
        //timers.gameObject.transform.position = GameStateUI.transform.position;

        //setup timers
        //timers.Setup(TimerLength * (1 + (Game.Teams.Count - 2) * (1 - TimerPlayerMod)), Game.Teams);
        timers.Setup(TimerLength, Game.Teams);

        //hide timers if in endless mode
        timers.gameObject.SetActive(!Endless);

        //TODO: lock unused control points
        activeCapturePoints = new List<CapturePointController>(ControlPoints);
    }
    public override bool IsTeamVictorious(int team, float deltaTime)
    {
        int controlledPoints = 0;
        int fullyControlledPoints = 0;
        //for each control point under the team's control
        foreach(var cp in ControlPoints)
        {
            if(cp.CurrOwner == team && cp.CaptureProgress > 0)
            {
                //count controlled points
                controlledPoints++;
                if(cp.CaptureProgress >= 1)
                {
                    //count fully-controlled points
                    fullyControlledPoints++;
                }
            }
        }

        //TODO: display controlled points
        //TODO: display fully-controlled points

        //if required number of points controlled, tick down timer
        bool inFullControl;
        int rivalControl = 0;
        if(IsInControl(team, out inFullControl, out rivalControl))
        {
            if (!Endless)
            {
                var remainingTime = timers.UpdateTimer(-deltaTime, team);
                //if timer is done
                if (remainingTime <= 0)
                {
                    //and required number of points are fully-controlled
                    if (inFullControl)
                    {
                        //team is victorious; return true
                        return true;
                    }
                    else
                    {
                        //TODO: display sudden death notification
                    }
                }
            }
        }

        //return false
        return false;
    }

    public override List<AIObjectiveModel> GetAIObjectives(int team, List<DroneController> allDrones)
    {
        var objectives = new List<AIObjectiveModel>();
        //evaluate each control point
        foreach (var cp in activeCapturePoints)
        {
            var objectivePoint = cp.transform.position;
            var objective = new AIObjectiveModel()
            {
                Objective = cp
            };
            objectives.Add(objective);
            int enemyUnitCount = 0;
            int alliedUnitCount = 0;
            foreach (var u in allDrones)
            {
                if(Vector3.Distance(objectivePoint, u.transform.position) < OCCUPATION_DISTANCE)
                {
                    if (u.Data.Team != team)
                    {
                        objective.EnemyAttackerWeight += u.Data.UnitClass.AttackerWeight;
                        objective.EnemyDefenderWeight += u.Data.UnitClass.DefenderWeight;
                        objective.EnemySupportWeight += u.Data.UnitClass.SupportWeight;
                        enemyUnitCount++;
                    }
                    else
                    {
                        objective.AttackerWeight += u.Data.UnitClass.AttackerWeight;
                        objective.DefenderWeight += u.Data.UnitClass.DefenderWeight;
                        objective.SupportWeight += u.Data.UnitClass.SupportWeight;
                        alliedUnitCount++;
                    }
                }
                
            }
            if(enemyUnitCount > 0)
            {
                float forceMultiplier = 1 + 0.1f * objective.EnemySupportWeight / enemyUnitCount;//support force-multiplier
                objective.EnemyAttackerWeight *= forceMultiplier;
                objective.EnemyDefenderWeight *= forceMultiplier;
            }
            if(alliedUnitCount > 0)
            {
                float forceMultiplier = 1 + 0.1f * objective.SupportWeight / alliedUnitCount;//support force-multiplier
                objective.AttackerWeight *= forceMultiplier;
                objective.DefenderWeight *= forceMultiplier;
            }
            
            //AI will:
            //Rush sparsely defended unowned points
            //Attack occupied points
            //Defend taken points
            //Retreat from points with strong enemy presence
            var spawnPoint = Game.GetAISpawnPoint(team);
            bool fullControl;
            int bestRival;
            bool inControl = IsInControl(cp.CurrOwner, out fullControl, out bestRival);
            var timeToWin = timers.GetRawTime(team);
            //prioritize:
            //points near the spawn point
            objective.Priority += 1 - Vector3.Distance(spawnPoint, objectivePoint)/100;
            
            if (cp.CurrOwner != team)
            {
                //points that already have allied units near them
                if (alliedUnitCount > 0)
                {
                    objective.Priority += 1;
                }
                //points with a strong presence
                if (objective.AttackerWeight + 0.5f * objective.DefenderWeight >= 7)
                {
                    objective.Priority += 1;
                }
                //rushing unoccupied points
                if (enemyUnitCount <= 1)
                {
                    objective.Priority += 1;
                }
                //attacking sparsely defended points
                if (enemyUnitCount <= 4)
                {
                    objective.Priority += 1;
                }
                if (objective.EnemyDefenderWeight + 0.5f * objective.EnemyAttackerWeight < 7)
                {
                    objective.Priority += 1;
                }
                //attacking/rushing neutral points
                if (cp.CurrOwner == -1)
                {
                    objective.Priority += 1;
                }
                //attacking points controlled by controlling team
                //attacking points owned by enemies close to winning
                else if (!inControl)
                {
                    objective.Priority += 1;
                    var timeLeft = timers.GetRawTime(cp.CurrOwner);
                    if (timeLeft < 30)
                    {
                        objective.Priority += 1;
                    }
                    if (timeLeft < timeToWin)
                    {
                        objective.Priority += 1;
                    }
                }
            }
            else
            {
                if (objective.EnemyAttackerWeight + 0.5f * objective.EnemyDefenderWeight < 7)
                {
                    objective.Priority += 1;
                }
                //defending owned points whose loss would result in loss of control
                //defending owned points when close to winning
                if (inControl)
                {
                    objective.Priority += 1;
                    if (timeToWin < 30)
                    {
                        objective.Priority += 1;
                    }
                }
            }
        }                                                               

        return objectives;

    }
    #endregion
    #region private methods
    private bool IsInControl(int team, out bool fullControl, out int bestRivalCount)
    {
        //check to see if the team controls a plurality of the points (must hold at least 1 point)
        int rivalControlCount = 0;
        int controlCount = 0;
        int fullControlCount = 0;
        foreach(var t in Game.Teams)
        {
            int teamCount = 0;
            int teamFullControlCount = 0;
            foreach(var cp in activeCapturePoints)
            {
                if(cp.CurrOwner == t.Team)
                {
                    teamCount++;
                    if(cp.CaptureProgress >= 1)
                    {
                        teamFullControlCount++;
                    }
                }
            }
            if (t.Team == team)
            {
                controlCount = teamCount;
                fullControlCount = teamFullControlCount;
            }
            else if (teamCount > rivalControlCount)
            {
                rivalControlCount = teamCount;
            }
        }
        fullControl = fullControlCount > 0 && fullControlCount >= rivalControlCount;
        bestRivalCount = rivalControlCount;
        return controlCount > 0 && controlCount >= rivalControlCount;
    }

    
    #endregion

}
