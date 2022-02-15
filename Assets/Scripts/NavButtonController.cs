using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavButtonController : MonoBehaviour
{
    #region public fields
    public GameObject CurrentPage;
    public GameObject NextPage;
    #endregion
    #region private fields
    private Button navButton;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        navButton = GetComponent<Button>();
        if(navButton != null)
        {
            //wire up button
            navButton.onClick.AddListener(ActionNavigate);
        }
    }
    #endregion
    #region actions
    public void ActionNavigate()
    {
        if(CurrentPage != null)
        {
            CurrentPage.SetActive(false);
        }
        if(NextPage != null)
        {
            NextPage.SetActive(true);
        }
       
    }
    #endregion
}
