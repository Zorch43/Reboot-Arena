using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class TrajectoryTools
    {
        public const float MIN_ARC_HEIGHT = 3f;
        /// <summary>
        /// Get the firing velocity that will result in a parabolic arc that conforms to restrictions
        /// </summary>
        /// <param name="startPos">Start position (t = 0)</param>
        /// <param name="targetPos">target impact position</param>
        /// <param name="minHeight">The minimum height above the highest position that the apogee of the arc should reach</param>
        /// <param name="gravity">gravitational acceleration constant</param>
        /// <returns></returns>
        public static Vector3 GetInitialVelocity(Vector3 startPos, Vector3 targetPos, float minHeight, float gravity)
        {
            Vector3 displacement = targetPos - startPos;
            float minArcHeight = Math.Max(startPos.y, targetPos.y) - startPos.y + minHeight;
            float verticalVelocity = (float)Math.Sqrt(2 * minArcHeight * gravity);
            //quadratic equation
            float timeToTarget = (float)(-verticalVelocity - Math.Sqrt(Math.Abs(Math.Pow(verticalVelocity, 2) + 2 * gravity * displacement.y))) / -gravity;

            Vector3 initialVelocity = new Vector3(displacement.x/timeToTarget, verticalVelocity, displacement.z/timeToTarget);

            return initialVelocity;
        }
    }
}
