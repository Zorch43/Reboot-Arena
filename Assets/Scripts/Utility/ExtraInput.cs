using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Data_Models.KeyBindModel;

namespace Assets.Scripts.Utility
{
    public static class ExtraInput
    {
        public static bool GetKey(KeyCodeExtra key)
        {
            switch (key)
            {
                case KeyCodeExtra.MouseWheelUp:
                    return Input.mouseScrollDelta.y > 0;
                case KeyCodeExtra.MouseWheelDown:
                    return Input.mouseScrollDelta.y < 0;
            }
            return false;
        }
        public static bool GetKeyDown(KeyCodeExtra key)
        {
            switch (key)
            {
                case KeyCodeExtra.MouseWheelUp:
                    return Input.mouseScrollDelta.y > 0;
                case KeyCodeExtra.MouseWheelDown:
                    return Input.mouseScrollDelta.y < 0;
            }
            return false;
        }
    }
}
