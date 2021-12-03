using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IMove
    {
        public void StartPath(Vector3 destination);//move towards the destination
        public bool HasArrived();//whether a moving unit has reached its destination
        public void Stop();//stop moving
        public Vector3 GetDestination();//get the mover's destination if it has one
        public bool IsMoving();//whether the mover has a destination
        public int ShouldBlock();//whether the mover should prevent itself from moving, bcome easy to move, or revert to default
        public void Block();//block the path of other movers
        public void UnBlock();//allow other movers to pass
    }
}
