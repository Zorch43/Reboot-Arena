using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public SelectionRectController SelectionRect;
    public GameObject Map;
    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //single selection
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            UnitController selectedUnit = null;
            if (Physics.Raycast(ray, out hit))
            {
                selectedUnit = hit.transform.GetComponent<UnitController>();
                if (selectedUnit != null)
                {
                    bool shift = Input.GetKey(KeyCode.LeftShift);

                    SelectUnits(new List<UnitController>() { selectedUnit }, shift);
                }
            }
            if(selectedUnit == null)
            {
                SelectionRect.StartSelection(Input.mousePosition, SelectUnitsInRect);
            }
            

            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit))
            //{
            //    var selection = hit.transform.GetComponent<UnitController>();
            //    if (selection != null)
            //    {
            //        bool shift = Input.GetKey(KeyCode.LeftShift);

            //        SelectUnits(new List<UnitController>() { selection }, shift);
            //    }
            //}
        }
        //perform contextual action
        else if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var mapPoint = hit.point;
                //TODO: action
                var allUnits = Map.GetComponentsInChildren<UnitController>();
                foreach (var u in allUnits)
                {
                    if (u.Data.IsSelected)
                    {
                        //u.Data.WayPoints = new List<Vector2>() { mapPoint };
                        //u.Data.IsMoving = true;
                        //u.SetDestination(mapPoint);
                        u.Agent.destination = mapPoint;
                    }

                }
            }
        }
    }
    #endregion
    #region public methods
    public void SelectUnits(List<UnitController> units, bool addToSelection = false)
    {
        var allUnits = Map.GetComponentsInChildren<UnitController>();
        if (!addToSelection)
        {
            foreach (var u in allUnits)
            {
                u.Data.IsSelected = false;
            }
        }
        foreach (var u in units)
        {
            u.Data.IsSelected = !addToSelection || !u.Data.IsSelected;
        }
    }
    #endregion
    #region private methods
    void SelectUnitsInRect(Rect rect)
    {
        var allUnits = Map.GetComponentsInChildren<UnitController>();
        var selectedUnits = new List<UnitController>();
        
        foreach (var u in allUnits)
        {
            var unitPoint = Camera.main.WorldToScreenPoint(u.transform.position);
            if (rect.Contains(unitPoint))
            {
                selectedUnits.Add(u);
            }
        }
        SelectUnits(selectedUnits, false);
    }
    #endregion
}
