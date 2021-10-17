using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    #region constants
    public enum SpecialCommands
    {
        Normal,
        AttackMove,
        ForceAttack,
        SetRallyPoint,
        ClassMenu,
        SpecialAbility
    }
    #endregion
    #region public fields
    public SelectionRectController SelectionRect;
    public GameObject Map;
    public UnitSlotManager UnitSlotUI;
    public ActionPanelController UnitActionUI;
    public GameMenuController GameMenuUI;
    #endregion
    #region private fields

    #endregion
    #region properties
    public int AITeam { get; set; } = 1;
    public SpecialCommands SelectedCommand { get; set; }
    public Action CommandCompletedCallback { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        KeyBindConfigSettings.LoadFromFile();
    }

    // Update is called once per frame
    void Update()
    {
        var hotKeyCommand = GetKeyCommand();
        //hotKey commands

        if(hotKeyCommand != null && hotKeyCommand.PressedKey != KeyCode.None)
        {
            if(hotKeyCommand == KeyBindConfigSettings.KeyBinds.AttackMoveKey)
            {
                GiveAttackMoveOrder(GetMouseMapPosition());
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.AttackMoveModeKey)
            {
                UnitActionUI.ActionMoveAttack();
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.ForceAttackKey)
            {
                GiveForceAttackOrder(GetMouseMapPosition());
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.ForceAttackModeKey)
            {
                UnitActionUI.ActionForceAttack();
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.GameMenuKey)
            {
                GameMenuUI.ShowMenu();
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.SetRallyPointKey)
            {
                GiveRallyOrder(GetMouseMapPosition());
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.SetRallyPointModeKey)
            {
                UnitActionUI.ActionSetRallyPoint();
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.StopActionKey)
            {
                GiveStopOrder();
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.UnitSlot1Key)
            {
                UnitSlotUI.SelectSlot(1); 
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.UnitSlot2Key)
            {
                UnitSlotUI.SelectSlot(2);
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.UnitSlot3Key)
            {
                UnitSlotUI.SelectSlot(3);
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.UnitSlot4Key)
            {
                UnitSlotUI.SelectSlot(4);
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.UnitSlot5Key)
            {
                UnitSlotUI.SelectSlot(5);
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.UnitSlot6Key)
            {
                UnitSlotUI.SelectSlot(6);
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.UnitSlot7Key)
            {
                UnitSlotUI.SelectSlot(7);
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.UnitSlot8Key)
            {
                UnitSlotUI.SelectSlot(8);
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.UnitSlot9Key)
            {
                UnitSlotUI.SelectSlot(9);
            }
        }
        else
        {
            //if the command mode is normal, lmb selects, rmb peforms action
            if (SelectedCommand == SpecialCommands.Normal || SelectedCommand == SpecialCommands.ClassMenu)
            {
                //selection
                if (Input.GetMouseButtonDown(0))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    UnitController selectedUnit = null;
                    if (GetRayHit(ray, out hit))
                    {
                        selectedUnit = hit.transform.GetComponent<UnitController>();
                        if (selectedUnit != null)
                        {
                            bool shift = Input.GetKey(KeyCode.LeftShift);

                            SelectUnits(new List<UnitController>() { selectedUnit }, shift);
                        }
                    }
                    if (selectedUnit == null)
                    {
                        SelectionRect.StartSelection(Input.mousePosition, SelectUnitsInRect);
                    }
                }
                //perform contextual action
                else if (Input.GetMouseButtonDown(1))
                {
                    GiveOrder(Input.mousePosition);
                }
            }
            //if attack-move command is active, lmb gives the order, rmb cancels the order
            else if (SelectedCommand == SpecialCommands.AttackMove)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GiveAttackMoveOrder(GetMouseMapPosition());
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    EndSpecialOrder();
                }
            }
            //if force-attack command is active, lmb gives the order, rmb cancels the order
            else if (SelectedCommand == SpecialCommands.ForceAttack)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GiveForceAttackOrder(GetMouseMapPosition());
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    EndSpecialOrder();
                }
            }
            //if rally point command is active, lmb sets the rally point, rmb cancels order
            else if (SelectedCommand == SpecialCommands.SetRallyPoint)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GiveRallyOrder(GetMouseMapPosition());
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    EndSpecialOrder();
                }
            }
            //if special ability command is active, and needs a target, lmb sets the target, rmb cancels the order
            else if (SelectedCommand == SpecialCommands.SpecialAbility)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //TODO: activate special ability
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    EndSpecialOrder();
                }
            }
        }
    }
    #endregion
    #region public methods
    public void SelectUnits(List<UnitController> units, bool addToSelection = false)
    {
        var allUnits = GetAllUnits();
        if (!addToSelection)
        {
            foreach (var u in allUnits)
            {
                u.Data.IsSelected = false;
            }
        }
        bool responseGiven = false;
        foreach (var u in units)
        {
            u.Data.IsSelected = AITeam != u.Data.Team && (!addToSelection || !u.Data.IsSelected);
            if(u.Data.IsSelected)
            {
                if (!responseGiven)
                {
                    responseGiven = true;
                    u.UnitVoice.PlaySelectionResponse();
                }
            }
        }
    }
    public void GiveOrder(Vector3 targetLocation, Camera fromCamera = null)
    {
        if(fromCamera == null)
        {
            fromCamera = Camera.main;
        }
        var ray = fromCamera.ScreenPointToRay(targetLocation);
        RaycastHit hit;
        if (GetRayHit(ray, out hit))
        {
            var mapPoint = hit.point;
            //action
            var selectedUnits = GetSelectedUnits();
            var unit = hit.collider.GetComponent<UnitController>();
            bool responseGiven = false;//only one response given per order.
            foreach (var u in selectedUnits)
            {
                if (unit != null)
                {
                    GiveUnitAttackOrder(u, unit);
                    if (!responseGiven)
                    {
                        responseGiven = true;
                        //TODO: move this logic to the unitcontroller
                        if (u.Data.Team != unit.Data.Team)
                        {
                            u.UnitVoice.PlayAttackResponse();
                        }
                        else
                        {
                            //TODO: play support response, if it makes sense
                        }
                    }
                }
                else
                {
                    GiveUnitMoveOrder(u, mapPoint);
                    if (!responseGiven)
                    {
                        responseGiven = true;
                        u.UnitVoice.PlayMoveResponse();
                    }
                }
            }
        }
    }
    public void GiveUnitAttackOrder(UnitController selectedUnit, UnitController targetUnit)
    {
        selectedUnit.CancelOrders();
        selectedUnit.CommandTarget = targetUnit;
    }
    public void GiveUnitMoveOrder(UnitController selectedUnit, Vector3 targetLocation)
    {
        selectedUnit.CancelOrders();
        selectedUnit.Agent.destination = targetLocation;
        selectedUnit.CommandTarget = null;
    }
    public void GiveAttackMoveOrder(Vector3 location)
    {
        //get all selected units
        var selectedUnits = GetSelectedUnits();
        //give each unit an attack-move command to the given location
        foreach(var u in selectedUnits)
        {
            u.DoAttackMove(location);
        }
        EndSpecialOrder();
    }
    public void GiveForceAttackOrder(Vector3 location)
    {
        //get all selected units
        var selectedUnits = GetSelectedUnits();
        //give each unit a force-attack order to fire on the given location
        foreach (var u in selectedUnits)
        {
            u.DoForceAttack(location);
        }
        EndSpecialOrder();
    }
    public void GiveRallyOrder(Vector3 location)
    {
        //get all selected units
        var selectedUnits = GetSelectedUnits();
        //set each selected unit's rally point to the given location
        foreach (var u in selectedUnits)
        {
            u.SetRallypoint(location);
        }
        EndSpecialOrder();
    }
    public void GiveStopOrder()
    {
        //get all selected units
        var selectedUnits = GetSelectedUnits();
        //stop all movement, clear attack-move, force-attack, and command-attack targets
        foreach (var u in selectedUnits)
        {
            u.CancelOrders();
        }
        EndSpecialOrder();
    }
    public void StartSpecialOrder(SpecialCommands command, Action onComplete)
    {
        SelectedCommand = command;
        CommandCompletedCallback = onComplete;
    }
    //TODO: add method to activate unit special ability
    public void EndSpecialOrder()
    {
        SelectedCommand = SpecialCommands.Normal;
        CommandCompletedCallback?.Invoke();
        CommandCompletedCallback = null;
    }
    public KeyBindModel GetKeyCommand()
    {
        foreach(var k in KeyBindConfigSettings.KeyBinds.AllKeyBinds)
        {
            if((k.HeldKey == KeyCode.None || Input.GetKey(k.HeldKey)) && Input.GetKeyDown(k.PressedKey))
            {
                return k;
            }
        }
        return null;
    }
    #endregion
    #region private methods
    private void SelectUnitsInRect(Rect rect)
    {
        var allUnits = GetAllUnits();
        var selectedUnits = new List<UnitController>();
        if(rect.width >= 32 || rect.height >= 32)
        {
            foreach (var u in allUnits)
            {
                var unitPoint = Camera.main.WorldToScreenPoint(u.transform.position);
                if (rect.Contains(unitPoint))
                {
                    selectedUnits.Add(u);
                }
            }
            SelectUnits(selectedUnits, false);
        }
    }
    private bool GetRayHit(Ray ray, out RaycastHit hit)
    {
        var hits = Physics.RaycastAll(ray);
        if(hits.Length > 0)
        {
            hit = hits[0];
        }
        else
        {
            hit = new RaycastHit();
        }
        foreach (var h in hits)
        {
            if (h.collider.gameObject.GetComponent<UnitController>() != null)
            {
                hit = h;
                break;
            }
        }

        return hits.Length > 0;
    }
    private Vector3 GetMapPositionFromScreen(Vector2 screenPos, Camera fromCamera = null)
    {
        if(fromCamera == null)
        {
            fromCamera = Camera.main;
        }

        var ray = fromCamera.ScreenPointToRay(screenPos);
        Vector3 mapPos = new Vector3();
        RaycastHit hit;
        if(GetRayHit(ray, out hit))
        {
            mapPos = hit.point;
        }
        return mapPos;
    }
    private Vector3 GetMouseMapPosition(Camera fromCamera = null)
    {
        //TODO: auto-detect which camera to use (main or minimap) based on mouse position
        return GetMapPositionFromScreen(Input.mousePosition, fromCamera);
    }
    private List<UnitController> GetSelectedUnits()
    {
        var allUnits = GetAllUnits();
        var selectedUnits = new List<UnitController>();
        foreach(var u in allUnits)
        {
            if (u.Data.IsSelected)
            {
                selectedUnits.Add(u);
            }
        }
        return selectedUnits;
    }
    private List<UnitController> GetAllUnits()
    {
        var allUnits = Map.GetComponentsInChildren<UnitController>();
        return new List<UnitController>(allUnits);
    }
    #endregion
}
