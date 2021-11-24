using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameObjectiveController : MonoBehaviour
{
    #region constants
    private const float COUNTDOWN_TIME = 180;//point must be held for 3 minutes to win
    #endregion
    #region static fields
    public static BattleConfigModel BattleConfig;
    #endregion
    #region public fields
    public GameMenuController GameMenu;
    public CapturePointController Objective;//capture point that teams are fighting over
    public GameObject GameStateUI;//where to display timer count-downs;
    public KotHTimerController GameStatusUI;//the prefab to add to the ui;
    public GameObject VictoryStateUI;//ui that displays victory or defeat message

    public SpawnPointController[] SpawnPoints;//list of all starting spawn points
    public UnitSlotManager PlayerSlotManager;//need this referenced here to apply to player-controlled teams
    public ActionPanelController PlayerUnitActions;
    public TeamController PlayerTeamTemplate;
    public TeamController AITeamTemplate;

    public MapController Map;
    public CommandController CommandInterface;
    public CameraController Cameras;
    public MusicPlayerController MusicPlayer;
    
    #endregion
    #region private fields

    private KotHTimerController timers;
    private TeamController playerTeam;
    #endregion
    #region properties
    public List<TeamController> Teams { get; set; } = new List<TeamController>();
    public List<UnitController> AllUnits
    {
        get
        {
            var units = new List<UnitController>();
            foreach(var t in Teams)
            {
                foreach(var u in t.UnitSlots)
                {
                    if(u.CurrentUnit != null)
                    {
                        units.Add(u.CurrentUnit);
                    }
                }
            }
            return units;
        }
    }
    public List<UnitController> PlayerUnits
    {
        get
        {
            var units = new List<UnitController>();
            if(playerTeam != null)
            {
                foreach (var u in playerTeam.UnitSlots)
                {
                    if (u.CurrentUnit != null)
                    {
                        units.Add(u.CurrentUnit);
                    }
                }
            }
            return units;
        }
    }

    #endregion
    #region unity methods
    private void Awake()
    {
        if (BattleConfig == null)
        {
            BattleConfig = new BattleConfigModel()
            {
                Players = new List<PlayerConfigModel>()
                {
                    new PlayerConfigModel() { Controller = PlayerConfigModel.ControlType.Player, TeamId = 0},
                    new PlayerConfigModel() { Controller = PlayerConfigModel.ControlType.AI, TeamId = 1}
                }
            };
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        if (!BattleConfig.IsValidConfig)
        {
            Debug.LogError("Not a valid battle config!");
        }

        //create teams based on battle config
        bool spectator = BattleConfig.IsPlayerSpectator;
        for(int i = 0; i < SpawnPoints.Length; i++)
        {
            if(i < BattleConfig.Players.Count)
            {
                var teamConfig = BattleConfig.Players[i];
                TeamController team = null;
                if (teamConfig.Controller == PlayerConfigModel.ControlType.AI)
                {
                    team = Instantiate(AITeamTemplate, transform);
                    team.SetUnitClasses(teamConfig.UnitClasses);
                    var ai = team.GetComponent<AIController>();
                    ai.GameObjective = this;
                    ai.Config = AIConfigTemplates.GetAIConfigList()[teamConfig.AIIndex];//select ai for ai player
                    ai.Map = Map;
                    if (!spectator)
                    {
                        team.HideUnitUI = true;
                    }
                }
                else if (teamConfig.Controller == PlayerConfigModel.ControlType.Player)
                {
                    team = Instantiate(PlayerTeamTemplate, transform);
                    team.SetUnitClasses(teamConfig.UnitClasses);
                    playerTeam = team;
                    team.UnitSlotManager = PlayerSlotManager;
                    PlayerUnitActions.Setup(team);
                    Cameras.PanToMapLocation(SpawnPoints[i].transform.position);
                }
                team.Team = teamConfig.TeamId;
                team.DefaultSpawnPoint = SpawnPoints[i];
                Teams.Add(team);
            }
            else
            {
                SpawnPoints[i].ControllingTeam = -1;
            }
        }
        if (spectator)
        {
            PlayerUnitActions.Setup(null);
        }
        //add timers to UI
        timers = Instantiate(GameStatusUI, GameStateUI.transform);
        //timers.gameObject.transform.position = GameStateUI.transform.position;

        //setup timers
        timers.Setup(COUNTDOWN_TIME, Teams);
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
                var playerTeam = BattleConfig.PlayerTeam;
                var victoryMessage = VictoryStateUI.GetComponentInChildren<TextMeshProUGUI>();
                victoryMessage.color = TeamTools.GetTeamColor(holdingTeam);
                var victorySound = VictoryStateUI.GetComponent<AudioSource>();
                
                if (playerTeam == null)
                {
                    //AI has won against AI, display victory message
                    victoryMessage.text = string.Format("AI WINS!");
                    MusicPlayer.FadeVolume(.2f, 3);
                }
                else if (holdingTeam == playerTeam?.TeamId)
                {
                    //player has won, display victory message
                    victoryMessage.text = string.Format("VICTORY!");
                    MusicPlayer.FadeVolume(.2f, 3);
                }
                else
                {
                    //AI has won againts player, display defeat message
                    victoryMessage.text = string.Format("DEFEAT...");
                    MusicPlayer.FadeVolume(0, 3);
                    
                }
                
                VictoryStateUI.SetActive(true);
                victorySound.Play();
                GameMenu.ShowMenu(true);
            }
        }

    }
    #endregion
    #region public methods
    public Vector3 GetAIObjective()
    {
        return Objective.transform.position;
    }
    public Vector3 GetAISpawnPoint(int team)
    {
        for(int i = 0; i < Teams.Count; i++)
        {
            if(Teams[i].Team == team)
            {
                return Teams[i].SpawnPoint.transform.position;
            }
        }
        Debug.LogError("Spawn point for team " + team + " not found!");
        return new Vector3();
        
    }
    #endregion
    #region private methods

    #endregion
}
