using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenuController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public ConfirmationMenuController ConfirmDialog;
    public Button ExitButton;
    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        ExitButton.onClick.AddListener(ActionExit);
    }
    #endregion
    #region actions
    public void ActionExit()
    {
        ConfirmDialog.GetConfirmation("Are you sure you want to quit?", Application.Quit);
    }
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
