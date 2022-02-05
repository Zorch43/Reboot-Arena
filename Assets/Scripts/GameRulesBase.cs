using Assets.Scripts.Data_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GameRulesBase : MonoBehaviour
{
    public const float OCCUPATION_DISTANCE = 6;
    #region public fields
    public GameController Game;
    public CapturePointController[] ControlPoints;
    #endregion
    #region abstract methods
    public abstract void Setup();
    public abstract bool IsTeamVictorious(int team, float deltaTime);
    public abstract List<AIObjectiveModel> GetAIObjectives(int team, List<DroneController> allDrones);
    public abstract void SetObjectiveActive(bool objectiveActive);
    #endregion
}
