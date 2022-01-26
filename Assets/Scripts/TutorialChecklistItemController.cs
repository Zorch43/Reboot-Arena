using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialChecklistItemController : MonoBehaviour
{
    #region public fields
    public TextMeshProUGUI Label;
    public TextMeshProUGUI Progress;
    public Image CheckMark;
    #endregion
    #region private fields
    private int _currentProgress;
    #endregion
    #region properties
    public int MaxProgress { get; set; }
    public int CurrentProgress
    {
        get
        {
            return _currentProgress;
        }
        set
        {
            _currentProgress = Mathf.Min(value, MaxProgress);
            Refresh();
        }
    }
    #endregion
    #region unity methods
    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
    #endregion
    #region public methods
    public void Setup(string itemLabel, int maxProgress)
    {
        Label.text = itemLabel;
        MaxProgress = maxProgress;
        CurrentProgress = 0;
        if(MaxProgress <= 1)
        {
            MaxProgress = 1;
            Progress.gameObject.SetActive(false);
        }
    }
    #endregion
    #region private methods
    private void Refresh()
    {
        //update progress
        Progress.text = string.Format("{0}/{1}", CurrentProgress, MaxProgress);
        CheckMark.gameObject.SetActive(CurrentProgress == MaxProgress);
    }
    #endregion
}
