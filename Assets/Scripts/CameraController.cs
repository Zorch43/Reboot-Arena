using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    #region constants
    public const float SCROLL_ZONE_WIDTH = 16;
    public const float SCROLL_SPEED = 3.2f;
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
    private Bounds cameraBox;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
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
        var mapBox = Map.Terrain.GetComponent<BoxCollider>();
        var viewPortMin = Camera.ScreenToWorldPoint(new Vector3(viewRect.min.x, viewRect.min.y, Camera.transform.position.y));
        var viewPortMax = Camera.ScreenToWorldPoint(new Vector3(viewRect.max.x, viewRect.max.y, Camera.transform.position.y));
        var cameraBorder = (viewPortMax - viewPortMin) / 2;
        cameraBox = new Bounds(mapBox.center, mapBox.bounds.size - 2 * cameraBorder);

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
        //clamp to map edges

        var cameraPosNew = ClampCameraPosition(new Vector3(location.x, Camera.transform.position.y, location.z));

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
    private Vector3 ClampCameraPosition(Vector3 location)
    {
        return new Vector3(Mathf.Clamp(location.x, cameraBox.min.x, cameraBox.max.x),
            location.y, Mathf.Clamp(location.z, cameraBox.min.z, cameraBox.max.z));
    }
    private void RedrawViewBounds()
    {
        //get all corners of the main viewport
        Vector3 p0 = Camera.ScreenToWorldPoint(new Vector3(viewRect.min.x, viewRect.min.y, Camera.transform.position.y));
        Vector3 p1 = Camera.ScreenToWorldPoint(new Vector3(viewRect.min.x, viewRect.max.y, Camera.transform.position.y));
        Vector3 p2 = Camera.ScreenToWorldPoint(new Vector3(viewRect.max.x, viewRect.max.y, Camera.transform.position.y));
        Vector3 p3 = Camera.ScreenToWorldPoint(new Vector3(viewRect.max.x, viewRect.min.y, Camera.transform.position.y));

        p0.y = 10;
        p1.y = 10;
        p2.y = 10;
        p3.y = 10;

        ViewBounds.SetPositions(new Vector3[] { p0, p1, p2, p3 });

    }
    #endregion
}
