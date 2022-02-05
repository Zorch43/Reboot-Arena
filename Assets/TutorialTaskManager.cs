using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTaskManager : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public TutorialMessageController MessageBox;//tutotial message controller UI
    public TutorialChecklistController Checklist;//tutorial checklist controller UI
    public GameController Game;
    public bool FinishMissionAtEnd;
    public Animator AnimationController;
    #endregion
    #region private fields
    private TutorialTaskController[] taskList;
    private int taskIndex = -1;
    #endregion
    #region properties

    #endregion
    #region unity methods

    #endregion
    #region public methods
    public void StartTutorial()
    {
        //populate task list
        ScanTasks();
        //do the first task
        DoNextTask();
    }
    public void ScanTasks()
    {
        taskList = GetComponentsInChildren<TutorialTaskController>();
    }
    public void DoNextTask()
    {
        taskIndex++;
        if(taskIndex < taskList.Length)
        {
            taskList[taskIndex].StartTask();
        }
        else if(FinishMissionAtEnd)
        {
            //tutorial end
            Game.FinishTutorial();
        }
    }
    #endregion
    #region private methods

    #endregion
}
