using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSelectorController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public Toggle SelectButton;
    public GameSetupController SetupPanel;
    public string SceneName;
    public int MaxPlayers;

    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        SelectButton.onValueChanged.AddListener(ActionSelect);

    }
    #endregion
    #region actions
    public void ActionSelect(bool state)
    {
        if (state)
        {
            SetupPanel.RefreshSettings(SceneName, MaxPlayers);
        }
        else if (!SelectButton.group.AnyTogglesOn())
        {
            SetupPanel.gameObject.SetActive(false);
        }
    }
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
