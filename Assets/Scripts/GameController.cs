using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region constants
    private const float COUNTDOWN_TIME = 180;//point must be held for 3 minutes to win
    #endregion
    #region static fields
    public static BattleConfigModel BattleConfig;
    #endregion
    #region public fields
    public GameMenuController GameMenu;
    
    public GameObject GameStateUI;//where to display timer count-downs;
    
    public GameObject VictoryStateUI;//ui that displays victory or defeat message

    public SpawnPointController[] SpawnPoints;//list of all starting spawn points
    public UnitSlotManager PlayerSlotManager;//need this referenced here to apply to player-controlled teams
    public TeamController PlayerTeamTemplate;
    public TeamController AITeamTemplate;

    public MapController Map;
    public CommandController CommandInterface;
    public MusicPlayerController MusicPlayer;

    public GameRulesBase GameRules;
    public TutorialTaskManager Tutorial;

    public PlayerConfigModel[] DefaultBattleConfig;

    #endregion
    #region private fields

    //private GameTimerController timers;
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
                    if(u.Unit != null)
                    {
                        units.Add(u.Unit);
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
                    if (u.Unit != null)
                    {
                        units.Add(u.Unit);
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

        }

        GameRules.Setup();
        Tutorial.StartTutorial();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        foreach(var t in Teams)
        {
            bool victoryState = GameRules.IsTeamVictorious(t.Team, deltaTime);
            if (victoryState)
            {
                var playerTeam = BattleConfig.PlayerTeam;
                var victoryMessage = VictoryStateUI.GetComponentInChildren<TextMeshProUGUI>();
                victoryMessage.color = TeamTools.GetTeamColor(t.Team);
                var victorySound = VictoryStateUI.GetComponent<AudioSource>();

                if (playerTeam == null)
                {
                    //AI has won against AI, display victory message
                    victoryMessage.text = string.Format("AI WINS!");
                    MusicPlayer.FadeVolume(.2f, 3);
                }
                else if (t.Team == playerTeam?.TeamId)
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
                if (!VictoryStateUI.activeSelf)
                {
                    VictoryStateUI.SetActive(true);
                    victorySound.Play();
                }
                GameMenu.ShowMenu(true);
            }
        }
    }
    #endregion
    #region public methods
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
    public void FinishTutorial()
    {
        //player has won, display victory message
        var victoryMessage = VictoryStateUI.GetComponentInChildren<TextMeshProUGUI>();
        victoryMessage.text = string.Format("TUTORIAL COMPLETE");
        MusicPlayer.FadeVolume(.2f, 3);
        if (!VictoryStateUI.activeSelf)
        {
            var victorySound = VictoryStateUI.GetComponent<AudioSource>();
            VictoryStateUI.SetActive(true);
            victorySound.Play();
        }
        GameMenu.ShowMenu(true);
    }

    #endregion
    #region static methods
    public static BattleConfigModel GetBattleConfig()
    {
        if (BattleConfig == null)
        {
            var obj = FindObjectOfType(typeof(GameController)) as GameController;
            BattleConfig = new BattleConfigModel()
            {
                Players = new List<PlayerConfigModel>(obj.DefaultBattleConfig)
            };
        }
        return BattleConfig;
    }
    #endregion
    #region private methods

    #endregion
}
