using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    #region constants
    public const float SCROLL_ZONE_WIDTH = 20;
    public const float SCROLL_SPEED = 10f;
    #endregion
    #region public fields
    public Camera Camera;
    public MapController Map;
    public GameObject ViewRect;
    public GameObject InitialView;
    public LineRenderer ViewBounds;
    #endregion
    #region private fields
    private Rect viewRect;
    private Rect scrollRect;
    private Rect screenRect;
    private Bounds mapBox;
    private Vector3 cameraOffset;
    private Plane terrainPlane = new Plane(new Vector3(0, 1, 0), 0);
    #endregion
    #region properties
    public Rect MainViewRect
    {
        get
        {
            return viewRect;
        }
    }
    #endregion
    #region unity methods
    void Awake()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);

        //define strategy screen area
        viewRect = ((RectTransform)(ViewRect.transform)).rect;
        viewRect.center -= viewRect.min;

        //set non-scrollable screen area
        var buffer = new Vector2(SCROLL_ZONE_WIDTH, SCROLL_ZONE_WIDTH);
        scrollRect = new Rect(screenRect.min + buffer, screenRect.size - 2 * buffer);

        //set camera viewport dimensions
        Camera.rect = new Rect(Camera.rect.x, Camera.rect.y, viewRect.width / screenRect.width, 1);

        //set the map area that the camera can center on without revealing the edges
        mapBox = Map.Terrain.GetComponent<BoxCollider>().bounds;

        //store the initial camera offset
        cameraOffset = GetCameraOffset();

        //pan to initial location
        PanToMapLocation(InitialView.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //if in border zone, scroll the camera
        var panVector = GetPanVector(Input.mousePosition);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            panVector.x = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            panVector.x = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            panVector.y = -1;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            panVector.y = 1;
        }
        if (panVector.magnitude > 0)
        {
            panVector = panVector * SCROLL_SPEED * Time.deltaTime;
            var panVector3d = new Vector3(panVector.x, 0, panVector.y);

            Camera.transform.position = ClampCameraPosition(Camera.transform.position + panVector3d);
            RedrawViewBounds();
        }

    }
    #endregion
    #region public methods
    public void PanToMapLocation(Vector3 location)
    {
        //clamp center to map edges
        var cameraPosNew = ClampCameraPosition(new Vector3(location.x, Camera.transform.position.y, location.z) + cameraOffset);

        Camera.transform.position = cameraPosNew;
        RedrawViewBounds();
    }
    #endregion
    #region private methods
    private Vector2 GetPanVector(Vector2 mousePos)
    {
        var scrollVector = new Vector2(0, 0);
        if (!scrollRect.Contains(mousePos))
        {
            scrollVector = mousePos - screenRect.center;
            return scrollVector.normalized;
        }
        return scrollVector;
    }
    private Vector3 ClampCameraPosition(Vector3 cameraPos)
    {
        //clamp location to terrain
        var cMax = mapBox.max + cameraOffset;
        var cMin = mapBox.min + cameraOffset;

        var clampedLocation = new Vector3(Mathf.Clamp(cameraPos.x, cMin.x, cMax.x), cameraPos.y, Mathf.Clamp(cameraPos.z, cMin.z, cMax.z));

        return clampedLocation;
    }
    private void RedrawViewBounds()
    {
        //get all corners of the main viewport
        Vector3 p0 = GetMapPointFromScreenPoint(viewRect.min);
        Vector3 p1 = GetMapPointFromScreenPoint(new Vector2(viewRect.min.x, viewRect.max.y));
        Vector3 p2 = GetMapPointFromScreenPoint(viewRect.max);
        Vector3 p3 = GetMapPointFromScreenPoint(new Vector2(viewRect.max.x, viewRect.min.y));

        p0.y = 10;
        p1.y = 10;
        p2.y = 10;
        p3.y = 10;

        ViewBounds.SetPositions(new Vector3[] { p0, p1, p2, p3 });

    }
    private Vector3 GetCameraOffset()
    {
        //project a ray from the center of the camera
        //get intersection point with terrain plane
        var mapPoint = GetMapPointFromScreenPoint(viewRect.center);
        //return xz offset of intersection point in relation to the camera
        var offset = Camera.transform.position - mapPoint;
        offset.y = 0;
        return offset;
    }
    private Vector3 GetMapPointFromScreenPoint(Vector3 screenPoint)
    {
        var ray = Camera.ScreenPointToRay(screenPoint);
        float enter = 0;
        Vector3 hit = new Vector3();
        if(terrainPlane.Raycast(ray, out enter))
        {
            hit = ray.GetPoint(enter);
        }
        return hit;
    }
    #endregion
}
