using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimerController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public TextMeshProUGUI[] TimerTextList;
    #endregion
    #region private fields
    private List<TimerModel> timers = new List<TimerModel>();
    private List<int> teams = new List<int>();
    #endregion
    #region properties
    #endregion
    #region unity methods

    #endregion
    #region public methods
    public void Setup(float initialSeconds, List<TeamController> teams)
    {
        for(int i = 0; i < teams.Count; i++)
        {
            var timer = new TimerModel() { RawTime = initialSeconds };
            timers.Add(timer);
            this.teams.Add(teams[i].Team);
            TimerTextList[i].color = TeamTools.GetTeamColor(teams[i].Team);
            TimerTextList[i].text = timer.ToString();
            TimerTextList[i].gameObject.SetActive(true);
        }

    }
    public float UpdateTimer(float time, int team)
    {
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i] == team)
            {
                return UpdateTimer(time, timers[i], TimerTextList[i]);
            }
        }
        Debug.LogError("Invalid team does not have a timer");
        return -1;
    }
    public float GetRawTime(int team)
    {
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i] == team)
            {
                return timers[i].RawTime;
            }
        }
        Debug.LogError("Invalid team does not have a timer");
        return -999;
    }
    #endregion
    #region private methods
    private float UpdateTimer(float time, TimerModel timer, TextMeshProUGUI timerText)
    {
        timer.RawTime += time;
        timer.RawTime = Mathf.Max(timer.RawTime, 0);
        timerText.text = timer.ToString();
        return timer.RawTime;
    }
    #endregion
}
