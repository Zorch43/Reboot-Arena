using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class MapModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<List<TerrainTileModel>> MapTiles { get; set; }
    }
}
