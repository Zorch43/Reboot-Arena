using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class TeamModel
    {
        public string Name { get; set; }
        public List<UnitClassModel> BuildOptions { get; set; }

    }
}
