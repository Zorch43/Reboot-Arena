using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenuController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public GameObject PlayView;
    public GameObject InfoView;
    public GameObject CreditsView;
    public ConfirmationMenuController ConfirmDialog;
    public Button PlayButton;
    public Button InfoButton;
    public Button CreditsButton;
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
        PlayButton.onClick.AddListener(ActionPlay);
        InfoButton.onClick.AddListener(ActionInfo);
        CreditsButton.onClick.AddListener(ActionCredits);
        ExitButton.onClick.AddListener(ActionExit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region actions
    public void ActionPlay()
    {
        PlayView.SetActive(true);
        gameObject.SetActive(false);
    }
    public void ActionInfo()
    {
        InfoView.SetActive(true);
        gameObject.SetActive(false);
    }
    public void ActionCredits()
    {
        CreditsView.SetActive(true);
        gameObject.SetActive(false);
    }
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
