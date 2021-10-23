using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipContentController : MonoBehaviour
{
    #region public fields
    public string Header;
    public string Body;
    public string[] Stats;
    public string AmmoCost;
    public string HealthCost;
    public KeyBindConfigModel.KeyBindId MainShortcut;
    public string AltShortcutType;
    public KeyBindConfigModel.KeyBindId AltShortcut;
    public bool UseKeyBindData;
    public bool Updates;
    #endregion
    #region properties
    public KeyBindModel MainKeyBind { get; private set; }
    public KeyBindModel AltKeyBind { get; private set; }
    #endregion
    #region unity methods
    private void Start()
    {
        MainKeyBind = KeyBindConfigSettings.KeyBinds.GetKeyBindById(MainShortcut);
        AltKeyBind = KeyBindConfigSettings.KeyBinds.GetKeyBindById(AltShortcut);

        if (UseKeyBindData)
        {
            Header = MainKeyBind.Name;
            Body = MainKeyBind.Description;
        }
    }
    #endregion
    #region public methods
    public void Clear()
    {
        Header = "";
        Body = "";
        Stats = new string[] { };
        AmmoCost = "";
        HealthCost = "";
    }
    #endregion
}
