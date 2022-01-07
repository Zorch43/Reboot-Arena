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
                    CreateLevel0(),
                    CreateLevel1(),
                    CreateLevel2(),
                    CreateLevel3(),
                    CreateLevel4(),
                    CreateLevel5(),
                    CreateLevel6(),
                    CreateLevel7(),
                    CreateLevel8(),
                    CreateLevel9()
                };
            }
            return _aiConfigs;
        }
        //easiest ai: only rushes and defends, slow tactical speed
        public static AIConfigModel CreateLevel0()
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
        public static AIConfigModel CreateLevel1()
        {
            var ai = new AIConfigModel()
            {
                Name = "Toaster",
                Speed = 3,
                Difficulty = 0.1f,
                RetreatMod = 1
            };
            return ai;
        }
        //very easy ai: can perform targeted attacks
        public static AIConfigModel CreateLevel2()
        {
            var ai = new AIConfigModel()
            {
                Name = "CPU",
                Speed = 3,
                Difficulty = 0.2f,
                AttackMod = .4f,
                RetreatMod = 1
            };
            return ai;
        }
        //easy ai: can do everything... poorly
        public static AIConfigModel CreateLevel3()
        {
            var ai = new AIConfigModel()
            {
                Name = "Drone",
                Speed = 2,
                Difficulty = .3f,
                AttackMod = .3f,
                RetreatMod = 1
            };
            return ai;
        }
        //normal ai: can do a little of everything, prioritizes resupplying and healing
        public static AIConfigModel CreateLevel4()
        {
            var ai = new AIConfigModel()
            {
                Name = "Soldier",
                Speed = 2,
                Difficulty = .4f,
                RetreatMod = 1,
                AmmoMod = 1,
                HealMod = 1,
            };
            return ai;
        }
        //normal ai: can do a little of everything, faster
        public static AIConfigModel CreateLevel5()
        {
            var ai = new AIConfigModel()
            {
                Name = "Cyborg",
                Speed = 1.5f,
                Difficulty = .5f,
                RetreatMod = 1f,
                CriticalHealMod = 1f,
                CriticalAmmoMod = 1f
            };
            return ai;
        }
        //hard ai: can do a most everything, fast
        public static AIConfigModel CreateLevel6()
        {
            var ai = new AIConfigModel()
            {
                Name = "Killbot",
                Speed = 1f,
                Difficulty = .75f,
                RetreatMod = .25f,
                AttackMod = 1
            };
            return ai;
        }
        //hard ai: can do everything, slowly
        public static AIConfigModel CreateLevel7()
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
        public static AIConfigModel CreateLevel8()
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
        public static AIConfigModel CreateLevel9()
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
