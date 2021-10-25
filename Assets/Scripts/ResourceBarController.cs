using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBarController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public SpriteRenderer Filling;
    #endregion
    #region private fields
    private float maxWidth;
    #endregion
    #region properties

    #endregion
    #region unity methods
    private void Start()
    {
        maxWidth = Filling.size.x;
    }
    #endregion
    #region public methods
    public void UpdateBar(float max, float current)
    {
        float fillPercent = Math.Min(Math.Max(current / max, 0), 1);
        
        Filling.size = new Vector2(maxWidth * fillPercent, Filling.size.y);
        Filling.transform.localPosition = new Vector3(-maxWidth * (1-fillPercent) / 2, Filling.transform.localPosition.y, Filling.transform.localPosition.z);
    }
    #endregion
}
