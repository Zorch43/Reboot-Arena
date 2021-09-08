using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region constants
    public const float SCROLL_ZONE_WIDTH = 128;
    public const float SCROLL_SPEED = 0.64f;
    #endregion
    #region public fields
    public Camera Camera;
    public MapController Map;
    #endregion
    #region private fields
    private Rect viewRect;
    private Rect scrollRect;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        viewRect = Camera.pixelRect;
        var buffer = new Vector2(SCROLL_ZONE_WIDTH, SCROLL_ZONE_WIDTH);
        scrollRect = new Rect(viewRect.min + buffer, viewRect.size - 2 * buffer);
        
    }

    // Update is called once per frame
    void Update()
    {
        //if in border zone, scroll the camera
        var scrollVector = GetScrollVector(Input.mousePosition);
        if(scrollVector.magnitude > 0)
        {
            scrollVector = scrollVector * SCROLL_SPEED * Time.deltaTime;
            var mapMin = Camera.WorldToScreenPoint(new Vector2(0, 0));

            var mapRect = new Rect(mapMin, Camera.WorldToScreenPoint(Map.Size) - mapMin);
            if ((scrollVector.x < 0 && viewRect.min.x + scrollVector.x < mapRect.min.x) 
                || (scrollVector.x > 0 && viewRect.max.x + scrollVector.x > mapRect.max.x))
            {
                scrollVector.x = 0;
            }
            if((scrollVector.y < 0 && viewRect.min.y + scrollVector.y < mapRect.min.y) 
                || (scrollVector.y > 0 && viewRect.max.y + scrollVector.y > mapRect.max.y))
            {
                scrollVector.y = 0;
            }
            Camera.transform.position += (Vector3)(scrollVector);
        }
        
    }
    #endregion
    #region public methods

    #endregion
    #region private methods
    private Vector2 GetScrollVector(Vector2 mousePos)
    {
        var scrollVector = new Vector2(0, 0);
        if (!scrollRect.Contains(mousePos) && viewRect.Contains(mousePos))
        {
            scrollVector = mousePos - viewRect.center;
            return scrollVector.normalized;
        }
        return scrollVector;
    }
    #endregion
}
