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
    public Button PlayButton;
    public string SceneName;
    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        PlayButton.onClick.AddListener(ActionPlay);

    }
    #endregion
    #region actions
    public void ActionPlay()
    {
        SceneManager.LoadSceneAsync(SceneName);
    }
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
