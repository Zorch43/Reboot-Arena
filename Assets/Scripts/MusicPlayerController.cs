using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public AudioSource Player;
    public AudioClip[] PlayList;
    public bool Shuffle;
    #endregion
    #region private fields
    int currentTrack = 0;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Update is called once per frame
    void Update()
    {
        if (!Player.isPlaying)
        {
            //get a random track, different from the current track
            if (Shuffle)
            {
                List<AudioClip> validClips = new List<AudioClip>();
                foreach(var c in PlayList)
                {
                    if(c != Player.clip)
                    {
                        validClips.Add(c);
                    }
                }
                if(validClips.Count > 0)
                {
                    Player.clip = validClips[Random.Range(0, validClips.Count - 1)];
                    Player.Play();
                }
                
            }
            else
            {
                if(currentTrack < 0 || currentTrack >= PlayList.Length)
                {
                    currentTrack = 0;
                }
                Player.clip = PlayList[currentTrack];
                Player.Play();
                currentTrack++;
            }
        }
    }
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
