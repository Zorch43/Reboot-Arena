using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationMenuController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public TextMeshProUGUI Message;
    public Button ConfirmButton;
    public Button CancelButton;
    #endregion
    #region private fields
    Action confirmAction;
    
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        ConfirmButton.onClick.AddListener(ConfirmAction);
        CancelButton.onClick.AddListener(CancelAction);
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
    #region actions
    public void ConfirmAction()
    {
        gameObject.SetActive(false);
        confirmAction.Invoke();
    }
    public void CancelAction()
    {
        gameObject.SetActive(false);
    }
    #endregion
    #region public methods
    public void GetConfirmation(string message, Action yesAction)
    {
        gameObject.SetActive(true);
        Message.text = message;
        confirmAction = yesAction;
    }
    #endregion
    #region private methods

    #endregion
}
