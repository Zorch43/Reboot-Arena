using Assets.Scripts.Data_Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class PlayerConfigModel
    {
        public enum ControlType
        {
            None,
            Player,
            //NetworkPlayer,
            AI
        }

        public int TeamId { get; set; }
        
        public ControlType Controller { get; set; }
        //AI settings
        public int AIIndex { get; set; }//if control type is AI, pick the AI from list using index
        //TODO: network player

        //unit selection
        public List<UnitClassTemplates.UnitClasses> UnitClasses { get; set; }

        //constructor
        public PlayerConfigModel()
        {
            UnitClasses = new List<UnitClassTemplates.UnitClasses>() 
            {
                UnitClassTemplates.UnitClasses.Ranger,
                UnitClassTemplates.UnitClasses.Trooper, 
                UnitClassTemplates.UnitClasses.Fabricator 
            };
        }
    }
}
