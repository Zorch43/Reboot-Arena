using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableEffect : MonoBehaviour
{
    #region public fields
    public ParticleSystem Effect;
    #endregion
    #region public methods
    public void PlayEffect(float intensity)
    {
        if(intensity > 1)
        {
            //modify the emission rate, then play the effect
            var emission = Effect.emission;
            emission.rateOverTime = intensity/25;
            Effect.Play();
        }
    }
    #endregion
}
