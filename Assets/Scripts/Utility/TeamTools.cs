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
                    return Color.blue;
                case 2:
                    return Color.yellow;
                case 3:
                    return Color.green;
                case 4:
                    return new Color(1, 0.5f, 0);//orange
                case 5:
                    return new Color(0.5f, 0, 1);//purple
                case 6:
                    return Color.magenta;
                case 7:
                    return Color.cyan;
                case 8:
                    return Color.black;
                case 9:
                    return Color.white;
                default:
                    return Color.grey;
            }
        }
    }
}
