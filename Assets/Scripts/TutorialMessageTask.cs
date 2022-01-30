using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TutorialMessageTask : TutorialTaskController
{
    #region public fields
    
    public Transform AnchorTransform;//UI or map object to anchor message to. leave null to display un-anchored
    public string Title;
    public string[] Messages;
    #endregion
    #region private fields

    #endregion
    #region unity methods

    #endregion
    #region public methods
    public override void StartTask()
    {
        if(manager == null)
        {
            SetupTask();
        }
        manager.MessageBox.Setup(Title, Messages, AnchorTransform, FinishTask);
        manager.AnimationController.SetBool("Message", true);
    }
    #endregion
    #region protected methods
    protected override void FinishTask()
    {
        manager.AnimationController.SetBool("Message", false);
    }
    #endregion

}
