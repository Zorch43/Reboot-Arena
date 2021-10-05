using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public GameObject MiniMapView;
    public MapController Map;
    public Camera Camera;
    public CameraController MainCamera;
    public CommandController CommandInterface;
    #endregion
    #region private fields
    private Rect viewRect;
    private Rect screenRect;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //match camera viewport with window
        screenRect = new Rect(0, 0, Screen.width, Screen.height);

        //define strategy screen area
        viewRect = ((RectTransform)(MiniMapView.transform)).rect;
        viewRect.position += (Vector2)MiniMapView.transform.position;
        Camera.rect = new Rect(viewRect.min.x/screenRect.width, viewRect.min.y / screenRect.height, 
            viewRect.width / screenRect.width, viewRect.height / screenRect.height);
        //set camera so that it can see the entire map
        Camera.orthographicSize = Mathf.Max(Map.Size.x, Map.Size.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (viewRect.Contains(Input.mousePosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                MainCamera.PanToMapLocation(Camera.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CommandInterface.GiveOrder(Input.mousePosition, Camera);
            }
        }
        
    }
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
