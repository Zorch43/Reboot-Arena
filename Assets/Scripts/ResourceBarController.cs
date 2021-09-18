using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBarController : MonoBehaviour
{
    #region constants
    const float BAR_WIDTH = 0.32f;
    #endregion
    #region public fields
    public SpriteRenderer Filling;
    #endregion
    #region private fields
    #endregion
    #region properties

    #endregion
    #region unity methods

    #endregion
    #region public methods
    public void UpdateBar(float max, float current)
    {
        float fillPercent = Math.Min(Math.Max(current / max, 0), 1);
        
        Filling.size = new Vector2(BAR_WIDTH * fillPercent, Filling.size.y);
    }
    #endregion
}
