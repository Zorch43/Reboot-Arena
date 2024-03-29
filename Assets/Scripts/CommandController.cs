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
    const float DOUBLE_TAP_TIME = 0.25f;
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
    private List<KeyBindModel> mainKeyBinds;
    private List<KeyBindModel> cameraKeyBinds;
    private List<KeyBindModel> classMenuKeyBinds;
    //double-tap detection
    private float clickTimer;
    private bool clicked;
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
        isSpectating = GameController.BattleConfig.IsPlayerSpectator;

        //match keybinds with actions
        var keyBinds = KeyBindConfigSettings.KeyBinds;
        keyBinds.AbilityGrenadeKey.BoundAction = ActionAbilityGrenade;
        keyBinds.AbilityTurretKey.BoundAction = ActionAbilityTurret;
        keyBinds.AbilityNanoPackKey.BoundAction = ActionAbilityNanoPack;
        keyBinds.AttackMoveKey.BoundAction = ActionAttackMove;
        keyBinds.AttackMoveModeKey.BoundAction = ActionAttackMoveMode;
        keyBinds.ClassMenuToggle.BoundAction = ActionToggleClassMenu;
        keyBinds.ClassSwitchFabricator.BoundAction = ActionSwitchClassFabricator;
        keyBinds.ClassSwitchQuickFabricator.BoundAction = ActionSwitchClassFabricator;
        keyBinds.ClassSwitchQuickTrooper.BoundAction = ActionSwitchClassTrooper;
        keyBinds.ClassSwitchTrooper.BoundAction = ActionSwitchClassTrooper;
        keyBinds.ClassSwitchQuickRanger.BoundAction = ActionSwitchClassRanger;
        keyBinds.ClassSwitchRanger.BoundAction = ActionSwitchClassRanger;
        keyBinds.ForceAttackKey.BoundAction = ActionForceAttack;
        keyBinds.ForceAttackModeKey.BoundAction = ActionForceAtttackMode;
        keyBinds.GameMenuKey.BoundAction = ActionGameMenu;
        keyBinds.SetRallyPointKey.BoundAction = ActionRally;
        keyBinds.SetRallyPointModeKey.BoundAction = ActionRallyMode;
        keyBinds.StopActionKey.BoundAction = ActionStop;
        keyBinds.UnitSlot1Key.BoundAction = ActionSlot1;
        keyBinds.UnitSlot2Key.BoundAction = ActionSlot2;
        keyBinds.UnitSlot3Key.BoundAction = ActionSlot3;
        keyBinds.UnitSlot4Key.BoundAction = ActionSlot4;
        keyBinds.UnitSlot5Key.BoundAction = ActionSlot5;
        keyBinds.UnitSlot6Key.BoundAction = ActionSlot6;
        keyBinds.UnitSlot7Key.BoundAction = ActionSlot7;
        keyBinds.UnitSlot8Key.BoundAction = ActionSlot8;
        keyBinds.UnitSlot9Key.BoundAction = ActionSlot9;
        //camera binds
        keyBinds.CameraPanDown.BoundAction = ActionPanCameraDown;
        keyBinds.CameraPanLeft.BoundAction = ActionPanCameraLeft;
        keyBinds.CameraPanRight.BoundAction = ActionPanCameraRight;
        keyBinds.CameraPanUp.BoundAction = ActionPanCameraUp;
        keyBinds.CameraRotateCCW.BoundAction = ActionCameraRotateCCW;
        keyBinds.CameraRotateCW.BoundAction = ActionCameraRotateCW;
        keyBinds.CameraTiltDown.BoundAction = ActionCameraTiltDown;
        keyBinds.CameraTiltUp.BoundAction = ActionCameraTiltUp;
        keyBinds.CameraZoomIn.BoundAction = ActionCameraZoomIn;
        keyBinds.CameraZoomOut.BoundAction = ActionCameraZoomOut;
        keyBinds.CameraReset.BoundAction = ActionCameraReset;

        keyBinds.SelectAll.BoundAction = ActionSelectAll;
        //organize binds into lists and sort them
        //populate the main list - available when not spectating
        mainKeyBinds = new List<KeyBindModel>()
        {
            keyBinds.AttackMoveKey,
            keyBinds.AttackMoveModeKey,
            keyBinds.ClassMenuToggle,
            keyBinds.ForceAttackKey,
            keyBinds.ForceAttackModeKey,
            keyBinds.SetRallyPointKey,
            keyBinds.SetRallyPointModeKey,
            keyBinds.StopActionKey,
            keyBinds.UnitSlot1Key,
            keyBinds.UnitSlot2Key,
            keyBinds.UnitSlot3Key,
            keyBinds.UnitSlot4Key,
            keyBinds.UnitSlot5Key,
            keyBinds.UnitSlot6Key,
            keyBinds.UnitSlot7Key,
            keyBinds.UnitSlot8Key,
            keyBinds.UnitSlot9Key,
            keyBinds.ClassSwitchQuickFabricator,
            keyBinds.ClassSwitchQuickTrooper,
            keyBinds.ClassSwitchQuickRanger,
            keyBinds.AbilityGrenadeKey,
            keyBinds.AbilityTurretKey,
            keyBinds.AbilityNanoPackKey,
            keyBinds.SelectAll
        };
        mainKeyBinds.Sort();
        //populate class menu list - available when class menu is open
        classMenuKeyBinds = new List<KeyBindModel>()
        {
            keyBinds.ClassMenuToggle,
            keyBinds.ClassSwitchFabricator,
            keyBinds.ClassSwitchTrooper,
            keyBinds.ClassSwitchRanger
        };
        classMenuKeyBinds.Sort();
        //populate camera controls - always available, can be activated simultaneously
        cameraKeyBinds = new List<KeyBindModel>()
        {
            keyBinds.GameMenuKey,
            keyBinds.CameraPanDown,
            keyBinds.CameraPanLeft,
            keyBinds.CameraPanUp,
            keyBinds.CameraPanRight,
            keyBinds.CameraRotateCCW,
            keyBinds.CameraRotateCW,
            keyBinds.CameraTiltDown,
            keyBinds.CameraTiltUp,
            keyBinds.CameraZoomIn,
            keyBinds.CameraZoomOut,
            keyBinds.CameraReset
        };
        cameraKeyBinds.Sort();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        bool isRunning = deltaTime > 0;
        Vector3 mapPos = new Vector3();
        Vector2 mousePosition = Input.mousePosition;

        var heldKey = GetHeldKey();
        bool command = false;
        //TODO: refactor to event-based structure.  This is getting dumb
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
                        command = true;
                        EventList.GetEvent(EventList.EventNames.OnInputAttackMoveSet).Invoke();
                    }

                }
                else if (Input.GetMouseButtonDown(1))
                {
                    EndSpecialOrder();
                    command = true;
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
                        command = true;
                        EventList.GetEvent(EventList.EventNames.OnInputForceAttackSet).Invoke();
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
                        command = true;
                        EventList.GetEvent(EventList.EventNames.OnInputRallySet).Invoke();
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    EndSpecialOrder();
                    command = true;
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
                        command = true;
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    EndSpecialOrder();
                    command = true;
                }
            }
            //class menu mode - class menu is open
            else if (SelectedCommand == SpecialCommands.ClassMenu)
            {
                command = DoBoundCommands(classMenuKeyBinds);
            }
            //if the command mode is normal, lmb selects, rmb peforms action
            else
            {
                //selection
                if (Input.GetMouseButtonDown(0))
                {
                    var ray = Cameras.MainCamera.ScreenPointToRay(mousePosition);
                    RaycastHit hit;
                    UnitController selectedUnit = null;
                    if (GetRayHit(ray, out hit, true))
                    {
                        selectedUnit = hit.transform.GetComponent<UnitController>();
                        if (selectedUnit != null)
                        {
                            
                            bool shift = Input.GetKey(KeyCode.LeftShift);
                            if(clicked && clickTimer < DOUBLE_TAP_TIME)
                            {
                                //select all units of the selected unit's type
                                SelectUnitsOfClassInView(selectedUnit.Data.UnitClass.ClassId);
                                //reset double-tap detection
                                clicked = false;
                                clickTimer = 0;
                            }
                            else
                            {
                                //invoke events
                                if (!GameController.BattleConfig.IsAITeam(selectedUnit.Data.Team))
                                {
                                    if (shift)
                                    {
                                        if (selectedUnit.SpawnSlot.IsSelected)
                                        {
                                            EventList.GetEvent(EventList.EventNames.OnInputSelectRemove).Invoke();
                                        }
                                        else
                                        {
                                            EventList.GetEvent(EventList.EventNames.OnInputSelectAdd).Invoke();
                                        }
                                    }
                                    else
                                    {
                                        EventList.GetEvent(EventList.EventNames.OnInputSelect).Invoke();
                                    }
                                }
                                clicked = true;
                                SelectUnits(new List<UnitController>() { selectedUnit }, shift);
                                
                            }
                        }
                    }
                    if (selectedUnit == null)
                    {
                        SelectionRect.StartSelection(mousePosition, SelectUnitsInRect);
                    }
                }
                //perform contextual action
                else if (Input.GetMouseButtonDown(1))
                {
                    GiveOrder(mousePosition);
                }
                
            }
            //normal commands
            if (!command)
            {
                DoBoundCommands(mainKeyBinds);
            }
        }
        //camera controls
        if (!command)
        {
            DoBoundCommands(cameraKeyBinds, false);
        }
        //left-click on minimap will always pan the camera to the clicked location
        if (Cameras.IsPointInMiniMapBounds(Input.mousePosition) && Input.GetMouseButton(0))
        {
            if (GetMouseMapPosition(mousePosition, out mapPos))
            {
                Cameras.PanToMapLocation(mapPos);
                //invoke events
                if (Input.GetMouseButtonDown(0))
                {
                    EventList.GetEvent(EventList.EventNames.OnInputMouseMinimapJump).Invoke();
                }
                else
                {
                    EventList.GetEvent(EventList.EventNames.OnInputMouseMinimapDrag).Invoke();
                }
            }
        }
        //update double-tap timer
        if (clicked)
        {
            if(clickTimer < DOUBLE_TAP_TIME)
            {
                clickTimer += deltaTime;
            }
            else
            {
                clickTimer = 0;
                clicked = false;
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
    public void SelectUnits(List<UnitController> units, bool addToSelection = false, bool playResponse = true)
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
            u.SpawnSlot.IsSelected = !GameController.BattleConfig.IsAITeam(u.Data.Team) && (!addToSelection || !u.SpawnSlot.IsSelected);
            if(u.SpawnSlot.IsSelected)
            {
                selectedUnits.Add(u);
                if (playResponse && !responseGiven)
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

        if(fromCamera != null)
        {
            var ray = fromCamera.ScreenPointToRay(targetLocation);
            RaycastHit hit;
            if (GetRayHit(ray, out hit, true))
            {
                var mapPoint = hit.point;
                //action
                var selectedUnits = GetSelectedUnits();
                if(selectedUnits.Count > 0)
                {
                    var unit = hit.collider.GetComponent<DroneController>();
                    if (unit != null)
                    {
                        if (unit.Data.Team != selectedUnits[0].Data.Team)
                        {
                            GiveAttackOrder(unit, selectedUnits);
                        }
                        else
                        {
                            GiveSupportOrder(unit, selectedUnits);
                        }
                    }
                    else
                    {
                        GiveUnitMoveOrder(mapPoint, selectedUnits);
                    }
                }
            }
        }
    }
    public void GiveAttackOrder(DroneController targetUnit, List<UnitController> selectedUnits = null)
    {
        selectedUnits = GetSelectedUnits(selectedUnits);
        DoGroupAction(selectedUnits, (UnitController selectedUnit) =>
        {
            selectedUnit.CancelOrders(true, false);
            selectedUnit.CommandTarget = targetUnit;
        }, (UnitController respondingUnit) =>
        {
            respondingUnit.UnitVoice.PlayAttackResponse();
            //place attack marker on enemy unit
            MarkerTemplate.Instantiate(AttackMarker, targetUnit.transform, targetUnit.transform.position, true);
            //invoke event
            EventList.GetEvent(EventList.EventNames.OnInputAttack).Invoke();
        });
        
    }
    public void GiveSupportOrder(DroneController targetUnit, List<UnitController> selectedUnits = null)
    {
        selectedUnits = GetSelectedUnits(selectedUnits);

        DoGroupAction(selectedUnits, (UnitController selectedUnit) =>
        {
            selectedUnit.CancelOrders(true, false);
            selectedUnit.CommandTarget = targetUnit;
        }, (UnitController respondingUnit) =>
        {
            //play support response, if it makes sense
            respondingUnit.UnitVoice.PlaySupportResponse();
            //place support marker, if it makes sense
            MarkerTemplate.Instantiate(SupportMarker, targetUnit.transform, targetUnit.transform.position, true);
            //invoke event
            EventList.GetEvent(EventList.EventNames.OnInputSupport).Invoke();
        });
    }
    public void GiveUnitMoveOrder(Vector3 targetLocation, List<UnitController> selectedUnits = null)
    {
        selectedUnits = GetSelectedUnits(selectedUnits);

        DoGroupAction(selectedUnits, (UnitController selectedUnit) =>
        {
            selectedUnit.DoMove(targetLocation, true);
            selectedUnit.CommandTarget = null;
        }, (UnitController respondingUnit) =>
        {
            respondingUnit.UnitVoice.PlayMoveResponse();
            //place move marker on position
            MarkerTemplate.Instantiate(MoveMarker, Map.transform, targetLocation, false);
            //invoke event
            EventList.GetEvent(EventList.EventNames.OnInputMove).Invoke();
        });
        
    }
    public void GiveAttackMoveOrder(Vector3 location, List<UnitController> selectedUnits = null)
    {
        selectedUnits = GetSelectedUnits(selectedUnits);

        DoGroupAction(selectedUnits, (UnitController selectedUnit) =>
        {
            selectedUnit.DoAttackMove(location);
        }, (UnitController respondingUnit) =>
        {
            //place attack-move marker
            MarkerTemplate.Instantiate(AttackMoveMarker, Map.transform, location, false);
            //TODO: give attack-move response
            respondingUnit.UnitVoice.PlayMoveResponse();
        });

        EndSpecialOrder();
    }
    public void GiveForceAttackOrder(Vector3 location, List<UnitController> selectedUnits = null)
    {
        selectedUnits = GetSelectedUnits(selectedUnits);

        DoGroupAction(selectedUnits, (UnitController selectedUnit) =>
        {
            selectedUnit.DoForceAttack(location);
        }, (UnitController respondingUnit) =>
        {
            //place attack-move marker
            MarkerTemplate.Instantiate(ForceAttackMarker, Map.transform, location, false);
            //TODO: give attack-move response
            respondingUnit.UnitVoice.PlayAttackResponse();
        });

        EndSpecialOrder();
    }
    public void GiveRallyOrder(Vector3 location, List<UnitController> selectedUnits = null)
    {
        selectedUnits = GetSelectedUnits(selectedUnits);
        DoGroupAction(selectedUnits, (UnitController selectedUnit) =>
        {
            selectedUnit.SpawnSlot.RallyPoint = location;
        }, (UnitController respondingUnit) =>
        {
            //place attack-move marker
            MarkerTemplate.Instantiate(RallyPointMarker, Map.transform, location, false);
            //TODO: give attack-move response
            respondingUnit.UnitVoice.PlaySelectionResponse();
        });
        
        EndSpecialOrder();
    }
    public void GiveStopOrder(List<UnitController> selectedUnits = null)
    {
        //get all selected units
        selectedUnits = GetSelectedUnits();
        //stop all movement, clear attack-move, force-attack, and command-attack targets
        foreach (var u in selectedUnits)
        {
            u.CancelOrders();
        }
        EndSpecialOrder();
    }
    public void GiveSpecialAbilityOrder(Vector3 location, List<UnitController> selectedUnits = null)
    {
        var abilityUnits = GetAbilityUnits(SpecialAbility, location);
        bool firstResponse = false;
        foreach(var u in abilityUnits)
        {
            //do special ability
            u.DoSpecialAbility(location, currentHologram);
            if (!firstResponse)
            {
                if (SpecialAbility.IsTargetedAbility)
                {
                    EventList.GetEvent(SpecialAbility.EventNameSet).Invoke();
                }
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
    public List<UnitController> GetSelectedUnits(List<UnitController> selectedUnits = null)
    {
        if(selectedUnits == null)
        {
            var selectedSlots = GetSelectedSlots();
            selectedUnits = new List<UnitController>();
            foreach (var s in selectedSlots)
            {
                if (s.Data.CurrentUnit != null)
                {
                    selectedUnits.Add(s.Data.CurrentUnit);
                }
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
    #region command actions
    private void ActionAttackMove()
    {
        Vector3 mapPos;
        if (GetMouseMapPosition(Input.mousePosition, out mapPos))
        {
            GiveAttackMoveOrder(mapPos);
        }
    }
    private void ActionAttackMoveMode()
    {
        UnitActionUI.ActionMoveAttack();
    }
    private void ActionForceAttack()
    {
        Vector3 mapPos;
        if (GetMouseMapPosition(Input.mousePosition, out mapPos))
        {
            GiveForceAttackOrder(mapPos);
        }
    }
    private void ActionForceAtttackMode()
    {
        UnitActionUI.ActionForceAttack();
    }
    private void ActionGameMenu()
    {
        GameMenuUI.ShowMenu();
    }
    private void ActionRally()
    {
        Vector3 mapPos;
        if (GetMouseMapPosition(Input.mousePosition, out mapPos))
        {
            GiveRallyOrder(mapPos);
        }
    }
    private void ActionRallyMode()
    {
        UnitActionUI.ActionSetRallyPoint();
    }
    private void ActionStop()
    {
        GiveStopOrder();
    }
    private void ActionSlot1()
    {
        UnitSlotUI.SelectSlot(1);
    }
    private void ActionSlot2()
    {
        UnitSlotUI.SelectSlot(2);
    }
    private void ActionSlot3()
    {
        UnitSlotUI.SelectSlot(3);
    }
    private void ActionSlot4()
    {
        UnitSlotUI.SelectSlot(4);
    }
    private void ActionSlot5()
    {
        UnitSlotUI.SelectSlot(5);
    }
    private void ActionSlot6()
    {
        UnitSlotUI.SelectSlot(6);
    }
    private void ActionSlot7()
    {
        UnitSlotUI.SelectSlot(7);
    }
    private void ActionSlot8()
    {
        UnitSlotUI.SelectSlot(8);
    }
    private void ActionSlot9()
    {
        UnitSlotUI.SelectSlot(9);
    }
    private void ActionAbilityGrenade()
    {
        UnitActionUI.ActivateUnitAbility("Grenade");
    }
    private void ActionAbilityTurret()
    {
        UnitActionUI.ActivateUnitAbility("Turret");
    }
    private void ActionAbilityNanoPack()
    {
        UnitActionUI.ActivateUnitAbility("NanoPack");
    }
    private void ActionToggleClassMenu()
    {
        if(SelectedCommand == SpecialCommands.ClassMenu)
        {
            UnitActionUI.ClassMenu.ActionHideClassMenu();
        }
        else
        {
            UnitActionUI.ClassMenu.ShowClassMenu();
        }
    }
    
    private void ActionSwitchClassFabricator()
    {
        UnitActionUI.ClassMenu.ActionSetClass(UnitClassTemplates.GetFabricatorClass());
    }
    private void ActionSwitchClassTrooper()
    {
        UnitActionUI.ClassMenu.ActionSetClass(UnitClassTemplates.GetTrooperClass());
    }
    private void ActionSwitchClassRanger()
    {
        UnitActionUI.ClassMenu.ActionSetClass(UnitClassTemplates.GetRangerClass());
    }
    private void ActionPanCameraDown()
    {
        Cameras.PanVector += new Vector2(0, -1);
    }
    private void ActionPanCameraUp()
    {
        Cameras.PanVector += new Vector2(0, 1);
    }
    private void ActionPanCameraLeft()
    {
        Cameras.PanVector += new Vector2(-1, 0);
    }
    private void ActionPanCameraRight()
    {
        Cameras.PanVector += new Vector2(1, 0);
    }
    private void ActionCameraRotateCCW()
    {
        Cameras.TiltVector += new Vector2(1, 0);
    }
    private void ActionCameraRotateCW()
    {
        Cameras.TiltVector += new Vector2(-1, 0);
    }
    private void ActionCameraTiltUp()
    {
        Cameras.TiltVector += new Vector2(0, 1);
    }
    private void ActionCameraTiltDown()
    {
        Cameras.TiltVector += new Vector2(0, -1);
    }
    private void ActionCameraZoomIn()
    {
        Cameras.ZoomDelta += 1;
    }
    private void ActionCameraZoomOut()
    {
        Cameras.ZoomDelta -= 1;
    }
    private void ActionCameraReset()
    {
        Cameras.ResetOrientation();
    }
    private void ActionSelectAll()
    {
        SelectUnits(GetUnitsInRect(Cameras.MainViewRect), false);
    }
    #endregion
    #region private methods
    private void SelectUnitsInRect(Rect rect)
    {
        if(rect.width >= 32 || rect.height >= 32)
        {
            var selectedUnits = GetUnitsInRect(rect);
            SelectUnits(selectedUnits, false);
            foreach(var u in selectedUnits)
            {
                if (!GameController.BattleConfig.IsAITeam(u.Data.Team))
                {
                    EventList.GetEvent(EventList.EventNames.OnInputSelectAreaFinish).Invoke();
                }
            }
        }
    }
    private List<UnitController> GetUnitsInRect(Rect rect)
    {
        var selectedUnits = new List<UnitController>();
        var allUnits = GetAllUnits();
        foreach (var u in allUnits)
        {
            var unitPoint = Cameras.MainCamera.WorldToScreenPoint(u.transform.position);
            if (rect.Contains(unitPoint))
            {
                var selectedUnit = u as UnitController;
                if (selectedUnit != null)
                {
                    selectedUnits.Add(selectedUnit);
                }
            }
        }
        return selectedUnits;
    }
    private void SelectUnitsOfClassInView(UnitClassTemplates.UnitClasses unitClass)
    {
        var allUnits = GetUnitsInRect(Cameras.MainViewRect);
        var classUnits = new List<UnitController>();
        foreach(var u in allUnits)
        {
            if(u.Data.UnitClass.ClassId == unitClass)
            {
                classUnits.Add(u);
            }
        }
        SelectUnits(classUnits, false, false);
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
    private bool DoBoundCommands(List<KeyBindModel> list, bool isExclusive = true)
    {
        bool found = false;
        foreach(var kb in list)
        {
            bool pressed = kb.TryInvoke();
            if (pressed)
            {
                found = true;
                if (isExclusive)
                {
                    break;
                }
            }
        }
        return found;
    }
    private void DoGroupAction(List<UnitController> group, Action<UnitController> each, Action<UnitController> all)
    {

        int leaderIndex = UnityEngine.Random.Range(0, group.Count);
        for(int i = 0; i < group.Count; i++)
        {
            each.Invoke(group[i]);
            if(i == leaderIndex)
            {
                all.Invoke(group[i]);
            }
        }
    }
    #endregion
}
