using Assets.Scripts.Data_Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSlotController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    //slot UI
    public GameObject RespawnFilter;
    public GameObject SelectionIndicator;
    public TextMeshProUGUI SlotNumberLabel;
    public TextMeshProUGUI ClassNameLabel;
    public Image PortraitImage;
    public Image ClassSymbol;
    public UIStatusBarController HealthBar;
    public UIStatusBarController AmmoBar;
    public ToolTipContentController ToolTip;
    #endregion
    #region private fields
    private Button button;
    private float maxWidth;
    #endregion
    #region properties
    public UnitSlotModel Data { get; set; } = new UnitSlotModel();
    public UnitSlotManager Manager { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(SelectUnitSlot);

        maxWidth = ((RectTransform)(RespawnFilter.transform)).rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        //update unit status
        var unitData = Data.CurrentUnit?.Data;
        if(unitData != null)
        {
            //unit has been spawned
            //hide RespawnFilter
            RespawnFilter.SetActive(false);
            //show Health and ammo bars
            HealthBar.gameObject.SetActive(true);
            AmmoBar.gameObject.SetActive(true);
            //update health and ammo values
            HealthBar.UpdateBar(unitData.HP, unitData.UnitClass.MaxHP);
            AmmoBar.UpdateBar(unitData.MP, unitData.UnitClass.MaxMP);
            //update unit name, portrait and symbol
            ClassNameLabel.text = unitData.UnitClass.Name;
            //TODO: update portrait and symbol
            PortraitImage.sprite = Data.CurrentUnit.Portrait;
            ClassSymbol.sprite = Data.CurrentUnit.Symbol;

            //update tooltip
            ToolTip.Clear();
            ToolTip.Header = "Unit Slot " + SlotNumberLabel.text + ": " + ClassNameLabel.text;
            ToolTip.Body = "Displays status of unit assigned to slot.  Select the slot to select the unit.";
        }
        else
        {
            //if no current unit avaialable, show respawn progress for next unit
            RespawnFilter.SetActive(true);
            ((RectTransform)(RespawnFilter.transform)).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
                maxWidth * Data.RespawnProgress, maxWidth * (1 - Data.RespawnProgress));
            //hide unit health and ammo
            HealthBar.gameObject.SetActive(false);
            AmmoBar.gameObject.SetActive(false);
            //TODO: update class name, symbol, and portrait with next unit (Pending spawn switching)
            //hide selection state
            SelectionIndicator.SetActive(false);

            //update tooltip
            ToolTip.Clear();
            ToolTip.Header = "Unit Slot " + SlotNumberLabel.text + ": " + ClassNameLabel.text;
            ToolTip.Body = "Unit will respawn at spawn point shortly...";
            ToolTip.Stats = new string[]
            {
                string.Format("Respawning... {0}%", (int)Mathf.Min(Data.RespawnProgress * 100, 100))
            };
        }
        //update selection state
        SelectionIndicator.SetActive(Data.IsSelected);
    }
    #endregion
    #region public methods
    public void SelectUnitSlot()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            foreach (var u in Manager.UnitSlots)
            {
                u.Data.IsSelected = false;
            }
            Data.IsSelected = true;
        }
        else
        {
            Data.IsSelected = !Data.IsSelected;
        }
        if (Data.IsSelected && Data.CurrentUnit != null)
        {
            Data.CurrentUnit.UnitVoice.PlaySelectionResponse();
        }
    }
    #endregion
    #region private methods

    #endregion
}
