using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRectController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public GameObject SelectionRect;
    #endregion
    #region private fields
    private Action<Rect> callback;
    private Rect viewRect;
    #endregion
    #region properties
    public bool IsDrawingSelection { get; set; }
    public Vector2 StartingPoint { get; set; }
    public Vector2 EndingPoint { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        ClearSelection();
        viewRect = Camera.main.GetComponent<CameraController>().MainViewRect;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDrawingSelection)
        {
            EndingPoint = new Vector2(
                Mathf.Clamp(Input.mousePosition.x, viewRect.min.x, viewRect.max.x), 
                Mathf.Clamp(Input.mousePosition.y, viewRect.min.y, viewRect.max.y));

            var rect = SelectionRect.transform as RectTransform;
            var selectionRect = CreateRectArea(StartingPoint, EndingPoint);
            rect.position = selectionRect.position;
            rect.sizeDelta = selectionRect.size;
            SelectionRect.SetActive(true);
            if (Input.GetMouseButtonUp(0))
            {
                callback(selectionRect);
                ClearSelection();
            }
            
        }
    }
    #endregion
    #region public methods
    public void StartSelection(Vector2 startPoint, Action<Rect> callback)
    {
        IsDrawingSelection = true;
        StartingPoint = new Vector2(
                Mathf.Clamp(startPoint.x, viewRect.min.x, viewRect.max.x),
                Mathf.Clamp(startPoint.y, viewRect.min.y, viewRect.max.y));
        this.callback = callback;
    }
    public void ClearSelection()
    {
        IsDrawingSelection = false;
        SelectionRect.SetActive(false);
        StartingPoint = new Vector2();
        EndingPoint = new Vector2();
    }
    #endregion
    #region static methods
    public static Rect CreateRectArea(Vector2 startPoint, Vector2 endPoint)
    {
        var origin = new Vector2(Math.Min(startPoint.x, endPoint.x), Math.Min(startPoint.y, endPoint.y));
        var size = new Vector2(Math.Abs(startPoint.x - endPoint.x), Math.Abs(startPoint.y - endPoint.y));
        return new Rect(origin, size);
    }
    #endregion
}
