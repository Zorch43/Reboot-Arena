using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    #region constants
    public const float SCROLL_ZONE_WIDTH = 20;
    public const float PAN_SPEED = 25f;
    public const float SUPER_PAN_SPEED = 75f;
    const float EDGE_LINE_HEIGHT = 35;
    const float MAX_TILT = 90f;
    const float MIN_TILT = 30.1f;//minimum must be above fov/2, otherwise the minimap view doesn't display correctly
    const float MAX_ZOOM = -8;//minimum distance from the focus target
    const float MIN_ZOOM = -32;//maximum distance from the focus target
    const float TILT_SPEED = 60;
    const float TURN_SPEED = 120;
    const float ZOOM_SPEED = (MIN_ZOOM - MAX_ZOOM)  * -2;
    #endregion
    #region public fields
    public Camera MainCamera;
    public RectTransform ViewRect_Main;
    public Camera MiniMapCamera;
    public RectTransform ViewRect_Mini;
    public LineRenderer ViewBounds;
    public MapController Map;
    public Transform RotationJoint;
    public Transform PanJoint;
    public Transform TiltJoint;
    public Transform ZoomJoint;
    #endregion
    #region private fields
    private Rect viewRectMain;
    private Rect viewRectMini;
    private Rect scrollRect;
    private Rect screenRect;
    private Bounds mapBox;
    private Plane terrainPlane = new Plane(new Vector3(0, 1, 0), 0);
    private float defaultZoom;
    private float defaultRotation;
    private float defaultTilt;
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
    public Vector2 PanVector { get; set; }
    public Vector2 TiltVector { get; set; }
    public float ZoomDelta { get; set; }
    #endregion
    #region unity methods
    void Awake()
    {
        defaultZoom = ZoomJoint.localPosition.z;
        defaultRotation = RotationJoint.localEulerAngles.y;
        defaultTilt = TiltJoint.localEulerAngles.x;

        bool isSpectating = GameController.BattleConfig.IsPlayerSpectator;
        screenRect = new Rect(0, 0, Screen.width, Screen.height);

        //set non-scrollable screen area
        var buffer = new Vector2(SCROLL_ZONE_WIDTH, SCROLL_ZONE_WIDTH);
        scrollRect = new Rect(screenRect.min + buffer, screenRect.size - 2 * buffer);
        if (!isSpectating)
        {
            //set main camera viewport dimensions
            viewRectMain = SetupCameraSpace(MainCamera, ViewRect_Main);
        }
        else
        {
            viewRectMain = screenRect;
            MainCamera.rect = new Rect(0, 0, 1, 1);//full screen
        }

        //set minimap camera viewport dimensions
        viewRectMini = SetupCameraSpace(MiniMapCamera, ViewRect_Mini);

        //set the map area that the camera can center on without revealing the edges
        mapBox = Map.Terrain.GetComponent<BoxCollider>().bounds;

        //set minimap camera so that it can see the entire map
        MiniMapCamera.orthographicSize = Mathf.Max(Map.Size.x, Map.Size.y);
    }
    private void Start()
    {
        RedrawViewBounds();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        //if in border zone, scroll the camera
        var panVector = GetPanVector(Input.mousePosition) + PanVector;

        bool redrawEdge = false;

        //pan the camera
        if (panVector.magnitude > 0)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                panVector = SUPER_PAN_SPEED * deltaTime * panVector;
            }
            else
            {
                panVector = PAN_SPEED * deltaTime * panVector;
            }
            
            var panVector3d = new Vector3(panVector.x, 0, panVector.y);
            //pan the joint in its local space
            PanJoint.localPosition += panVector3d;
            //make the pan joint's parent move in world space, clamp that position within map bounds
            PanJoint.parent.position = ClampCameraPosition(PanJoint.position);
            //reset the pan joint's local position
            PanJoint.localPosition = new Vector3();
        }
        //rotate the camera
        if(Mathf.Abs(TiltVector.x) > 0)
        {
            var rotateDelta = TURN_SPEED * deltaTime * TiltVector.x;
            //rotate the rotation joint around the y axis
            RotationJoint.localEulerAngles += new Vector3(0, rotateDelta, 0);
        }
        //tilt the camera
        if(Mathf.Abs(TiltVector.y) > 0)
        {
            var tiltDelta = TILT_SPEED * deltaTime * TiltVector.y;
            //rotate the tilt joint around the x axis
            //clamp the total angle
            var tilt = Mathf.Clamp(tiltDelta + TiltJoint.localEulerAngles.x, MIN_TILT, MAX_TILT);
            TiltJoint.localEulerAngles = new Vector3(tilt, 0, 0);
            //update the edge renderer
            redrawEdge = true;
        }
        //zoom the camera
        if(Mathf.Abs(ZoomDelta) > 0)
        {
            var zoomDelta = ZOOM_SPEED * deltaTime * ZoomDelta;
            //move the zoom joint's local z by the zoom amount
            //clamp the zoom
            var zoom = Mathf.Clamp(ZoomJoint.localPosition.z + zoomDelta, MIN_ZOOM, MAX_ZOOM);
            ZoomJoint.localPosition = new Vector3(0, 0, zoom);
            //update the edge renderer
            redrawEdge = true;
        }

        //redraw view bounds
        if (redrawEdge)
        {
            RedrawViewBounds();
        }
        
        PanVector = new Vector2();
        TiltVector = new Vector2();
        ZoomDelta = 0;
    }
    #endregion
    #region public methods
    public void PanToMapLocation(Vector3 location)
    {
        //clamp center to map edges
        var cameraPosNew = ClampCameraPosition(new Vector3(location.x, 0, location.z));

        PanJoint.parent.position = cameraPosNew;
    }
    public void SetRotation(float rotation)
    {
        RotationJoint.localEulerAngles = new Vector3(0, rotation, 0);
    }
    public void SetTilt(float tilt, bool redraw = true)
    {
        tilt = Mathf.Clamp(tilt, MIN_TILT, MAX_TILT);
        TiltJoint.localEulerAngles = new Vector3(tilt, 0, 0);
        if (redraw)
        {
            RedrawViewBounds();
        }
    }
    public void SetZoom(float zoom, bool redraw = true)
    {
        zoom = Mathf.Clamp(zoom, MIN_ZOOM, MAX_ZOOM);
        ZoomJoint.localPosition = new Vector3(0, 0, zoom);
        if (redraw)
        {
            RedrawViewBounds();
        }
    }
    public void ResetOrientation()
    {
        SetRotation(defaultRotation);
        SetTilt(defaultTilt, false);
        SetZoom(defaultZoom, false);
        RedrawViewBounds();
    }
    public Camera GetCommandCamera(Vector2 mousePos)
    {
        if (IsPointInMiniMapBounds(mousePos))
        {
            return MiniMapCamera;
        }
        else if (IsPointInMainMapBounds(mousePos))
        {
            return MainCamera;
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
        var cMax = mapBox.max;
        var cMin = mapBox.min;

        var clampedLocation = new Vector3(Mathf.Clamp(cameraPos.x, cMin.x, cMax.x), cameraPos.y, Mathf.Clamp(cameraPos.z, cMin.z, cMax.z));

        return clampedLocation;
    }
    private void RedrawViewBounds()
    {
        //get all corners of the main viewport
        Vector3 p0 = GetMapPointFromScreenPoint(viewRectMain.min) - ViewBounds.transform.position;
        Vector3 p1 = GetMapPointFromScreenPoint(new Vector2(viewRectMain.min.x, viewRectMain.max.y)) - ViewBounds.transform.position;
        Vector3 p2 = GetMapPointFromScreenPoint(viewRectMain.max) - ViewBounds.transform.position;
        Vector3 p3 = GetMapPointFromScreenPoint(new Vector2(viewRectMain.max.x, viewRectMain.min.y)) - ViewBounds.transform.position;

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
        return GetCameraOffset(mapPoint);
    }
    private Vector3 GetCameraOffset(Vector3 mapPoint)
    {
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
