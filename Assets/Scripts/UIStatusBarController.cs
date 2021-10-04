using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatusBarController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public GameObject BarFill;
    public TextMeshProUGUI BarText;
    #endregion
    #region private fields
    float maxWidth;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        maxWidth = ((RectTransform)(BarFill.transform)).rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region public methods
    public void UpdateBar(float current, float max)
    {
        current = Mathf.Min(current, max);
        var ratio = current / max;
        ((RectTransform)(BarFill.transform)).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, maxWidth * (1 - ratio), maxWidth * ratio);
        BarText.text = string.Format("{0}/{1}", Mathf.RoundToInt(current), Mathf.RoundToInt(max));
    }
    #endregion
    #region private methods

    #endregion
}
