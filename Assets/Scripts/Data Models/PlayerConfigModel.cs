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
        //TODO: AI settings
        //TODO: network player
    }
}
