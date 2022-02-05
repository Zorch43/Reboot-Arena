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
    public Animator Animations;

    public TextMeshProUGUI CurrentClassNameLabel;
    public Image CurrentClassPortrait;
    public Image CurrentClassSymbol;

    public TextMeshProUGUI NextClassNameLabel;
    public Image NextClassPortrait;
    public Image NextClassSymbol;
    #endregion
    #region private fields
    private Button button;
    private float maxWidth;
    private bool shouldUpdate;
    private int slotNumber;
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
        button.onClick.AddListener(ActionClickSlot);

        maxWidth = ((RectTransform)(RespawnFilter.transform)).rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        //update unit status
        //update next unit name, portrait , and symbol
        if (Data.IsDirty)
        {
            var nextClass = Data.NextUnitClass;
            NextClassNameLabel.text = nextClass.Name;
            NextClassPortrait.sprite = Resources.Load<Sprite>(nextClass.Portrait);
            NextClassSymbol.sprite = Resources.Load<Sprite>(nextClass.Symbol);
            Data.IsDirty = false;
        }
        //if the current unit is active (alive)
        if (Data.CurrentUnit != null)
        {
            var unitData = Data.CurrentUnit.Data;
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
            CurrentClassNameLabel.text = unitData.UnitClass.Name;
            CurrentClassPortrait.sprite = Data.CurrentUnit.Portrait;
            CurrentClassSymbol.sprite = Data.CurrentUnit.Symbol;
           
            //update tooltip
            ToolTip.Clear();
            ToolTip.Header = "Unit Slot " + SlotNumberLabel.text + ": " + CurrentClassNameLabel.text;
            ToolTip.Body = "Displays status of unit assigned to slot.  Select the slot to select the unit.";
        }
        //if the current unit is not active (respawning)
        else
        {
            //if no current unit avaialable, show respawn progress for next unit
            RespawnFilter.SetActive(true);
            ((RectTransform)(RespawnFilter.transform)).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
                maxWidth * Data.RespawnProgress, maxWidth * (1 - Data.RespawnProgress));
            //hide unit health and ammo
            HealthBar.gameObject.SetActive(false);
            AmmoBar.gameObject.SetActive(false);
            //update class name, symbol, and portrait with next unit (Pending spawn switching)
            CurrentClassNameLabel.text = NextClassNameLabel.text;
            CurrentClassPortrait.sprite = NextClassPortrait.sprite;
            CurrentClassSymbol.sprite = NextClassSymbol.sprite;
            //hide selection state
            SelectionIndicator.SetActive(false);

            //update tooltip
            ToolTip.Clear();
            ToolTip.Header = "Unit Slot " + SlotNumberLabel.text + ": " + CurrentClassNameLabel.text;
            ToolTip.Body = "Unit will respawn at spawn point shortly...";
            ToolTip.Stats = new string[]
            {
                string.Format("Respawning... {0}%", (int)Mathf.Min(Data.RespawnProgress * 100, 100))
            };
        }
        //update animation parameters
        Animations.SetBool("CanChange", Data.CanChangeClass());
        Animations.SetBool("ChangeClass", Data.ShouldChangeClass());

        //update selection state
        SelectionIndicator.SetActive(Data.IsSelected);
    }
    #endregion
    #region actions
    public void ActionClickSlot()
    {
        if(Data != null)
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
        if (Data.IsSelected && Data.CurrentUnit != null)
        {
            Data.CurrentUnit.UnitVoice.PlaySelectionResponse();
        }
    }
    #endregion
    #region private methods

    #endregion
}
