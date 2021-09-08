using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class TerrainDataModel
    {
        public bool IsImpassable { get; set; }//whether ground units can move through this tile
        public bool IsImpenetrable { get; set; }//whether non-arcing ranged attacks can pass through this tile
    }
}
