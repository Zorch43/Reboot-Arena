using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class TeamTools
    {
        public static Color GetTeamColor(int team)
        {
            switch (team)
            {
                case 0:
                    return Color.red;
                case 1:
                    return new Color(0, 0.15f, 1);//blue
                case 2:
                    return new Color(1, 0.85f, 0);//gold
                case 3:
                    return new Color(0, 0.7f, 0);//green (darker)
                case 4:
                    return new Color(1, 0.5f, 0);//orange
                case 5:
                    return new Color(0.7f, 0, 1);//purple
                case 6:
                    return Color.magenta;
                case 7:
                    return Color.cyan;
                case 8:
                    return new Color(0.1f, 0.1f, 0.1f); //off-white
                case 9:
                    return new Color(0.9f, 0.9f, 0.9f);//dark grey
                default:
                    return Color.grey;
            }
        }
        public static string GetTeamColorHex(int team)
        {
            var color = GetTeamColor(team);
            string hex = string.Format("{0}{1}{2}{3}", 
                ColorFloatToByte(color.r), ColorFloatToByte(color.g), ColorFloatToByte(color.b), ColorFloatToByte(color.a));
            return hex;
        }

        public static List<int> GetRandomTeams(int teamCount)
        {
            var teamList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            List<int> pickedTeams = new List<int>();
            for(int i = 0; i < teamCount; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, teamList.Count - 1);
                pickedTeams.Add(teamList[randomIndex]);
                teamList.RemoveAt(randomIndex);
            }

            return pickedTeams;
        }

        private static string ColorFloatToByte(float colorFloat)
        {
            return ((int)(colorFloat * 255)).ToString("X");
        }
    }
}
