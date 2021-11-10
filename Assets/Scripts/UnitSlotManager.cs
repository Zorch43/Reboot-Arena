using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSlotManager : MonoBehaviour
{
    #region constants
    const int MAX_SLOT_COUNT = 9;
    #endregion
    #region public fields
    public UnitSlotController UnitSlotTemplate;
    #endregion
    #region private fields

    #endregion
    #region properties
    public List<UnitSlotController> UnitSlots { get; private set; } = new List<UnitSlotController>();
    public int SlotCount { get; set; } = MAX_SLOT_COUNT;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        if (GameObjectiveController.BattleConfig.IsPlayerSpectator)
        {
            gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < SlotCount; i++)
            {
                var unitSlot = Instantiate(UnitSlotTemplate, transform);
                unitSlot.Manager = this;
                unitSlot.SlotNumberLabel.text = (i + 1).ToString();
                unitSlot.ToolTip.MainShortcut = KeyBindConfigSettings.KeyBinds.GetUnitSlotKeyBind(i + 1);
                UnitSlots.Add(unitSlot);
            }
        }
    }

    #endregion
    #region public methods
    public void SelectSlot(int slotNumber)
    {
        slotNumber = Mathf.Clamp(slotNumber, 1, UnitSlots.Count) - 1;
        UnitSlots[slotNumber].SelectUnitSlot();
    }
    #endregion
    #region private methods

    #endregion
}
