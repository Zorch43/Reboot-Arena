using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitSlotConsole : MonoBehaviour
{
    #region constants
    private const float TRANSPARENCY_ENTER = .1f;
    private const float TRANSPARENCY_FULL = .85f;
    private const float TRANSPARENCY_EXIT = 0f;
    private const float TRANSITION_SPEED = 0.5f;
    private const float TRANSPARENCY_ENTER_DELTA = (TRANSPARENCY_FULL - TRANSPARENCY_ENTER)/TRANSITION_SPEED;
    private const float TRANSPARENCY_EXIT_DELTA = (TRANSPARENCY_FULL - TRANSPARENCY_EXIT)/TRANSITION_SPEED;
    #endregion
    #region public fields
    public TextMeshProUGUI StatusLineTemplate;
    #endregion
    #region private fields
    private List<TextMeshProUGUI> AdditionQueue = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> RemovalQueue = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> StatusLineList = new List<TextMeshProUGUI>();
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        
    }
    #endregion
    #region public methods
    public TextMeshProUGUI CreateStatusLine(string text)
    {
        var line = GameObject.Instantiate(StatusLineTemplate, transform);
        line.text = text;
        line.color = new Color(1, 1, 1, TRANSPARENCY_ENTER);
        StatusLineList.Add(line);
        AdditionQueue.Add(line);

        return line;
    }
    public void RemoveStatusLine(TextMeshProUGUI line)
    {
        RemovalQueue.Add(line);
    }
    public void UpdateConsole(float deltaTime)
    {
        UpdateAdditions(deltaTime);
        UpdateRemovals(deltaTime);
    }
    #endregion
    #region private methods
    private void UpdateAdditions(float deltaTime)
    {
        var tempList = new List<TextMeshProUGUI>(AdditionQueue);
        foreach (var item in tempList)
        {
            if (item.color.a >= TRANSPARENCY_FULL)
            {
                item.color = new Color(1, 1, 1, TRANSPARENCY_FULL);
                AdditionQueue.Remove(item);
            }
            else
            {
                var transparency = item.color.a + deltaTime * TRANSPARENCY_ENTER_DELTA;
                item.color = new Color(1, 1, 1, transparency);
            }
        }
    }
    private void UpdateRemovals(float deltaTime)
    {
        var tempList = new List<TextMeshProUGUI>(RemovalQueue);
        foreach (var item in tempList)
        {
            if (item?.color == null)
            {
                var test = "Test";
            }
            if (item.color.a <= TRANSPARENCY_EXIT)
            {
                item.color = new Color(1, 1, 1, TRANSPARENCY_EXIT);
                RemovalQueue.Remove(item);
                StatusLineList.Remove(item);
                Destroy(item.gameObject);
            }
            else
            {
                var transparency = item.color.a - deltaTime * TRANSPARENCY_EXIT_DELTA;
                item.color = new Color(1, 1, 1, transparency);
            }
        }
    }
    #endregion
}
