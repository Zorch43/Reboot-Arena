using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetupController : MonoBehaviour
{
    #region constants
    const int MAX_POSSIBLE_PLAYERS = 8;
    #endregion
    #region public fields
    public Button PlayButton;
    public GameObject SettingsList;
    public TeamSettingController SettingsTemplate;
    public LoadingTransitionController LoadingTransition;
    public bool UseCampaignSettings;
    #endregion
    #region private fields

    #endregion
    #region properties
    public int MaxPlayers { get; set; }
    public string SceneName { get; set; }
    public List<TeamSettingController> Settings { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    private void Start()
    {
        //wire up play button
        PlayButton.onClick.AddListener(ActionPlay);
    }
    #endregion
    #region public methods
    public void RefreshSettings(string scene, int maxPlayers)
    {
        SceneName = scene;
        MaxPlayers = maxPlayers;

        if(Settings == null)
        {
            //set up list of settings
            Settings = new List<TeamSettingController>();
            for (int i = 0; i < MAX_POSSIBLE_PLAYERS; i++)
            {
                var setting = SettingsTemplate.Instantiate(this, string.Format("Team {0}:", i + 1));
                setting.Data.TeamId = i;
                Settings.Add(setting);
            }
        }

        

        for (int i = 0; i < Settings.Count; i++)
        {
            var setting = Settings[i];
            if (i < MaxPlayers)
            {
                setting.gameObject.SetActive(true);
                if (i == 0)
                {
                    setting.Data.Controller = PlayerConfigModel.ControlType.Player;
                }
                else
                {
                    setting.Data.Controller = PlayerConfigModel.ControlType.AI;
                    //var list = AIConfigTemplates.GetAIConfigList();
                    //setting.Data.AIIndex = Random.Range(0, list.Count);
                }
            }
            else
            {
                //setting.SetInteractive(false);
                setting.gameObject.SetActive(false);
            }
            setting.Refresh();
            if (UseCampaignSettings)
            {
                setting.SetInteractive(false);
            }
        }

        gameObject.SetActive(true);
    }
    #endregion
    #region actions
    public void ActionPlay()
    {
        //transfer data to battle config
        if (UseCampaignSettings)
        {
            GameController.BattleConfig = null;//use default map settings
        }
        else
        {
            var config = new BattleConfigModel();
            config.Players = new List<PlayerConfigModel>();
            foreach (var s in Settings)
            {
                if (s.Data.Controller != PlayerConfigModel.ControlType.None)
                {
                    config.Players.Add(s.Data);
                }
            }
            GameController.BattleConfig = config;
        }
        
        //load scene
        LoadingTransition.LoadScene(SceneName, null);
    }
    
    #endregion
    #region private methods

    #endregion
}
