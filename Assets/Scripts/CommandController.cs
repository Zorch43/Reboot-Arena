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
    public Texture2D AttackMoveCursor;
    public Texture2D ForceAttackCursor;
    public Texture2D SetRallyPointCursor;
    public ActionMarker MarkerTemplate;
    public Sprite AttackMarker;
    public Sprite MoveMarker;
    public Sprite AttackMoveMarker;
    public Sprite ForceAttackMarker;
    public Sprite RallyPointMarker;
    #endregion
    #region private fields

    #endregion
    #region properties
    public int AITeam { get; set; } = 1;
    public SpecialCommands SelectedCommand { get; set; }
    public UnitAbilityModel SpecialAbility { get; set; }
    public Action CommandCompletedCallback { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        KeyBindConfigSettings.LoadFromFile();
        MarkerTemplate.MainCamera = Camera.main;
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
                GiveAttackMoveOrder(GetSelectedUnits(), GetMouseMapPosition());
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
            else if(hotKeyCommand == KeyBindConfigSettings.KeyBinds.AbilityGrenadeKey)
            {
                UnitActionUI.ActivateUnitAbility("Grenade");
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
                    GiveAttackMoveOrder(GetSelectedUnits(), GetMouseMapPosition());
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
                    //activate special ability
                    GiveSpecialAbilityOrder(GetMouseMapPosition());
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
        List<UnitController> selectedUnits = new List<UnitController>();
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
                selectedUnits.Add(u);
            }
        }
        UnitActionUI.PopulateAbilityButtons(selectedUnits);
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
                            //place attack marker on enemy unit
                            MarkerTemplate.Instantiate(AttackMarker, unit.transform, unit.transform.position, true);
                        }
                        else
                        {
                            //TODO: play support response, if it makes sense
                            //TODO: place support marker, if it makes sense
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
                        //place move marker on position
                        MarkerTemplate.Instantiate(MoveMarker, Map.transform, mapPoint, false);
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
        selectedUnit.DoMove(targetLocation);
        selectedUnit.CommandTarget = null;
    }
    public void GiveAttackMoveOrder(List<UnitController> selectedUnits, Vector3 location)
    {
        bool firstResponse = false;
        //give each unit an attack-move command to the given location
        foreach(var u in selectedUnits)
        {
            u.DoAttackMove(location);
            if (!firstResponse)
            {
                firstResponse = true;
                //place attack-move marker
                MarkerTemplate.Instantiate(AttackMoveMarker, Map.transform, location, false);
                //TODO: give attack-move response
            }
        }
        EndSpecialOrder();
    }
    public void GiveForceAttackOrder(Vector3 location)
    {
        //get all selected units
        var selectedUnits = GetSelectedUnits();
        bool firstResponse = false;
        //give each unit a force-attack order to fire on the given location
        foreach (var u in selectedUnits)
        {
            u.DoForceAttack(location);
            if (!firstResponse)
            {
                firstResponse = true;
                //place force-attack marker
                MarkerTemplate.Instantiate(ForceAttackMarker, Map.transform, location, false);
                //TODO: give force-attack response
            }
        }
        EndSpecialOrder();
    }
    public void GiveRallyOrder(Vector3 location)
    {
        //get all selected units
        var selectedUnits = GetSelectedUnits();
        bool firstResponse = false;
        //set each selected unit's rally point to the given location
        foreach (var u in selectedUnits)
        {
            u.SetRallypoint(location);
            if (!firstResponse)
            {
                firstResponse = true;
                //place rally point marker
                MarkerTemplate.Instantiate(RallyPointMarker, Map.transform, location, false);
                //TODO: give rally point response
            }
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
    public void GiveSpecialAbilityOrder(Vector3 location)
    {
        var abilityUnits = GetAbilityUnits(SpecialAbility, location);
        bool firstResponse = false;
        foreach(var u in abilityUnits)
        {
            //do special ability
            u.DoSpecialAbility(location);
            if (!firstResponse)
            {
                firstResponse = true;
                MarkerTemplate.Instantiate(Resources.Load<Sprite>(SpecialAbility.Marker), Map.transform, location, false);
                //give special ability response
                u.UnitVoice.PlayAbilityResponse();
            }
        }
        EndSpecialOrder();
    }
    public List<UnitController> GetAbilityUnits(UnitAbilityModel ability, Vector3 target)
    {
        var selectedUnits = GetSelectedUnits();
        List<UnitController> validUnits = new List<UnitController>();
        List<UnitController> bestUnits = new List<UnitController>();
        float bestScore = 0;

        //get valid units
        foreach(var u in selectedUnits)
        {
            var unitAbility = u.Data.UnitClass.SpecialAbility;
            //unit must have the ability
            //unit must be able to pay for the ability
            if (unitAbility.Name == ability.Name && u.Data.MP >= ability.AmmoCostInstant)
            {
                validUnits.Add(u);
            }
        }
        //activate ability for valid units
        foreach(var u in validUnits)
        {
            var unitAbility = u.Data.UnitClass.SpecialAbility;

            //if the ability is group activation, all units do the ability
            //if there is only one valid unit selected, skip evaluation
            if (unitAbility.GroupActivationRule == UnitAbilityModel.GroupActivationType.All || validUnits.Count == 1)
            {
                bestUnits.Add(u);
            }
            //if the ability is single activation, find the best unit to activate the ability
            else if (unitAbility.GroupActivationRule == UnitAbilityModel.GroupActivationType.Single)
            {
                float score = ScoreUnitForAbility(u, target);
                //update best score and best unit
                if (score > bestScore || bestUnits.Count == 0)
                {
                    bestUnits = new List<UnitController>() { u };
                }
            }
        }

        return bestUnits;
    }
    public float ScoreUnitForAbility(UnitController unit, Vector3 target)
    {
        float score = 0;

        var specialAbility = unit.Data.UnitClass.SpecialAbility;
        //if unit already has an ability target, heavily de-prioritize it
        if(unit.AbilityTarget != null)
        {
            score -= 100;
        }

        //TODO: consider weighting these values
        if (specialAbility.ConsiderLeastAmmoInGroup)
        {
            score += 1 - (unit.Data.MP - specialAbility.AmmoCostInstant) / (unit.Data.UnitClass.MaxMP - specialAbility.AmmoCostInstant);
        }
        if (specialAbility.ConsiderLeastHealthInGroup)
        {
            score += 1 - unit.Data.HP / unit.Data.UnitClass.MaxHP;
        }
        if (specialAbility.ConsiderLeastMoveDistanceToTarget)
        {
            score += -(Mathf.Max((target - unit.transform.position).magnitude - specialAbility.AbilityWeapon.MaxRange, 0)/unit.Data.UnitClass.MoveSpeed);
        }
        if (specialAbility.ConsiderLeastTotalDistanceToTarget)
        {
            score += -((target - unit.transform.position).magnitude - specialAbility.AbilityWeapon.MaxRange)/ specialAbility.AbilityWeapon.MaxRange;
        }
        if (specialAbility.ConsiderMostAmmoInGroup)
        {
            score += unit.Data.MP / unit.Data.UnitClass.MaxMP;
        }
        if (specialAbility.ConsiderMostHealthInGroup)
        {
            score += unit.Data.HP / unit.Data.UnitClass.MaxHP;
        }

        return score;
    }
    public void StartSpecialOrder(SpecialCommands command, Action onComplete)
    {
        SelectedCommand = command;
        CommandCompletedCallback = onComplete;
        SetCursor(SelectedCommand);
    }
    public void StartSpecialOrder(UnitAbilityModel ability, Action onComplete)
    {
        SpecialAbility = ability;
        if (ability.IsTargetedAbility)
        {
            SelectedCommand = SpecialCommands.SpecialAbility;
            CommandCompletedCallback = onComplete;
            //set cursor
            SetCursor(SelectedCommand);
        }
        else
        {
            //non-target ability
            GiveSpecialAbilityOrder(new Vector3());//no targeting necessary
        }

    }
    public void EndSpecialOrder()
    {
        SelectedCommand = SpecialCommands.Normal;
        CommandCompletedCallback?.Invoke();
        CommandCompletedCallback = null;
        SetCursor(SelectedCommand);
    }
    public void SetCursor(SpecialCommands mode)
    {
        Texture2D cursorTexture = null;
        switch (mode)
        {
            case SpecialCommands.AttackMove:
                cursorTexture = AttackMoveCursor;
                break;
            case SpecialCommands.ForceAttack:
                cursorTexture = ForceAttackCursor;
                break;
            case SpecialCommands.SetRallyPoint:
                cursorTexture = SetRallyPointCursor;
                break;
            case SpecialCommands.SpecialAbility:
                cursorTexture = Resources.Load<Texture2D>(SpecialAbility.Cursor);
                break;
        }
        if(cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, new Vector2(16, 16), CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(null, new Vector2(), CursorMode.Auto);
        }
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
