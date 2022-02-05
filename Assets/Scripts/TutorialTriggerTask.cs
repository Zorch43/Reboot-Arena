using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TutorialTriggerTask : TutorialTaskController
{
    #region public fields
    public bool PauseAI;
    public bool PauseObjective;
    #endregion
    #region unity methods

    #endregion
    #region public methods
    public override void StartTask()
    {
        if (manager == null)
        {
            SetupTask();
        }
        //set AI state
        foreach (var t in manager.Game.Teams)
        {
            var ai = t.GetComponent<AIController>();
            if(ai != null)
            {
                ai.SetAIActive(!PauseAI);
            }
        }
        //set objective state
        manager.Game.GameRules.SetObjectiveActive(!PauseObjective);
        FinishTask();
    }
    #endregion

}
