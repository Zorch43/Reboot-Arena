using Assets.Scripts.Data_Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassMenuController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public TextButtonController ClassButtonTemplate;
    public GameObject BlankButtonTemplate;
    public TextButtonController CloseButton;
    public CommandController CommandInterface;
    #endregion
    #region private fields
    private List<UnitClassModel> classList;
    #endregion
    #region properties
    public List<TextButtonController> ClassButtons;
    #endregion
    #region unity methods
    private void Start()
    {
        CloseButton.Button.onClick.AddListener(ActionHideClassMenu);
    }
    #endregion
    #region actions
    public void ActionSetClass(UnitClassModel nextClass)
    {
        CommandInterface.SetRespawnClass(nextClass);
        ActionHideClassMenu();
    }
    public void ActionHideClassMenu()
    {
        gameObject.SetActive(false);
        CommandInterface.SelectedCommand = CommandController.SpecialCommands.Normal;
    }
    #endregion
    #region public methods
    public void Setup(List<UnitClassModel> classList)
    {
        ClassButtons = new List<TextButtonController>();
        this.classList = classList;
        //instantiate all class buttons
        for(int i = 0; i < 9; i++)
        {
            if(i < classList.Count)
            {
                var button = Instantiate(ClassButtonTemplate, transform);
                var unitClass = classList[i];
                button.Text.text = unitClass.Name;
                button.Button.image.sprite = Resources.Load<Sprite>(unitClass.Symbol);
                button.Button.onClick.AddListener(() =>
                {
                    ActionSetClass(unitClass);
                });
                ClassButtons.Add(button);
            }
            else
            {
                var button = Instantiate(BlankButtonTemplate, transform);
            }
        }
    }
    public void ShowClassMenu()
    {
        gameObject.SetActive(true);
        CommandInterface.SetClassMenuMode();
    }
    #endregion
}
