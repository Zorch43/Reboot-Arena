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
    public int AITeam { get; set; } = 1;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //selection
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            UnitController selectedUnit = null;
            if (GetRayHit(ray, out hit))
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
        bool responseGiven = false;
        foreach (var u in units)
        {
            u.Data.IsSelected = AITeam != u.Data.Team && (!addToSelection || !u.Data.IsSelected);
            if(u.Data.IsSelected && !responseGiven)
            {
                responseGiven = true;
                u.UnitVoice.PlaySelectionResponse();
            }
        }
    }
    public void GiveUnitOrder(UnitController selectedUnit, UnitController targetUnit)
    {
        selectedUnit.CommandTarget = targetUnit;
    }
    public void GiveUnitOrder(UnitController selectedUnit, Vector3 targetLocation)
    {
        selectedUnit.Agent.destination = targetLocation;
        selectedUnit.CommandTarget = null;
    }
    public void GiveOrder(Vector3 targetLocation, Camera fromCamera = null)
    {
        if(fromCamera == null)
        {
            fromCamera = Camera.main;
        }
        var ray = fromCamera.ScreenPointToRay(targetLocation);
        RaycastHit hit;
        if (GetRayHit(ray, out hit))
        {
            var mapPoint = hit.point;
            //action
            var allUnits = Map.GetComponentsInChildren<UnitController>();
            var unit = hit.collider.GetComponent<UnitController>();
            bool responseGiven = false;//only one response given per order.
            foreach (var u in allUnits)
            {
                if (u.Data.IsSelected)
                {
                    //u.Data.WayPoints = new List<Vector2>() { mapPoint };
                    //u.Data.IsMoving = true;
                    //u.SetDestination(mapPoint);
                    
                    if (unit != null)
                    {
                        GiveUnitOrder(u, unit);
                        if (!responseGiven)
                        {
                            responseGiven = true;
                            //TODO: move this logic to the unitcontroller
                            if(u.Data.Team != unit.Data.Team)
                            {
                                u.UnitVoice.PlayAttackResponse();
                            }
                            else
                            {
                                //TODO: play support response, if it makes sense
                            }
                        }
                    }
                    else
                    {
                        GiveUnitOrder(u, mapPoint);
                        if (!responseGiven)
                        {
                            responseGiven = true;
                            u.UnitVoice.PlayMoveResponse();
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region private methods
    void SelectUnitsInRect(Rect rect)
    {
        var allUnits = Map.GetComponentsInChildren<UnitController>();
        var selectedUnits = new List<UnitController>();
        if(rect.width >= 32 || rect.height >= 32)
        {
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
    }
    private bool GetRayHit(Ray ray, out RaycastHit hit)
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
        foreach (var h in hits)
        {
            if (h.collider.gameObject.GetComponent<UnitController>() != null)
            {
                hit = h;
                break;
            }
        }

        return hits.Length > 0;
    }
    #endregion
}
