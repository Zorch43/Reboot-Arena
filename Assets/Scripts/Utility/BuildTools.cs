using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class BuildTools
    {
        public static bool IsValidBuildSite(Vector3 site, float radius)
        {
            var hits = Physics.OverlapSphere(site, radius);
            foreach(var h in hits)
            {
                if (BlocksConstruction(h.gameObject))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool BlocksConstruction(GameObject obj)
        {
            //constructed drones block construction (units don't)
            //walls block construction (Terrain doesn't)
            //spawn fields block construction
            //objectives block construction
            //pickup spawners block construction (packs don't)
            if (obj.CompareTag("Drone")
                || obj.CompareTag("Wall")
                || obj.CompareTag("SpawnPoint")
                || obj.CompareTag("Objective")
                || obj.CompareTag("Pickup"))
            {
                return true;
            }

            //units don't block construction
            //build holograms don't block construction
            //terrain doesn't block construction
            //bullets/projectiles don't block construction
            //health/ammo packs don't block construction

            return false;
        }
    }
}
