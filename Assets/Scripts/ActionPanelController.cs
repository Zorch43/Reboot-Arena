using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanelController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public CommandController CommandInterface;
    public Button MoveAttackButton;
    public Button ForceAttackButton;
    public Button StopActionButton;
    public Button SetRallyButton;
    public Button ChangeClassButton;
    public Button[] FreeButtons;
    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //connect actions to buttons
        MoveAttackButton.onClick.AddListener(ActionMoveAttack);
        ForceAttackButton.onClick.AddListener(ActionForceAttack);
        SetRallyButton.onClick.AddListener(ActionSetRallyPoint);
        StopActionButton.onClick.AddListener(ActionStop);
        ChangeClassButton.onClick.AddListener(ActionToggleClassMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region actions
    public void ActionMoveAttack()
    {
        CommandInterface.StartSpecialOrder(CommandController.SpecialCommands.AttackMove, ActionNormalize);
    }
    public void ActionForceAttack()
    {
        CommandInterface.StartSpecialOrder(CommandController.SpecialCommands.ForceAttack, ActionNormalize);
    }
    public void ActionSetRallyPoint()
    {
        CommandInterface.StartSpecialOrder(CommandController.SpecialCommands.SetRallyPoint, ActionNormalize);
    }
    public void ActionStop()
    {
        CommandInterface.GiveStopOrder();
    }
    public void ActionToggleClassMenu()
    {

    }
    public void ActionNormalize()
    {
        //TODO: reset the targeting state of all buttons
    }
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
