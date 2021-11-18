using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePointController : MonoBehaviour
{
    #region constants
    const float CAPTURE_RATE = 0.05f;//5% per second per unit
    const float DECAY_RATE = 0.1f;//lose 10% per second while unfinished capture is decaying
    const float DECAY_DELAY = 5;//wait 5 seconds until decay starts
    const float PROGRESS_SOUND_INTERVAL = 2;//interval to play the progress sound effect while capturing/decapturing
    #endregion
    #region public fields
    public SpriteRenderer OwnerIndicator;
    public AudioSource PointSound;
    public AudioClip CaptureSound;
    public AudioClip UncaptureSound;
    public AudioClip CaptureCompleteSound;

    #endregion
    #region private fields
   
    List<UnitController> units = new List<UnitController>();
    
    float decayTimer = 0;
    float maxProgressSize;
    float progressSoundTimer = PROGRESS_SOUND_INTERVAL;
    #endregion
    #region properties
    public int NextOwner { get; set; } = -1;//the next owner of the point, if current capture attempt succeeds
    public int CurrOwner { get; set; } = -1;//current owner of the point
    public float CaptureProgress { get; set; } = 0;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        maxProgressSize = OwnerIndicator.gameObject.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        //check how many units of each team are contesting the point
        List<int> teams = new List<int>();
        foreach(var u in units)
        {
            bool found = false;
            foreach(var t in teams)
            {
                if(u.Data.Team == t)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                teams.Add(u.Data.Team);
            }
        }

        int captureDirection = 0;

        //if there are units of only one team on the point, capture the point for that team
        if(teams.Count == 1)
        {
            //if the point is neutral, advance capture progress for this team
            if (CurrOwner == -1)
            {
                //if another team has progress, revert progress at an accelerated rate
                if(NextOwner != teams[0] && CaptureProgress > .01f)
                {
                    CaptureProgress -= (units.Count * CAPTURE_RATE + DECAY_RATE) * Time.deltaTime;
                    captureDirection = -1;
                }
                //advance progress
                else
                {
                    NextOwner = teams[0];
                    CaptureProgress += units.Count * CAPTURE_RATE * Time.deltaTime;
                    captureDirection = 1;
                    if(CaptureProgress >= 1)
                    {
                        //complete capture
                        CurrOwner = teams[0];
                    }
                    //reset decay timer;
                    decayTimer = 0;
                }
            }
            //if the point is already captured, advance capture progress at an accelerated rate
            else if (CurrOwner == teams[0] && CaptureProgress < 1)
            {
                CaptureProgress += (units.Count * CAPTURE_RATE + DECAY_RATE) * Time.deltaTime;
                captureDirection = 1;
                
            }
            //if the point is currently owned by another team, revert capture progress to neutral
            else if(CurrOwner != teams[0])
            {
                CaptureProgress -= units.Count * CAPTURE_RATE * Time.deltaTime;
                captureDirection = -1;

                if(CaptureProgress <= 0)
                {
                    //complete reversion
                    CurrOwner = -1;
                }
                //reset decay timer
                decayTimer = 0;
            }
        }
        //if there are no units on the point, and capture is incomplete, decay capture progress
        else if(teams.Count == 0)
        {
            //run decay timer
            decayTimer += Time.deltaTime;
            //once decay timer has finished, revert progress
            if(decayTimer >= DECAY_DELAY)
            {
                decayTimer = DECAY_DELAY;
                //if the point is neutral, but has capture progress, decay progress to empty
                if(CurrOwner == -1 && CaptureProgress > 0.01f)
                {
                    CaptureProgress -= DECAY_RATE * Time.deltaTime;
                    captureDirection = -1;
                }
                //if the point is owned by a team, but is partially reverted, decay progress to full
                else if(CurrOwner != -1 && CaptureProgress < 1)
                {
                    CaptureProgress += DECAY_RATE * Time.deltaTime;
                    captureDirection = 1;
                }
            }
        }

        //clamp progress value
        CaptureProgress = Mathf.Clamp((float)CaptureProgress, 0, 1);

        //compare last capture progress to current
        if(captureDirection < 0)
        {
            
            if(CaptureProgress == 0)
            {
                PlayCaptureCompleteSound();
            }
            else
            {
                PlayUncaptureSound();
            }
        }
        else if(captureDirection > 0)
        {
           
            if (CaptureProgress == 1)
            {
                PlayCaptureCompleteSound();
            }
            else
            {
                PlayCaptureSound();
            }
        }

        //update ownership visual
        var teamColor = TeamTools.GetTeamColor(NextOwner);
        OwnerIndicator.gameObject.SetActive(NextOwner != -1 && CaptureProgress > .01f);
        OwnerIndicator.gameObject.transform.localScale = new Vector3(maxProgressSize * (float)CaptureProgress, maxProgressSize * (float)CaptureProgress, 1);
        OwnerIndicator.color = teamColor;
        
        //clear list of units
        units.Clear();
    }
    private void OnTriggerStay(Collider other)
    {
        var unit = other.gameObject.GetComponent<UnitController>();
        if(unit != null)
        {
            units.Add(unit);
        }
    }
    #endregion
    #region public methods

    #endregion
    #region private methods
    private void PlayCaptureSound()
    {

        progressSoundTimer -= Time.deltaTime;
        if(progressSoundTimer <= 0)
        {
            progressSoundTimer = PROGRESS_SOUND_INTERVAL;
            PointSound.clip = CaptureSound;
            PointSound.Play();
        }
    }
    private void PlayUncaptureSound()
    {
        progressSoundTimer -= Time.deltaTime;
        if (progressSoundTimer <= 0)
        {
            progressSoundTimer = PROGRESS_SOUND_INTERVAL;
            PointSound.clip = UncaptureSound;
            PointSound.Play();
        }
    }
    private void PlayCaptureCompleteSound()
    {
        progressSoundTimer = PROGRESS_SOUND_INTERVAL;
        PointSound.clip = CaptureCompleteSound;
        PointSound.Play();
    }
    #endregion
}
