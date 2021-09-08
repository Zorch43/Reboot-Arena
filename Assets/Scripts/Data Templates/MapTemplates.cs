using Assets.Scripts.Data_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Templates
{
    public static class MapTemplates
    {
        public static MapModel GetBlankMap(int width, int height)
        {
            var flatTerrain = new TerrainTileModel()
            {
                Id = "_",
                Name = "Basic Flat",
                SpriteName = "BasicFlat",
                TerrainData = TerrainDataTemplates.GetTemplate(TerrainDataTemplates.TerrainType.Flat)
            };
            var list = new List<List<TerrainTileModel>>();
            
            for(int x = 0; x < width; x++)
            {
                list.Add(new List<TerrainTileModel>());
                for(int y = 0; y < height; y++)
                {
                    list[x].Add(flatTerrain);
                }
            }

            var map = new MapModel()
            {
                Id = "BlankMap",
                Name = "Generated Blank Map",
                MapTiles = list
            };
            return map;
        }

        public static MapModel getBasicRandomMap(int width, int height, double featureChance)
        {
            var flatTerrain = new TerrainTileModel()
            {
                Id = "_",
                Name = "Basic Flat",
                SpriteName = "BasicFlat",
                TerrainData = TerrainDataTemplates.GetTemplate(TerrainDataTemplates.TerrainType.Flat)
            };
            var holeTerrain = new TerrainTileModel()
            {
                Id = "#",
                Name = "Basic Hole",
                SpriteName = "BasicHole",
                TerrainData = TerrainDataTemplates.GetTemplate(TerrainDataTemplates.TerrainType.Hole)
            };
            var wallTerrain = new TerrainTileModel()
            {
                Id = "|",
                Name = "Basic Wall",
                SpriteName = "BasicWall",
                TerrainData = TerrainDataTemplates.GetTemplate(TerrainDataTemplates.TerrainType.Wall)
            };
            var rand = new Random();

            var list = new List<List<TerrainTileModel>>();

            for (int x = 0; x < width; x++)
            {
                list.Add(new List<TerrainTileModel>());
                for (int y = 0; y < height; y++)
                {
                    var randNum = rand.NextDouble();
                    if(randNum < featureChance/2)
                    {
                        list[x].Add(wallTerrain);
                    }
                    else if(randNum < featureChance)
                    {
                        list[x].Add(holeTerrain);
                    }
                    else
                    {
                        list[x].Add(flatTerrain);
                    }
                }
            }

            var map = new MapModel()
            {
                Id = "RandomMap",
                Name = "Generated Random Map",
                MapTiles = list
            };
            return map;
        }
    }
}
