using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialMessageController : MonoBehaviour
{
    #region public fields
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Message;
    public TextMeshProUGUI PageCounter;
    public Button PrevButton;
    public Button NextButton;
    public Button DoneButton;

    public CameraController Cameras;
    #endregion
    #region private fields
    private int _currentPage;
    private Vector2 defaultScreenPos;
    private Vector2 defaultPivot;
    #endregion
    #region properties
    public List<string> Pages { get; set; }
    public UnityAction Callback { get; set; }
    public int CurrentPage
    {
        get
        {
            return _currentPage;
        }
        set
        {
            _currentPage = Mathf.Clamp(value, 1, Pages.Count);
            Refresh();
        }
    }
    public Transform AnchorTransform { get; set; }
    #endregion
    #region unity methods
    private void Awake()
    {
        defaultScreenPos = ((RectTransform)transform).position;
        defaultPivot = ((RectTransform)transform).pivot;

        PrevButton.onClick.AddListener(ActionPrevious);
        NextButton.onClick.AddListener(ActionNext);
        DoneButton.onClick.AddListener(ActionDone);
    }

    // Update is called once per frame
    void Update()
    {
        if(AnchorTransform != null && AnchorTransform as RectTransform == null)
        {
            SetPositionMap(AnchorTransform.position);
        }
    }
    #endregion
    #region actions
    public void ActionPrevious()
    {
        CurrentPage--;
    }
    public void ActionNext()
    {
        CurrentPage++;
    }
    public void ActionDone()
    {
        gameObject.SetActive(false);
        Callback.Invoke();
    }
    #endregion
    #region public methods
    public void Setup(string title, string[] messages, UnityAction finishCallback)
    {
        Title.text = title;
        Pages = new List<string>(messages);
        Callback = finishCallback;
        CurrentPage = 1;
        gameObject.SetActive(true);
        AnchorTransform = null;
    }
    public void Setup(string title, string[] messages, Transform anchor, UnityAction finishCallback)
    {
        Setup(title, messages, finishCallback);
        AnchorTransform = anchor;
        if(anchor != null)
        {
            var rectTransform = anchor as RectTransform;
            if (rectTransform != null)
            {
                SetPositionUI(rectTransform);
            }
        }
        else
        {
            ResetPosition();
        }
    }
    public void SetPositionManual(Vector2 screenPos, bool clamp)
    {
        //pick best pivot
        //set pivot
        float xPivot = 0;
        float yPivot = 0;
        var screenCenter = Cameras.MainViewRect.center;
        if (screenPos.x > screenCenter.x)
        {
            xPivot = 1;
        }
        if (screenPos.y > screenCenter.y)
        {
            yPivot = 1;
        }

        var pivot = new Vector2(xPivot, yPivot);
        //set position
        SetPositionManual(screenPos, pivot, clamp);
    }
    public void SetPositionManual(Vector2 screenPos, Vector2 pivot, bool clamp)
    {
        var rectTransform = transform as RectTransform;
        rectTransform.pivot = pivot;
        //set position
        //clamp to the strategy screen if clamping set
        if (clamp)
        {
            var rect = new Rect(Cameras.MainViewRect);
            //clamp
            rectTransform.position = new Vector2(Mathf.Clamp(screenPos.x, rect.xMin, rect.xMax), Mathf.Clamp(screenPos.y, rect.yMin, rect.yMax));
        }
        else
        {
            rectTransform.position = screenPos;
        }
    }
    public void SetPositionUI(RectTransform uiAnchor)
    {
        
        //get global position of corners
        Vector3[] corners = new Vector3[4];
        uiAnchor.GetWorldCorners(corners);
        Vector3 position = corners[0];
        Vector2 size = new Vector2(
            uiAnchor.lossyScale.x * uiAnchor.rect.size.x,
            uiAnchor.lossyScale.y * uiAnchor.rect.size.y);
        var targetRect = new Rect(position, size);

        //pick inner corner (nearest to center of screen)
        var centerPoint = Cameras.MainViewRect.center;
        float posX;
        float posY;
        if(targetRect.xMin > centerPoint.x)
        {
            posX = targetRect.xMin;
        }
        else
        {
            posX = targetRect.xMax;
        }
        if(targetRect.yMin > centerPoint.y)
        {
            posY = targetRect.yMin;
        }
        else
        {
            posY = targetRect.yMax;
        }
        //set position and anchor
        SetPositionManual(new Vector2(posX, posY), false);
    }
    public void SetPositionMap(Vector3 mapPos)
    {
        //get screen pos
        var screenPos = Cameras.MainCamera.WorldToScreenPoint(mapPos);
        //set position
        SetPositionManual(screenPos, true);
    }
    #endregion
    #region private methods
    private void Refresh()
    {
        PrevButton.gameObject.SetActive(CurrentPage != 1);
        NextButton.gameObject.SetActive(CurrentPage < Pages.Count);
        DoneButton.gameObject.SetActive(CurrentPage == Pages.Count);
        PageCounter.gameObject.SetActive(Pages.Count > 1);
        PageCounter.text = string.Format("{0} / {1}", CurrentPage, Pages.Count);
        Message.text = Pages[CurrentPage - 1];
    }
    private void ResetPosition()
    {
        var rectTransform = transform as RectTransform;
        rectTransform.position = defaultScreenPos;
        rectTransform.pivot = defaultPivot;
    }
    #endregion
}
