using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data_Models
{
    public class AIObjectiveModel
    {
        public enum Stance
        {
            Rush,//move to objective with mix of attackers and defenders
            Attack,//dislodge defenders from objective using attackers
            Defend,//hold objective from attackers using defenders
            Retreat//cede objective and regroup
        }
        public CapturePointController Objective { get; set; }
        public Stance TargetStance { get; set; }//what stance to take towards this objective
        public float EnemyAttackerWeight { get; set; }//how much enemy attack power is near the point
        public float EnemyDefenderWeight { get; set; }//how much enemy defense power is near the point
        public float EnemySupportWeight { get; set; }//how much enemy support power is near the point
        public float AttackerWeight { get; set; }//allied attack power near point
        public float DefenderWeight { get; set; }//allied defense power near point
        public float SupportWeight { get; set; }//allied support power near point
        public float Priority { get; set; }//how much this objective matters
        
    }
}
