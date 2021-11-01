using Assets.Scripts.Data_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Templates
{
    public static class AIConfigTemplates
    {
        private static List<AIConfigModel> _aiConfigs;

        public static List<AIConfigModel> GetAIConfigList()
        {
            if(_aiConfigs == null)
            {
                _aiConfigs = new List<AIConfigModel>()
                {
                    CreateRusher(),
                    CreatePotato(),
                    CreateToaster(),
                    CreateDrone(),
                    CreateSoldier(),
                    CreateCyborg(),
                    CreateKillbot(),
                    CreateMech(),
                    CreateReaper(),
                    CreateNanite()
                };
            }
            return _aiConfigs;
        }
        //easiest ai: only rushes and defends, slow tactical speed
        public static AIConfigModel CreateRusher()
        {
            var ai = new AIConfigModel()
            {
                Name = "Potato",
                Speed = 5,
                Difficulty = 0
            };
            return ai;
        }
        //very easy ai: will rush, but will retreat as a group
        public static AIConfigModel CreatePotato()
        {
            var ai = new AIConfigModel()
            {
                Name = "Toaster",
                Speed = 3,
                Difficulty = 0,
                RetreatMod = 1
            };
            return ai;
        }
        //very easy ai: can perform targeted attacks and do critical resupply
        public static AIConfigModel CreateToaster()
        {
            var ai = new AIConfigModel()
            {
                Name = "CPU",
                Speed = 3,
                Difficulty = 0,
                AttackMod = .5f,
                CriticalHealMod = .25f,
                CriticalAmmoMod = .25f,
                RetreatMod = .5f
            };
            return ai;
        }
        //easy ai: can do everything... poorly
        public static AIConfigModel CreateDrone()
        {
            var ai = new AIConfigModel()
            {
                Name = "Drone",
                Speed = 2,
                Difficulty = .2f,
                RetreatMod = 1
            };
            return ai;
        }
        //normal ai: can do a little of everything, prioritizes resupplying after battle
        public static AIConfigModel CreateSoldier()
        {
            var ai = new AIConfigModel()
            {
                Name = "Soldier",
                Speed = 2,
                Difficulty = .5f,
                RetreatMod = 1,
                AmmoMod = .25f,
                HealMod = .4f
            };
            return ai;
        }
        //normal ai: can do a little of everything, faster
        public static AIConfigModel CreateCyborg()
        {
            var ai = new AIConfigModel()
            {
                Name = "Cyborg",
                Speed = 1.5f,
                Difficulty = .5f,
            };
            return ai;
        }
        //hard ai: can do a most everything, fast
        public static AIConfigModel CreateKillbot()
        {
            var ai = new AIConfigModel()
            {
                Name = "Killbot",
                Speed = 1f,
                Difficulty = .75f,
                RetreatMod = .25f
            };
            return ai;
        }
        //hard ai: can do everything, slowly
        public static AIConfigModel CreateMech()
        {
            var ai = new AIConfigModel()
            {
                Name = "Mech",
                Speed = 2f,
                Difficulty = 1f
            };
            return ai;
        }
        //very hard ai: does everything fast
        public static AIConfigModel CreateReaper()
        {
            var ai = new AIConfigModel()
            {
                Name = "Reaper",
                Speed = 1f,
                Difficulty = 1
            };
            return ai;
        }
        //very hard ai: does everything faster
        public static AIConfigModel CreateNanite()
        {
            var ai = new AIConfigModel()
            {
                Name = "Nanite",
                Speed = .5f,
                Difficulty = 1
            };
            return ai;
        }
    }
}
