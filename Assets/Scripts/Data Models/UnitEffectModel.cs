using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class UnitEffectModel
    {
        public float HealthDamage { get; set; }//damage or healing that effect does to target
        public float AmmoDamage { get; set; }//drain or restore that effect does to the target
        //condition(s) to apply to target
        public List<UnitConditionModel> Conditions { get; set; } = new List<UnitConditionModel>();

        public event EventHandler<UnitController> OnEffectApplied;
    }
}
