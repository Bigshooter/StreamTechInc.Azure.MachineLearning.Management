using System;
using System.Collections.Generic;
using System.Text;

namespace StreamTechInc.Azure.MachineLearning.Management
{
    public class DateTimeConvert
    {
        //stored as a MS DateTime
        private DateTime _time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public void SetTime(long ticks)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            _time = origin.AddSeconds(ticks);

        }

        public void SetTime(DateTime time)
        {
            _time = time;
        }
        public long GetJavaTime()
        {

            long returnValue = 0;

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = _time - origin;

            returnValue = (long)Math.Floor(diff.TotalSeconds);

            return returnValue;


        }
        public DateTime GetDateTime()
        {

            return _time;


        }

    }
}
