using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public GameObject UnitAppearance;
    public SpriteRenderer TeamColorRenderer;
    public GameObject UnitEffects;
    public SpriteRenderer Selector;
    public NavMeshAgent Agent;
    #endregion
    #region private fields
    private Quaternion initialRotation;
    #endregion
    #region properties
    public UnitModel Data { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //TEMP: initialize data model
        Data = new UnitModel(UnitClassTemplates.GetTrooperClass());
        initialRotation = UnitEffects.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.deltaTime;
        //update selection state
        Selector.gameObject.SetActive(Data.IsSelected);
        //manual turning
        if (Agent.hasPath)
        {
            //var wayPoint = Agent.nextPosition;

            //var vector = wayPoint - transform.position;

            ////if not also attacking, turn towards the next waypoint
            //float angle = Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg - 90;
            //Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            //UnitAppearance.transform.rotation = Quaternion.Slerp(UnitAppearance.transform.rotation, q, Time.deltaTime * Data.UnitClass.TurnSpeed);
            
            if (Data.IsAttacking)
            {
                //TODO: override facing
            }
            UnitEffects.transform.rotation = initialRotation;
        }
        ////move towards next waypoint
        //if (Data.IsMoving)
        //{
        //    var vector = Data.WayPoints[0] - (Vector2)transform.position;
        //    var moveVector = vector.normalized * elapsedTime * Data.UnitClass.MoveSpeed;

        //    if(moveVector.magnitude >= vector.magnitude)
        //    {
        //        transform.position = Data.WayPoints[0];
        //        Data.WayPoints.RemoveAt(0);
        //        if(Data.WayPoints.Count < 1)
        //        {
        //            Data.IsMoving = false;
        //        }
        //    }
        //    else
        //    {
        //        transform.position += (Vector3)moveVector;
        //    }
        //    //if not also attacking, turn towards the next waypoint
        //    if (!Data.IsAttacking)
        //    {
        //        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg - 90;
        //        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        //        UnitAppearance.transform.rotation = Quaternion.Slerp(UnitAppearance.transform.rotation, q, Time.deltaTime * Data.UnitClass.TurnSpeed);
        //    }
        //}
    }
    #endregion
    #region public methods

    #endregion
}
