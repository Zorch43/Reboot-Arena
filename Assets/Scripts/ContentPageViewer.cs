using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentPageViewer : MonoBehaviour
{
    #region public fields
    public GameObject PageParent;
    #endregion
    #region private fields
    private List<GameObject> contentpages;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        var content = PageParent.GetComponentsInChildren<RectTransform>(true);
        contentpages = new List<GameObject>();
        foreach(var c in content)
        {
            //only get direct children
            if (c.parent == PageParent.transform)
            {
                contentpages.Add(c.gameObject);
            }
        }
    }
    #endregion
    #region public methods
    public void ShowPage(GameObject page)
    {
        foreach(var c in contentpages)
        {
            c.SetActive(c == page);
        }
    }
    #endregion
}
