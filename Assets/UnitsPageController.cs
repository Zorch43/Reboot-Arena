using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitsPageController : MonoBehaviour
{
    #region public fields
    public Button TrooperClassButton;
    public ManualTopicButton TrooperTopic;
    public Button RangerClassButton;
    public ManualTopicButton RangerTopic;
    public Button FabricatorClassButton;
    public ManualTopicButton FabricatorTopic;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //wire up buttons
        TrooperClassButton.onClick.AddListener(TrooperTopic.ActionSelectTopic);
        RangerClassButton.onClick.AddListener(RangerTopic.ActionSelectTopic);
        FabricatorClassButton.onClick.AddListener(FabricatorTopic.ActionSelectTopic);
    }
    #endregion
    #region actions

    #endregion
}
