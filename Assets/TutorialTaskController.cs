using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialTaskController : MonoBehaviour
{
    #region public fields

    #endregion
    #region protected fields
    protected TutorialTaskManager manager;
    #endregion
    #region properties

    #endregion
    #region unity methods

    #endregion
    #region public methods

    #endregion
    #region abstract methods
    public abstract void StartTask();
    #endregion
    #region protected methods
    protected virtual void FinishTask()
    {
        manager.DoNextTask();
    }
    protected virtual void SetupTask()
    {
        //set manager
        manager = GetComponentInParent<TutorialTaskManager>();
    }
    #endregion
}
