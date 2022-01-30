using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TutorialChecklistItemTask : MonoBehaviour
{
    #region public fields
    public string Description;
    public int TaskCount = 1;
    public string EventNameManual;
    public EventList.EventNames EventName;
    public KeyBindConfigModel.KeyBindId KeyBind;
    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods

    #endregion
    #region action

    #endregion
    #region public methods
    public UnityEvent GetEvent()
    {
        if (string.IsNullOrWhiteSpace(EventNameManual))
        {
            return EventList.GetEvent(EventName);
        }
        else
        {
            return EventList.GetEvent(EventNameManual);
        }
    }
    public string GetDescription()
    {
        if(KeyBind == KeyBindConfigModel.KeyBindId.None)
        {
            return Description;
        }
        else
        {
            return string.Format("<color=lightblue>[{0}]</color> {1}", KeyBindConfigSettings.KeyBinds.GetKeyBindById(KeyBind).ToString(), Description);
        }
    }
    #endregion
}
