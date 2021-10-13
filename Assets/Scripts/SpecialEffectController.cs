using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffectController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public AudioSource SoundEffect;
    public ParticleSystem ParticleEffect;
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
        //once play is completed, remove the special effect
        if (!SoundEffect.isPlaying && !ParticleEffect.isPlaying)
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region public methods
    public SpecialEffectController Instantiate(Transform transform, Vector3 location)
    {
        var sfx = Instantiate(this, transform);
        sfx.transform.position = location;
        sfx.SoundEffect.Play();
        //sfx.ParticleEffect.Play();//plays automatically, would be a pain to play it manually
        return sfx;
    }
    #endregion
    #region private methods

    #endregion
}
