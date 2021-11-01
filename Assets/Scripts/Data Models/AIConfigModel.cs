using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class AIConfigModel
    {
        public string Name { get; set; }//human-readable ai name.  shows up in battleconfig
        public float Difficulty { get; set; }//base competancy at using tactical actions. higher=harder
        public float Speed { get; set; }//minimum delay between tactical options
        public float RetreatMod { get; set; }//difficulty modifier for giving a retreat order. higher=better
        public float CriticalHealMod { get; set; }//difficulty modidier for getting health in battle
        public float CriticalAmmoMod { get; set; }//mod for getting ammo in battle
        public float HealMod { get; set; }//mod for getting health after battle
        public float AmmoMod { get; set; }//mod for reloading after battle
        public float AttackMod { get; set; }//mod for doing a targeted attack
        public float SpecialMod { get; set; }//mod for activating special ability
    }
}
