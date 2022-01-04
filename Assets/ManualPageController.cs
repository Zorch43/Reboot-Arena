using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualPageController : MonoBehaviour
{
    #region public fields
    public Button[] NavButtons;
    public ManualTopicButton[] NavTopics;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //wire up buttons
        for(int i = 0; i < NavButtons.Length; i++)
        {
            NavButtons[i].onClick.AddListener(NavTopics[i].ActionSelectTopic);
        }
    }
    #endregion
    #region actions

    #endregion
}
