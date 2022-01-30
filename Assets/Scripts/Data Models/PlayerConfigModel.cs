using Assets.Scripts.Data_Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    [System.Serializable]
    public class PlayerConfigModel
    {
        public enum ControlType
        {
            None,
            Player,
            //NetworkPlayer,
            AI
        }

        public int TeamId;

        public ControlType Controller;
        //AI settings
        public int AIIndex;//if control type is AI, pick the AI from list using index
        //TODO: network player

        //unit selection
        public UnitClassTemplates.UnitClasses[] UnitClasses;

        //constructor
        public PlayerConfigModel()
        {
            UnitClasses = new UnitClassTemplates.UnitClasses[] 
            { 
                UnitClassTemplates.UnitClasses.Ranger,
                UnitClassTemplates.UnitClasses.Trooper,
                UnitClassTemplates.UnitClasses.Fabricator 
            };

            AIIndex = 2;
        }
    }
}
