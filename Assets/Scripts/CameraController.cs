using Assets.Scripts.Utility;
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
    public MapController Map;
    public Transform RotationJoint;
    public Transform PanJoint;
    public Transform TiltJoint;
    public Transform ZoomJoint;
    #endregion
    #region private fields
    private Rect viewRectMain;
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

        bool isSpectating = GameController.GetBattleConfig().IsPlayerSpectator;
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

        //set the map area that the camera can center on without revealing the edges
        mapBox = Map.Terrain.GetComponent<BoxCollider>().bounds;
    }

    // Update is called once per frame
    void Update()
    {

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
           
        }
    }
    public void SetZoom(float zoom, bool redraw = true)
    {
        zoom = Mathf.Clamp(zoom, MIN_ZOOM, MAX_ZOOM);
        ZoomJoint.localPosition = new Vector3(0, 0, zoom);
        if (redraw)
        {
            
        }
    }
    public void ResetOrientation()
    {
        SetRotation(defaultRotation);
        SetTilt(defaultTilt, false);
        SetZoom(defaultZoom, false);
    }
    public bool IsPointInMainMapBounds(Vector2 screenPos)
    {
        return MainViewRect.Contains(screenPos);
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
