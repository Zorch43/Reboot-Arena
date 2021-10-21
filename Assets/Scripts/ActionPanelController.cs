using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
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
    public TeamController PlayerTeam;
    public Button MoveAttackButton;
    public Button ForceAttackButton;
    public Button StopActionButton;
    public Button SetRallyButton;
    public Button ChangeClassButton;
    public GameObject[] FreeButtons;
    public TextButtonController ButtonTemplate;
    #endregion
    #region private fields
    private List<TextButtonController> abilityButtons = new List<TextButtonController>();
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

        //add all possible unit ability buttons
        foreach(var u in PlayerTeam.UnitTemplates)
        {
            var abilityButton = Instantiate(ButtonTemplate, transform);
            var specialAbility = u.GetData().UnitClass.SpecialAbility;
            abilityButton.Button.image.sprite = Resources.Load<Sprite>(specialAbility.Icon);
            abilityButton.Button.onClick.AddListener(() =>
            {
                ActionUnitAbility(specialAbility);
            });
            abilityButton.Text.text = specialAbility.Name;
            abilityButton.gameObject.SetActive(false);
            abilityButtons.Add(abilityButton);
        }
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
    public void ActionUnitAbility(UnitAbilityModel ability)
    {
        //activate the special ability in the command interface
        CommandInterface.StartSpecialOrder(ability, ActionNormalize);
    }
    #endregion
    #region public methods
    public void PopulateAbilityButtons(List<UnitController> selectedUnits)
    {
        //hide all ability buttons
        foreach(var a in abilityButtons)
        {
            a.gameObject.SetActive(false);
        }
        //show avaialable ability buttons
        int abilityCount = 0;
        foreach(var u in selectedUnits)
        {
            var specialAbility = u.Data.UnitClass.SpecialAbility;
            foreach(var b in abilityButtons)
            {
                //if button for ability found
                if(b.Text.text == specialAbility.Name)
                {
                    //and the button is hidden
                    if (!b.gameObject.activeSelf)
                    {
                        //show the button
                        b.gameObject.SetActive(true);
                        //increment the abilityCount
                        abilityCount++;
                    }
                    break;
                }
            }
        }
        //show or hide placeholder buttons so that there are the same number of visible buttons
        for(int i = 0; i < FreeButtons.Length; i++)
        {
            FreeButtons[i].SetActive(i >= abilityCount);
        }
    }
    public void ActivateUnitAbility(string abilityName)
    {
        foreach(var b in abilityButtons)
        {
            if(b.Text.text == abilityName)
            {
                b.Button.onClick.Invoke();
                break;
            }
        }
    }
    #endregion
    #region private methods

    #endregion
}
