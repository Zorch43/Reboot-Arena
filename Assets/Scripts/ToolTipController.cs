using Assets.Scripts.Data_Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTipController : MonoBehaviour
{
    #region constants
    const float HOVER_TIME = 0.5f;//time to hover before tooltip shows up
    const float CLICK_COOLDOWN = 2;//time after a click that tooltips don't show up
    const float ANIMATION_SPEED = 0.5f;//time to take expanding the tooltip to full size
    //style
    const int HEADER_FONT_SIZE = 150;//percentage
    const int BODY_FONT_SIZE = 100;//percentage
    const int STAT_FONT_SIZE = 75;//percentage
    const int COST_FONT_SIZE = 75;//percentage
    const int SHORTCUT_FONT_SIZE = 75;//percentage
    const string AMMO_COST_COLOR = "#FFD800";
    const string HEALTH_COST_COLOR = "#4CBF00";
    const string SHORTCUT_COLOR = "#FF0000";
    const string STAT_COLOR = "#0094FF";
    #endregion
    #region public fields
    public GameObject ToolTipPanel;
    public TextMeshProUGUI Text;
    public CameraController Cameras;
    public GraphicRaycaster CanvasRaycaster;
    #endregion
    #region private fields
    private ToolTipContentController currentContent;
    private float hoverTimer = 0;
    private float clickTimer = 0;
    private RectTransform panelTransform;
    private Rect screenRect;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        panelTransform = (RectTransform)ToolTipPanel.transform;
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        var pointerPos = Input.mousePosition;
        ToolTipContentController toolTip = null;
        float deltaTime = Time.deltaTime;

        //click cooldown
        clickTimer += deltaTime;
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            clickTimer = 0;
            hoverTimer = 0;
            ToolTipPanel.SetActive(false);
        }
        if(clickTimer >= CLICK_COOLDOWN)
        {
            //graphic raycast
            PointerEventData pointer = new PointerEventData(null);
            pointer.position = pointerPos;
            List<RaycastResult> uiHits = new List<RaycastResult>();
            CanvasRaycaster.Raycast(pointer, uiHits);

            bool lastFound = false;

            foreach (var h in uiHits)
            {
                //look for tooltip data to display
                toolTip = h.gameObject.GetComponent<ToolTipContentController>();
                if (toolTip != null)
                {
                    if (toolTip == currentContent)
                    {
                        lastFound = true;

                    }
                    else
                    {
                        currentContent = toolTip;
                    }
                    break;
                }
            }

            //normal raycast for map objects
            if (!lastFound)
            {
                var hits = Physics.RaycastAll(Cameras.MainCamera.ScreenPointToRay(pointerPos));
                foreach (var h in hits)
                {
                    //look for tooltip data to display
                    toolTip = h.collider.gameObject.GetComponent<ToolTipContentController>();
                    if (toolTip != null)
                    {
                        if (toolTip == currentContent)
                        {
                            lastFound = true;

                        }
                        else
                        {
                            currentContent = toolTip;
                        }
                        break;
                    }
                }
            }

            //if found, advance hover time
            if (currentContent != null && lastFound)
            {
                hoverTimer += deltaTime;
                //if hover timer has completed,
                if (hoverTimer >= HOVER_TIME)
                {
                    hoverTimer = HOVER_TIME;
                    //show tooltip panel
                    //set tooltip text
                    //if not shown yet, or if the text updates, get text
                    if (!ToolTipPanel.activeInHierarchy || currentContent.Updates)
                    {
                        Text.text = ParseContent(currentContent);
                    }

                    //reveal the tooltip, set pivot and position
                    if (!ToolTipPanel.activeInHierarchy && !string.IsNullOrWhiteSpace(Text.text))
                    {
                        //set position
                        panelTransform.position = pointerPos;
                        //set active
                        ToolTipPanel.SetActive(true);
                        //set pivot
                        float xPivot = 0;
                        float yPivot = 0;
                        var screenCenter = screenRect.center;
                        if (pointerPos.x > screenCenter.x)
                        {
                            xPivot = 1;
                        }
                        if (pointerPos.y > screenCenter.y)
                        {
                            yPivot = 1;
                        }

                        var pivot = new Vector2(xPivot, yPivot);

                        panelTransform.pivot = pivot;
                    }
                }
            }
            else
            {
                //reset timer
                hoverTimer = 0;
                //hide the tooltip
                ToolTipPanel.SetActive(false);
            }
        }
    }
    #endregion
    #region public methods
    public string ParseContent(ToolTipContentController content)
    {
        string output = "";
        if (!string.IsNullOrWhiteSpace(content.Header))
        {
            output += string.Format("<size={1}%><b>{0}</b>\n", content.Header, HEADER_FONT_SIZE);
        }
        if (!string.IsNullOrWhiteSpace(content.Body))
        {
            output += string.Format("<size={1}%>{0}\n", content.Body, BODY_FONT_SIZE);
        }
        foreach (var s in content.Stats)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                output += string.Format("<size={1}%><color={2}><i>{0}</i></color>\n", s, STAT_FONT_SIZE, STAT_COLOR);
            }
        }
        if (!string.IsNullOrWhiteSpace(content.AmmoCost))
        {
            output += string.Format("<size={1}%><color={2}><i>{0}</i></color>\n", content.AmmoCost, COST_FONT_SIZE, AMMO_COST_COLOR);
        }
        if (!string.IsNullOrWhiteSpace(content.HealthCost))
        {
            output += string.Format("<size={1}%><color={2}><i>{0}</i></color>\n", content.HealthCost, COST_FONT_SIZE, HEALTH_COST_COLOR);
        }
        if(content.MainShortcut != KeyBindConfigModel.KeyBindId.None)
        {
            output += string.Format("\n<align=\"right\"><size={1}%><color={2}><i>Shortcut: {0}</i></color>", content.MainKeyBind.ToString(), SHORTCUT_FONT_SIZE, SHORTCUT_COLOR);
        }
        if (content.AltShortcut != KeyBindConfigModel.KeyBindId.None)
        {
            output += string.Format("<space=5em><size={2}%><color={3}><i>{0}: {1}</i></color>", 
                content.AltShortcutType, content.AltKeyBind.ToString(), SHORTCUT_FONT_SIZE, SHORTCUT_COLOR);
        }
        return output;
    }
    #endregion
    #region private methods

    #endregion
}
