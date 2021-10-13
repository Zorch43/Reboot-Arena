using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPointController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public AudioSource AudioPlayer;
    #endregion
    #region private fields

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
        //once play is compled, remove the soundpoint
        if (!AudioPlayer.isPlaying)
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region public methods
    public SoundPointController Instantiate(AudioClip clip, Transform transform, Vector3 location)
    {
        var soundPoint = Instantiate(this, transform);
        soundPoint.transform.position = location;
        soundPoint.AudioPlayer.clip = clip;
        soundPoint.AudioPlayer.Play();
        return soundPoint;
    }
    #endregion
    #region private methods

    #endregion

}
