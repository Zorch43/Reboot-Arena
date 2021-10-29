using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownColorController : MonoBehaviour
{
    #region public fields
    public Image ItemBackground;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //get this gameobject's name
        string gameObjectName = gameObject.name;
        //extract index
        char num = '0';
        foreach(var c in gameObjectName)
        {
            if (char.IsDigit(c))
            {
                num = c;
                break;
            }
        }
        gameObjectName = num.ToString();
        int index = int.Parse(gameObjectName);
        //input index into team color tool
        var itemColor = TeamTools.GetTeamColor(index);
        //color item background
        ItemBackground.color = itemColor;
    }
}
