using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TutorialChecklistTask : TutorialTaskController
{
    #region public fields
    public string Title;
    public string Description;
    #endregion
    #region private fields
    private TutorialChecklistItemTask[] checklistItems;
    #endregion
    #region public methods
    public override void StartTask()
    {
        if (manager == null)
        {
            SetupTask();
        }
        manager.Checklist.Setup(Title, Description, checklistItems, FinishTask);
        manager.AnimationController.SetBool("Checklist", true);
    }
    #endregion
    #region protected methods
    protected override void SetupTask()
    {
        base.SetupTask();
        checklistItems = GetComponentsInChildren<TutorialChecklistItemTask>();
    }
    protected override void FinishTask()
    {
        manager.AnimationController.SetBool("Checklist", false);
    }
    #endregion

}
