using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayViewController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public GameObject MainMenu;
    public Button BackButton;

    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        BackButton.onClick.AddListener(ActionBack);

    }
    #endregion
    #region actions
    public void ActionBack()
    {
        MainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
