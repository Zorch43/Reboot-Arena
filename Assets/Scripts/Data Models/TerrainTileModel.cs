using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class TerrainTileModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SpriteName { get; set; }
        public TerrainDataModel TerrainData { get; set; }
    }
}
