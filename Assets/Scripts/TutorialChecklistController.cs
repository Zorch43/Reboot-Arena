using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialChecklistController : MonoBehaviour
{
    #region public fields
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public GameObject Checklist;
    public TutorialChecklistItemController ChecklistItem;
    #endregion
    #region private fields
    private List<TutorialChecklistItemController> checklistItems = new List<TutorialChecklistItemController>();
    private int completedTasks;
    private UnityAction callback;
    #endregion
    #region properties
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region actions

    #endregion
    #region public methods
    public void Setup(string title, string description, TutorialChecklistItemTask[] items, UnityAction finishCallback)
    {
        //set title
        Title.text = title;
        //set description
        Description.text = description;
        //set callback
        callback = finishCallback;
        //clear checklist
        foreach(var c in checklistItems)
        {
            Destroy(c.gameObject);
        }
        checklistItems.Clear();
        completedTasks = 0;
        //add checklist items
        foreach(var i in items)
        {
            var item = Instantiate(ChecklistItem, Checklist.transform);
            item.Setup(i, this);
            checklistItems.Add(item);
        }
        gameObject.SetActive(true);
    }
    public void CompleteTask()
    {
        completedTasks++;
        if(completedTasks >= checklistItems.Count)
        {
            gameObject.SetActive(false);
            callback.Invoke();
        }
    }
    #endregion
    #region private methods

    #endregion
}
