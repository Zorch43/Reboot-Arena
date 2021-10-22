using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectiveController : MonoBehaviour
{
    #region constants
    private const float COUNTDOWN_TIME = 180;//point must be held for 3 minutes to win
    #endregion
    #region public fields
    public GameMenuController GameMenu;
    public CapturePointController Objective;//capture point that teams are fighting over
    public GameObject GameStateUI;//where to display timer count-downs;
    public KotHTimerController GameStatusUI;//the prefab to add to the ui;
    public GameObject VictoryStateUI;//ui that displays victory message
    public GameObject DefeatStateUI;//ui that displays defeat message
    public TeamController PlayerTeam;
    public TeamController AITeam;
    #endregion
    #region private fields
    private int team1;
    private int team2;
    private KotHTimerController timers;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //set teams
        team1 = PlayerTeam.Team;
        team2 = AITeam.Team;

        //add timers to UI
        timers = Instantiate(GameStatusUI, GameStateUI.transform);
        //timers.gameObject.transform.position = GameStateUI.transform.position;

        //setup timers
        timers.Setup(COUNTDOWN_TIME, team1, team2);
    }

    // Update is called once per frame
    void Update()
    {
        //check if a team is in control of the objective
        if(Objective.CaptureProgress > 0 && Objective.CurrOwner >= 0 && Objective.CurrOwner == Objective.NextOwner)
        {
            var holdingTeam = Objective.CurrOwner;
            //count down the holding team's timer
            var remainingTime = timers.UpdateTimer(-Time.deltaTime, holdingTeam);
            //check for holding team's victory - countdown completed, AND in complete control of the point
            if(!GameMenu.gameObject.activeSelf && remainingTime <= 0 && Objective.CaptureProgress >= 1)
            {
                if(holdingTeam == team1)
                {
                    //player has won, display victory message
                    VictoryStateUI.SetActive(true);
                    GameMenu.ShowMenu(true);
                }
                else
                {
                    //AI has won, display defeat message
                    DefeatStateUI.SetActive(true);
                    GameMenu.ShowMenu(true);
                }
            }
        }

    }
    #endregion
    #region public methods
    public Vector3 GetAIObjective()
    {
        return Objective.transform.position;
    }
    public Vector3 GetAISpawnPoint()
    {
        return AITeam.SpawnPoint.transform.position;
    }
    #endregion
    #region private methods

    #endregion
}
