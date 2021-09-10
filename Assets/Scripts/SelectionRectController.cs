using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRectController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields

    #endregion
    #region private fields
    Action<Rect> callback;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDrawingSelection)
        {
            EndingPoint = Input.mousePosition;
            var rect = transform as RectTransform;
            var selectionRect = CreateRectArea(StartingPoint, EndingPoint);
            rect.position = selectionRect.position;
            rect.sizeDelta = selectionRect.size;
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Rectangular selection finished");
                
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
        gameObject.SetActive(true);
        StartingPoint = startPoint;
        this.callback = callback;
    }
    public void ClearSelection()
    {
        IsDrawingSelection = false;
        gameObject.SetActive(false);
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
