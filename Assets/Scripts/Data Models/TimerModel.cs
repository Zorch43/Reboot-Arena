using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class TimerModel
    {
        public float RawTime { get; set; }
        public int Minutes
        {
            get
            {
                return (int)(RawTime / 60);
            }
        }
        public int Seconds
        {
            get
            {
                return (int)(RawTime % 60);
            }
        }
        public override string ToString()
        {
            return string.Format("{0}:{1}", Minutes.ToString("D2"), Seconds.ToString("D2"));
        }
    }
}
