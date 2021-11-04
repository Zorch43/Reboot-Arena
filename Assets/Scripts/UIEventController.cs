using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEventController : MonoBehaviour, IPointerEnterHandler
{
    #region public fields
    public AudioSource SoundPlayer;
    public AudioClip ClickSound;
    public AudioClip HoverSound;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        var button = GetComponent<Button>();
        if(button != null)
        {
            button.onClick.AddListener(ActionClick);
        }
        
    }
    #endregion
    #region actions
    public void ActionHover()
    {
        SoundPlayer.clip = HoverSound;
        SoundPlayer.Play();
    }
    public void ActionClick()
    {
        SoundPlayer.clip = ClickSound;
        SoundPlayer.Play();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActionHover();
    }
    #endregion
}
