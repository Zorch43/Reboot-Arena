using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualTopicButton : MonoBehaviour
{
    #region public fields
    public GameObject TopicContent;
    public Button TopicButton;
    public string GroupName;
    public bool IsSubTopic;
    #endregion
    #region private fields
    private List<ManualTopicButton> topicList;
    private ContentPageViewer pageViewer;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //populate sub-topic list
        var list = transform.parent.GetComponentsInChildren<ManualTopicButton>(true);
        topicList = new List<ManualTopicButton>(list);

        //get the page viewer
        pageViewer = GetComponentInParent<ContentPageViewer>();

        //wire up button
        TopicButton.onClick.AddListener(ActionSelectTopic);
    }
    #endregion
    #region actions
    public void ActionSelectTopic()
    {
        //TODO: show associated content page
        pageViewer.ShowPage(TopicContent);
        if (!IsSubTopic)
        {
            foreach (var t in topicList)
            {
                if (t.IsSubTopic)
                {
                    //show the  associated sub-topics
                    //hide non-associated sub-topics
                    t.gameObject.SetActive(t.GroupName == GroupName);
                }
            }
        }
    }
    #endregion
}
