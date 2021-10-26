using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    #region constants
    public const float SCROLL_ZONE_WIDTH = 20;
    public const float SCROLL_SPEED = 10f;
    const float EDGE_LINE_HEIGHT = 25;
    #endregion
    #region public fields
    public Camera MainCamera;
    public RectTransform ViewRect_Main;
    public Camera MiniMapCamera;
    public RectTransform ViewRect_Mini;
    public LineRenderer ViewBounds;
    public MapController Map;
    public GameObject InitialView;
    #endregion
    #region private fields
    private Rect viewRectMain;
    private Rect viewRectMini;
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
            return viewRectMain;
        }
    }
    public Rect MiniViewRect
    {
        get
        {
            return viewRectMini;
        }
    }
    #endregion
    #region unity methods
    void Awake()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);

        //set non-scrollable screen area
        var buffer = new Vector2(SCROLL_ZONE_WIDTH, SCROLL_ZONE_WIDTH);
        scrollRect = new Rect(screenRect.min + buffer, screenRect.size - 2 * buffer);

        //set main camera viewport dimensions
        viewRectMain = SetupCameraSpace(MainCamera, ViewRect_Main);

        //set minimap camera viewport dimensions
        viewRectMini = SetupCameraSpace(MiniMapCamera, ViewRect_Mini);

        //set the map area that the camera can center on without revealing the edges
        mapBox = Map.Terrain.GetComponent<BoxCollider>().bounds;

        //set minimap camera so that it can see the entire map
        MiniMapCamera.orthographicSize = Mathf.Max(Map.Size.x, Map.Size.y);

        //store the initial camera offset
        cameraOffset = GetCameraOffset();

        //TODO: set initial viewBounds
        //TODO: pan them when moving camera
        //TODO: only update when changing camera rotation or angle

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

            MainCamera.transform.position = ClampCameraPosition(MainCamera.transform.position + panVector3d);
            RedrawViewBounds();
        }

    }
    #endregion
    #region public methods
    public void PanToMapLocation(Vector3 location)
    {
        //clamp center to map edges
        var cameraPosNew = ClampCameraPosition(new Vector3(location.x, MainCamera.transform.position.y, location.z) + cameraOffset);

        MainCamera.transform.position = cameraPosNew;
        RedrawViewBounds();
    }
    public Camera GetCommandCamera(Vector2 mousePos)
    {
        if (IsPointInMainMapBounds(mousePos))
        {
            return MainCamera;
        }
        else if (IsPointInMiniMapBounds(mousePos))
        {
            return MiniMapCamera;
        }
        return null;
    }
    public bool IsPointInMainMapBounds(Vector2 screenPos)
    {
        return MainViewRect.Contains(screenPos);
    }
    public bool IsPointInMiniMapBounds(Vector2 screenPos)
    {
        return MiniViewRect.Contains(screenPos);
    }
    #endregion
    #region private methods
    private Rect SetupCameraSpace(Camera camera, RectTransform panelRect)
    {
        //define strategy screen area
        var viewRect = panelRect.rect;
        viewRect.position += (Vector2)panelRect.transform.position;
        camera.rect = new Rect(viewRect.min.x / screenRect.width, viewRect.min.y / screenRect.height,
            viewRect.width / screenRect.width, viewRect.height / screenRect.height);

        return viewRect;
    }
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
        Vector3 p0 = GetMapPointFromScreenPoint(viewRectMain.min);
        Vector3 p1 = GetMapPointFromScreenPoint(new Vector2(viewRectMain.min.x, viewRectMain.max.y));
        Vector3 p2 = GetMapPointFromScreenPoint(viewRectMain.max);
        Vector3 p3 = GetMapPointFromScreenPoint(new Vector2(viewRectMain.max.x, viewRectMain.min.y));

        p0.y = EDGE_LINE_HEIGHT;
        p1.y = EDGE_LINE_HEIGHT;
        p2.y = EDGE_LINE_HEIGHT;
        p3.y = EDGE_LINE_HEIGHT;

        ViewBounds.SetPositions(new Vector3[] { p0, p1, p2, p3 });

    }
    private Vector3 GetCameraOffset()
    {
        //project a ray from the center of the camera
        //get intersection point with terrain plane
        var mapPoint = GetMapPointFromScreenPoint(viewRectMain.center);
        //return xz offset of intersection point in relation to the camera
        var offset = MainCamera.transform.position - mapPoint;
        offset.y = 0;
        return offset;
    }
    private Vector3 GetMapPointFromScreenPoint(Vector3 screenPoint)
    {
        var ray = MainCamera.ScreenPointToRay(screenPoint);
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
