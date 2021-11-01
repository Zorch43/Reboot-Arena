using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsViewController : MonoBehaviour
{
    public Button BackButton;
    public GameObject MainMenu;
    // Start is called before the first frame update
    void Start()
    {
        BackButton.onClick.AddListener(ActionBack);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActionBack()
    {
        gameObject.SetActive(false);
        MainMenu.SetActive(true);
    }
}
