using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public TextMeshProUGUI Title;
    public Button ResumeButton;
    public Button RestartButton;
    public Button ReturnButton;
    public Button QuitButton;
    public Button MenuButton;
    public ConfirmationMenuController ConfirmationMenu;
    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Awake()
    {
        ResumeButton.onClick.AddListener(ActionResume);
        RestartButton.onClick.AddListener(ActionRestart);
        ReturnButton.onClick.AddListener(ActionReturn);
        QuitButton.onClick.AddListener(ActionQuit);
    }
    #endregion
    #region actions
    public void ActionResume()
    {
        gameObject.SetActive(false);
        MenuButton.gameObject.SetActive(true);
        Time.timeScale = 1;
    }
    public void ActionRestart()
    {
        
        ConfirmationMenu.GetConfirmation("Are you sure you want to retart this game scenario?", () =>
        {
            var sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadSceneAsync(sceneName);
            Time.timeScale = 1;
        });
        
    }
    public void ActionReturn()
    {
        ConfirmationMenu.GetConfirmation("Are you sure you want to quit the game and return to the main menu?", () =>
        {
            SceneManager.LoadSceneAsync("MainMenu");
            Time.timeScale = 1;
        });
    }
    public void ActionQuit()
    {
        ConfirmationMenu.GetConfirmation("Are you sure you want to quit the game and return to the desktop?", () =>
        {
            Application.Quit();
        });
    }
    public void ActionMenu()
    {
        ShowMenu();
    }
    #endregion
    #region public methods
    public void ShowMenu(bool gameOver = false)
    {
        Title.gameObject.SetActive(!gameOver);
        ResumeButton.gameObject.SetActive(!gameOver);
        gameObject.SetActive(true);
        MenuButton.gameObject.SetActive(false);
        Time.timeScale = 0;
    }
    #endregion
    #region private methods

    #endregion
}
