using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSlotManager : MonoBehaviour
{
    #region constants
    public const int MAX_SLOT_COUNT = 6;
    #endregion
    #region public fields
    public CommandController CommandInterface;
    public UnitSlotController UnitSlotTemplate;
    #endregion
    #region private fields

    #endregion
    #region properties
    public List<UnitSlotController> UnitSlots { get; private set; } = new List<UnitSlotController>();
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        if (GameController.BattleConfig.IsPlayerSpectator)
        {
            gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < MAX_SLOT_COUNT; i++)
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
