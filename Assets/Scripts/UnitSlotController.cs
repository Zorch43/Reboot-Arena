using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
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
    public UIStatusBarController HealthBar;
    public UIStatusBarController AmmoBar;
    public ToolTipContentController ToolTip;
    public TextMeshProUGUI ClassNameLabel;
    public Image ClassPortrait;
    public Button SelectionButton;
    public TextButtonController TargetedAbilityButton;
    public TextButtonController ActivatedAbilityButton;
    #endregion
    #region private fields

    private float maxWidth;
    private bool shouldUpdate;
    private int slotNumber;
    private float respawnProgress;
    #endregion
    #region properties
    public UnitSlotModel Data { get; set; } = new UnitSlotModel();
    public UnitSlotManager Manager { get; set; }
    //public UnitController Unit { get; set; }
    //public UnitController UnitTemplate { get; set; }
    //public int SlotNumber { get; set; }
    //public float RespawnProgress
    //{
    //    get
    //    {
    //        return respawnProgress;
    //    }
    //    set
    //    {
    //        respawnProgress = value;
    //        ((RectTransform)(RespawnFilter.transform)).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
    //            maxWidth * respawnProgress, maxWidth * (1 - respawnProgress));
    //    }
    //}
    //public Vector3? RallyPoint { get; set; }
    //public bool FolowRallyPoint { get; set; } = true;
    //public bool IsSelected
    //{
    //    get
    //    {
    //        return SelectionIndicator.activeSelf;
    //    }
    //    set
    //    {
    //        SelectionIndicator.SetActive(value);
    //    }
    //}
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        SelectionButton.onClick.AddListener(ActionClickSlot);

        maxWidth = ((RectTransform)(RespawnFilter.transform)).rect.width;

        Data.Controller = this;
    }

    // Update is called once per frame
    void Update()
    {
        //update unit status
        //if the current unit is active (alive)
        if (Data.Unit != null)
        {
            var unitData = Data.Unit.Data;
            //unit has been spawned
            //hide RespawnFilter
            RespawnFilter.SetActive(false);
            //show Health and ammo bars
            HealthBar.gameObject.SetActive(true);
            AmmoBar.gameObject.SetActive(true);
            //update health and ammo values
            HealthBar.UpdateBar(unitData.HP, unitData.UnitClass.MaxHP);
            AmmoBar.UpdateBar(unitData.MP, unitData.UnitClass.MaxMP);
            //update current unit name, portrait and symbol
            ClassNameLabel.text = unitData.UnitClass.Name;
            ClassPortrait.sprite = Data.Unit.Portrait;
           
            //update tooltip
            ToolTip.Clear();
            ToolTip.Header = "Unit Slot " + SlotNumberLabel.text + ": " + ClassNameLabel.text;
            ToolTip.Body = "Displays status of unit assigned to slot.  Select the slot to select the unit.";
        }
        //if the current unit is not active (respawning)
        else
        {
            //if no current unit avaialable, show respawn progress for next unit
            RespawnFilter.SetActive(true);
            //hide unit health and ammo
            HealthBar.gameObject.SetActive(false);
            AmmoBar.gameObject.SetActive(false);

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
    }
    #endregion
    #region actions
    public void ActionClickSlot()
    {
        switch (Data.SlotNumber)
        {
            case 1:
                EventList.GetEvent(EventList.EventNames.OnInputUISelectSlot1).Invoke();
                break;
            case 2:
                EventList.GetEvent(EventList.EventNames.OnInputUISelectSlot2).Invoke();
                break;
            case 3:
                EventList.GetEvent(EventList.EventNames.OnInputUISelectSlot3).Invoke();
                break;
            case 4:
                EventList.GetEvent(EventList.EventNames.OnInputUISelectSlot4).Invoke();
                break;
            case 5:
                EventList.GetEvent(EventList.EventNames.OnInputUISelectSlot5).Invoke();
                break;
            case 6:
                EventList.GetEvent(EventList.EventNames.OnInputUISelectSlot6).Invoke();
                break;
            case 7:
                EventList.GetEvent(EventList.EventNames.OnInputUISelectSlot7).Invoke();
                break;
            case 8:
                EventList.GetEvent(EventList.EventNames.OnInputUISelectSlot8).Invoke();
                break;
            case 9:
                EventList.GetEvent(EventList.EventNames.OnInputUISelectSlot9).Invoke();
                break;
            default:
                Debug.LogError("Error: invalid slot event #" + Data.SlotNumber + " called.");
                break;
        }
        SelectUnitSlot();
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
        if (Data.IsSelected && Data.Unit != null)
        {
            Data.Unit.UnitVoice.PlaySelectionResponse();
        }
    }
    public void UpdateRespawnProgress(float progress)
    {
        ((RectTransform)(RespawnFilter.transform)).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
            maxWidth * progress, maxWidth * (1 - progress));
    }
    #endregion
    #region private methods

    #endregion
}
