using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamSettingController : MonoBehaviour
{
    #region public fields
    public TextMeshProUGUI TeamLabel;
    public TMP_Dropdown ControllerSelector;
    public TMP_Dropdown ColorSelector;
    public GameSetupController Setup;
    #endregion
    #region properties
    public PlayerConfigModel Data { get; set; }
    
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //wire up controls
        ControllerSelector.onValueChanged.AddListener(ActionSelectController);
        ColorSelector.onValueChanged.AddListener(ActionSelectColor);
        
    }
    #endregion
    #region actions
    public void ActionSelectController(int state)
    {
        //save the old state
        var oldState = Data.Controller;
        //update model
        Data.Controller = (PlayerConfigModel.ControlType)state;
        //check the list of settings:
        int playerCount = 0;
        int activeCount = 0;
        foreach(var s in Setup.Settings)
        {
            if(s.Data.Controller == PlayerConfigModel.ControlType.Player)
            {
                playerCount++;
            }
            if(s.Data.Controller != PlayerConfigModel.ControlType.None)
            {
                activeCount++;
            }
        }
        //if there is more than 1 team with control type of Player, revert to old state
        //if there are fewer than 2 teams with a control type (other than None), revert to old state
        if ((playerCount > 1 && Data.Controller == PlayerConfigModel.ControlType.Player)
            ||(activeCount < 2 && Data.Controller == PlayerConfigModel.ControlType.None))
        {
            Data.Controller = oldState;
            Refresh();
        }

        //if new state is None, disable color selector
        ColorSelector.interactable = Data.Controller != PlayerConfigModel.ControlType.None;
    }
    public void ActionSelectColor(int state)
    {
        //save old state
        var oldState = Data.TeamId;
        //update model
        Data.TeamId = state;
        //check the list of settings:
        foreach(var s in Setup.Settings)
        {
            if(s != this && s.Data.TeamId == state)
            {
                //if another setting exists that shares this color, assign the old color to it
                s.Data.TeamId = oldState;
                s.Refresh();
                break;
            }
        }
        ColorSelector.image.color = TeamTools.GetTeamColor(state);
    }
    #endregion
    #region public methods
    public TeamSettingController Instantiate(GameSetupController setup, string label)
    {
        var result = Instantiate(this, setup.SettingsList.transform);
        result.Setup = setup;
        result.Data = new PlayerConfigModel();
        result.TeamLabel.text = label;

        //mark each option with team color
        for(int i = 0; i < result.ColorSelector.options.Count; i++)
        {
            var o = result.ColorSelector.options[i];

        }

        return result;
    }
    public void Refresh()
    {
        //set control type from data
        ControllerSelector.SetValueWithoutNotify((int)Data.Controller);
        
        //set color from data
        ColorSelector.SetValueWithoutNotify(Data.TeamId);
        ColorSelector.image.color = TeamTools.GetTeamColor(Data.TeamId);
        ColorSelector.interactable = Data.Controller != PlayerConfigModel.ControlType.None;
    }
    public void SetInteractive(bool state)
    {
        if (state)
        {
            ControllerSelector.interactable = true;
            ColorSelector.interactable = true;
        }
        else
        {
            ControllerSelector.interactable = false;
            //ControllerSelector.SetValueWithoutNotify(0);
            //Data.Controller = PlayerConfigModel.ControlType.None;
            ColorSelector.interactable = false;
        }
    }
    #endregion
}
