using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
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
    public MapController Map;
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
    public Sprite SupportMarker;
    public Sprite MoveMarker;
    public Sprite AttackMoveMarker;
    public Sprite ForceAttackMarker;
    public Sprite RallyPointMarker;
    #endregion
    #region private fields
    bool isSpectating;
    private BuildHologramController currentHologram;
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
        float deltaTime = Time.deltaTime;
        bool isRunning = deltaTime > 0;
        Vector3 mapPos = new Vector3();
        Vector2 mousePosition = Input.mousePosition;
        //hotKey commands
        //var hotKeyCommand = GetKeyCommand();
        var heldKey = GetHeldKey();

        if (!isSpectating)
        {
            //if attack-move command is active, lmb gives the order, rmb cancels the order
            if (SelectedCommand == SpecialCommands.AttackMove)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (GetMouseMapPosition(mousePosition, out mapPos))
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
                    if (GetMouseMapPosition(mousePosition, out mapPos))
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
                    if (GetMouseMapPosition(mousePosition, out mapPos))
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
                    if (GetMouseMapPosition(mousePosition, out mapPos))
                    {
                        GiveSpecialAbilityOrder(mapPos);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    EndSpecialOrder();
                }
            }
            //if the command mode is normal, lmb selects, rmb peforms action
            else
            {
                //selection
                if (Input.GetMouseButtonDown(0))
                {
                    var ray = Cameras.MainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    UnitController selectedUnit = null;
                    if (GetRayHit(ray, out hit, true))
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
                //class menu mode - class menu is open
                if (SelectedCommand == SpecialCommands.ClassMenu)
                {
                    //close menu
                    if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.ClassMenuClose))
                    {
                        UnitActionUI.ClassMenu.ActionHideClassMenu();
                    }
                    else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.ClassSwitchFabricator))
                    {
                        UnitActionUI.ClassMenu.ActionSetClass(UnitClassTemplates.GetFabricatorClass());
                    }
                    else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.ClassSwitchTrooper))
                    {
                        UnitActionUI.ClassMenu.ActionSetClass(UnitClassTemplates.GetTrooperClass());
                    }
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.AttackMoveKey))
                {
                    if (GetMouseMapPosition(mousePosition, out mapPos))
                    {
                        GiveAttackMoveOrder(mapPos);
                    }
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.AttackMoveModeKey))
                {
                    UnitActionUI.ActionMoveAttack();
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.ForceAttackKey))
                {
                    if (GetMouseMapPosition(mousePosition, out mapPos))
                    {
                        GiveForceAttackOrder(mapPos);
                    }
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.ForceAttackModeKey))
                {
                    UnitActionUI.ActionForceAttack();
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.GameMenuKey))
                {
                    GameMenuUI.ShowMenu();
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.SetRallyPointKey))
                {
                    if (GetMouseMapPosition(mousePosition, out mapPos))
                    {
                        GiveRallyOrder(mapPos);
                    }
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.SetRallyPointModeKey))
                {
                    UnitActionUI.ActionSetRallyPoint();
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.StopActionKey))
                {
                    GiveStopOrder();
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.UnitSlot1Key))
                {
                    UnitSlotUI.SelectSlot(1);
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.UnitSlot2Key))
                {
                    UnitSlotUI.SelectSlot(2);
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.UnitSlot3Key))
                {
                    UnitSlotUI.SelectSlot(3);
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.UnitSlot4Key))
                {
                    UnitSlotUI.SelectSlot(4);
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.UnitSlot5Key))
                {
                    UnitSlotUI.SelectSlot(5);
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.UnitSlot6Key))
                {
                    UnitSlotUI.SelectSlot(6);
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.UnitSlot7Key))
                {
                    UnitSlotUI.SelectSlot(7);
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.UnitSlot8Key))
                {
                    UnitSlotUI.SelectSlot(8);
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.UnitSlot9Key))
                {
                    UnitSlotUI.SelectSlot(9);
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.AbilityGrenadeKey))
                {
                    UnitActionUI.ActivateUnitAbility("Grenade");
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.AbilityTurretKey))
                {
                    UnitActionUI.ActivateUnitAbility("Turret");
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.ClassMenuOpen))
                {
                    UnitActionUI.ClassMenu.ShowClassMenu();
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.ClassSwitchQuickFabricator))
                {
                    UnitActionUI.ClassMenu.ActionSetClass(UnitClassTemplates.GetFabricatorClass());
                }
                else if (IsKeyBindActive(heldKey, KeyBindConfigSettings.KeyBinds.ClassSwitchQuickTrooper))
                {
                    UnitActionUI.ClassMenu.ActionSetClass(UnitClassTemplates.GetTrooperClass());
                }
            }
        }
        //left-click on minimap will always pan the camera to the clicked location
        if (Cameras.IsPointInMiniMapBounds(Input.mousePosition) && Input.GetMouseButton(0))
        {
            if (GetMouseMapPosition(mousePosition, out mapPos))
            {
                Cameras.PanToMapLocation(mapPos);
            }
        }
        //general updates
        //update hologram if it is valid
        if (isRunning && UpdateHologramVisibility(mousePosition))
        {
            if (GetMouseMapPosition(mousePosition, out mapPos))
            {
                currentHologram.transform.position = mapPos;
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
            selectedUnits.Clear();
        }
        bool responseGiven = false;
        foreach (var u in units)
        {
            u.SpawnSlot.IsSelected = !GameObjectiveController.BattleConfig.IsAITeam(u.Data.Team) && (!addToSelection || !u.SpawnSlot.IsSelected);
            if(u.SpawnSlot.IsSelected)
            {
                selectedUnits.Add(u);
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
        if (GetRayHit(ray, out hit, true))
        {
            var mapPoint = hit.point;
            //action
            var selectedUnits = GetSelectedUnits();
            var unit = hit.collider.GetComponent<DroneController>();
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
                            //play support response, if it makes sense
                            u.UnitVoice.PlaySupportResponse();
                            //place support marker, if it makes sense
                            MarkerTemplate.Instantiate(SupportMarker, unit.transform, unit.transform.position, true);
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
    public void GiveUnitAttackOrder(UnitController selectedUnit, DroneController targetUnit)
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
            u.DoSpecialAbility(location, currentHologram);
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
            //if is a build ability, show the appropriate hologram
            if (ability.IsBuildAbility)
            {
                //load and show build hologram
                var hologramTemplate = Resources.Load<BuildHologramController>(ability.DroneHologram);
                currentHologram = Instantiate(hologramTemplate, Map.transform);
            }
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
    public void SetClassMenuMode()
    {
        SelectedCommand = SpecialCommands.ClassMenu;
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
    public bool IsKeyBindActive(KeyCode heldKey, KeyBindModel keyBind)
    {
        return heldKey == keyBind.HeldKey && Input.GetKeyDown(keyBind.PressedKey);
    }
    public KeyCode GetHeldKey()
    {
        foreach(var k in KeyBindConfigSettings.KeyBinds.ValidHeldKeys)
        {
            if (Input.GetKey(k))
            {
                return k;
            }
        }
        return KeyCode.None;
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
    public List<DroneController> GetAllUnits()
    {
        return Map.Units;
    }
    public void SetRespawnClass(UnitClassModel nextClass)
    {
        var slots = GetSelectedSlots();
        foreach(var s in slots)
        {
            s.Data.NextUnitClass = nextClass;
        }
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
                    var selectedUnit = u as UnitController;
                    if(selectedUnit != null)
                    {
                        selectedUnits.Add(selectedUnit);
                    }
                }
            }
            SelectUnits(selectedUnits, false);
        }
    }
    private bool GetRayHit(Ray ray, out RaycastHit hit, bool hitDrones)
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
        bool firstHit = false;
        foreach (var h in hits)
        {
            if (!h.collider.isTrigger)
            {
                if (h.collider.CompareTag("Drone") || h.collider.CompareTag("Unit"))
                {
                    if (hitDrones)
                    {
                        hit = h;
                        break;
                    }
                }
                else
                {
                    if (!firstHit)
                    {
                        hit = h;
                        if (!hitDrones)
                        {
                            break;
                        }
                    }
                }
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
        if(GetRayHit(ray, out hit, false))
        {
            mapPos = hit.point;
            return true;
        }
        return false;
    }
    private bool GetMouseMapPosition(Vector2 mousePosition, out Vector3 mapPos)
    {
        //auto-detect which camera to use (main or minimap) based on mouse position
        var fromCamera = Cameras.GetCommandCamera(mousePosition);

        return GetMapPositionFromScreen(mousePosition, out mapPos, fromCamera);
    }

    private bool UpdateHologramVisibility(Vector2 mousePosition)
    {
        bool isVisible = SelectedCommand == SpecialCommands.SpecialAbility && SpecialAbility?.IsBuildAbility == true;
        if (!isVisible)
        {
            if(currentHologram != null)
            {
                Destroy(currentHologram.gameObject);
            }
            
            currentHologram = null;//remove old hologram if no longer in use
        }
        else
        {
            isVisible = Cameras.IsPointInMainMapBounds(mousePosition);
            currentHologram.gameObject.SetActive(isVisible);//hide hologram when cursor is not in the main view
        }
        return isVisible;
    }
    #endregion
}
