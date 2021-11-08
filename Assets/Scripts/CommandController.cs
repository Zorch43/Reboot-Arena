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
    public GameObject Map;
    public UnitSlotManager UnitSlotUI;
    public ActionPanelController UnitActionUI;
    public SelectionRectController SelectionRect;
    public GameMenuController GameMenuUI;
    public CameraController Cameras;
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
    bool isSpectating;
    #endregion
    #region properties
    public SpecialCommands SelectedCommand { get; set; }
    public UnitAbilityModel SpecialAbility { get; set; }
    public Action CommandCompletedCallback { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        KeyBindConfigSettings.LoadFromFile();
        MarkerTemplate.MainCamera = Cameras.MainCamera;
        isSpectating = GameObjectiveController.BattleConfig.IsPlayerSpectator;
    }

    // Update is called once per frame
    void Update()
    {

        var hotKeyCommand = GetKeyCommand();
        //hotKey commands
        Vector3 mapPos = new Vector3();

        if(!isSpectating && hotKeyCommand != null && hotKeyCommand.PressedKey != KeyCode.None)
        {
            if(hotKeyCommand == KeyBindConfigSettings.KeyBinds.AttackMoveKey)
            {
                if(GetMouseMapPosition(out mapPos))
                {
                    GiveAttackMoveOrder(mapPos);
                }
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.AttackMoveModeKey)
            {
                UnitActionUI.ActionMoveAttack();
            }
            else if (hotKeyCommand == KeyBindConfigSettings.KeyBinds.ForceAttackKey)
            {
                if (GetMouseMapPosition(out mapPos))
                {
                    GiveForceAttackOrder(mapPos);
                }   
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
                if (GetMouseMapPosition(out mapPos))
                {
                    GiveRallyOrder(mapPos);
                }
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
            //left-click on minimap will always pan the camera to the clicked location
            if(Cameras.IsPointInMiniMapBounds(Input.mousePosition) && Input.GetMouseButton(0))
            {
                if(GetMouseMapPosition(out mapPos))
                {
                    Cameras.PanToMapLocation(mapPos);
                }
            }
            else if (!isSpectating)
            {
                //if the command mode is normal, lmb selects, rmb peforms action
                if (SelectedCommand == SpecialCommands.Normal || SelectedCommand == SpecialCommands.ClassMenu)
                {
                    //selection
                    if (Input.GetMouseButtonDown(0))
                    {
                        var ray = Cameras.MainCamera.ScreenPointToRay(Input.mousePosition);
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
                        if (GetMouseMapPosition(out mapPos))
                        {
                            GiveAttackMoveOrder(mapPos);
                        }

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
                        if (GetMouseMapPosition(out mapPos))
                        {
                            GiveForceAttackOrder(mapPos);
                        }
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
                        if (GetMouseMapPosition(out mapPos))
                        {
                            GiveRallyOrder(mapPos);
                        }
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
                        if (GetMouseMapPosition(out mapPos))
                        {
                            GiveSpecialAbilityOrder(mapPos);
                        }
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        EndSpecialOrder();
                    }
                }
            }
            
        }
    }
    #endregion
    #region public methods
    public void SelectUnits(List<UnitController> units, bool addToSelection = false)
    {
        var selectedSlots = GetSelectedSlots();
        var selectedUnits = GetSelectedUnits();
        if (!addToSelection)
        {
            foreach(var s in selectedSlots)
            {
                s.Data.IsSelected = false;
            }
        }
        bool responseGiven = false;
        foreach (var u in units)
        {
            u.SpawnSlot.IsSelected = !GameObjectiveController.BattleConfig.IsAITeam(u.Data.Team) && (!addToSelection || !u.SpawnSlot.IsSelected);
            if(u.SpawnSlot.IsSelected)
            {
                if (!responseGiven)
                {
                    responseGiven = true;
                    u.UnitVoice.PlaySelectionResponse();
                }
            }
        }
        UnitActionUI.PopulateAbilityButtons(selectedUnits);
    }
    public void GiveOrder(Vector3 targetLocation)
    {
        var fromCamera = Cameras.GetCommandCamera(targetLocation);

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
        selectedUnit.DoMove(targetLocation, true);
        selectedUnit.CommandTarget = null;
    }
    public void GiveAttackMoveOrder(Vector3 location)
    {
        var selectedUnits = GetSelectedUnits();
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
        //get all selected slots
        var selectedUnits = GetSelectedSlots();
        bool firstResponse = false;
        //set each selected unit's rally point to the given location
        foreach (var u in selectedUnits)
        {
            u.Data.RallyPoint = location;
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
                    bestScore = score;
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
            score -= 1000;
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
    //get selected units from slots
    public List<UnitSlotController> GetSelectedSlots()
    {
        var allSlots = UnitSlotUI.UnitSlots;
        var selectedSlots = new List<UnitSlotController>();
        foreach (var u in allSlots)
        {
            //add to list if selected
            if (u.Data.IsSelected)
            {
                selectedSlots.Add(u);
            }
        }
        return selectedSlots;
    }
    public List<UnitController> GetSelectedUnits()
    {
        var selectedSlots = GetSelectedSlots();
        var selectedUnits = new List<UnitController>();
        foreach (var s in selectedSlots)
        {
            if (s.Data.CurrentUnit != null)
            {
                selectedUnits.Add(s.Data.CurrentUnit);
            }
        }
        return selectedUnits;
    }
    public List<UnitController> GetAllUnits()
    {
        var allUnits = new List<UnitController>();
        var allSlots = UnitSlotUI.UnitSlots;
        foreach (var s in allSlots)
        {
            if (s.Data.CurrentUnit != null)
            {
                allUnits.Add(s.Data.CurrentUnit);
            }
        }

        return allUnits;
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
                var unitPoint = Cameras.MainCamera.WorldToScreenPoint(u.transform.position);
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
    private bool GetMapPositionFromScreen(Vector2 screenPos, out Vector3 mapPos, Camera fromCamera = null)
    {
        mapPos = new Vector3();
        if (fromCamera == null)
        {
            return false;
        }

        var ray = fromCamera.ScreenPointToRay(screenPos);

        RaycastHit hit;
        if(GetRayHit(ray, out hit))
        {
            mapPos = hit.point;
            return true;
        }
        return false;
    }
    private bool GetMouseMapPosition(out Vector3 mapPos)
    {
        //auto-detect which camera to use (main or minimap) based on mouse position
        var fromCamera = Cameras.GetCommandCamera(Input.mousePosition);

        return GetMapPositionFromScreen(Input.mousePosition, out mapPos, fromCamera);
    }
    
    #endregion
}
