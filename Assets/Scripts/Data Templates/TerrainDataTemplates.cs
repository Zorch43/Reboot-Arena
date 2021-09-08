using Assets.Scripts.Data_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Templates
{
    public static class TerrainDataTemplates
    {
        public enum TerrainType
        {
            Flat,
            Hole,
            Wall
        }
        public static List<TerrainDataModel> GetTemplates()
        {
            var list = new List<TerrainDataModel>();
            list.Add(GetBasicFlatTile());
            list.Add(GetBasicChasmTile());
            list.Add(GetBasicWallTile());
            return list;
        }

        public static TerrainDataModel GetTemplate(TerrainType type)
        {
            switch (type)
            {
                case TerrainType.Hole:
                    return GetBasicChasmTile();
                case TerrainType.Wall:
                    return GetBasicWallTile();
                default:
                    return GetBasicFlatTile();
            }
        }

        static TerrainDataModel GetBasicFlatTile()
        {
            var tile = new TerrainDataModel()
            {
                IsImpassable = false,
                IsImpenetrable = false
            };
            return tile;
        }

        static TerrainDataModel GetBasicChasmTile()
        {
            var tile = new TerrainDataModel()
            {
                IsImpassable = true,
                IsImpenetrable = false
            };
            return tile;
        }

        static TerrainDataModel GetBasicWallTile()
        {
            var tile = new TerrainDataModel()
            {
                IsImpassable = true,
                IsImpenetrable = true
            };
            return tile;
        }
    }
}
