using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KotHTimerController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public TextMeshProUGUI TimerText1, TimerText2;
    #endregion
    #region private fields
    private TimerModel timer1, timer2;
    private int team1, team2;
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
    #region public methods
    public void Setup(float initialSeconds, int team1, int team2)
    {
        timer1 = new TimerModel() { RawTime = initialSeconds };
        timer2 = new TimerModel() { RawTime = initialSeconds };

        TimerText1.color = TeamTools.GetTeamColor(team1);
        TimerText2.color = TeamTools.GetTeamColor(team2);

        TimerText1.text = timer1.ToString();
        TimerText2.text = timer2.ToString();

        this.team1 = team1;
        this.team2 = team2;
    }
    public float UpdateTimer(float time, int team)
    {
        if(team == team1)
        {
            return UpdateTimer(time, timer1, TimerText1);
        }
        else if(team == team2)
        {
            return UpdateTimer(time, timer2, TimerText2);
        }
        else
        {
            Debug.LogError("Invalid team does not have a timer");
            return -1;
        }
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
