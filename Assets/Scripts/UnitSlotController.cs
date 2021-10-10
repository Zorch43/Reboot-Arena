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
            //update selection state
            SelectionIndicator.SetActive(unitData.IsSelected);
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
        }
    }
    #endregion
    #region public methods
    public void SelectUnitSlot()
    {
        if(Data.CurrentUnit != null)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                foreach (var u in Manager.UnitSlots)
                {
                    if (u.Data.CurrentUnit != null)
                    {
                        u.Data.CurrentUnit.Data.IsSelected = false;
                    }
                }
                Data.CurrentUnit.Data.IsSelected = true;
            }
            else
            {
                Data.CurrentUnit.Data.IsSelected = !Data.CurrentUnit.Data.IsSelected;
            }
        }
    }
    #endregion
    #region private methods

    #endregion
}
