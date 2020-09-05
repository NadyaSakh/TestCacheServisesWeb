using System;

namespace TestCacheServisesWeb.Utils
{
    public class TimeUtils
    {
        public static string showEllapsedTime(TimeSpan ts)
        {
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
           /* Console.WriteLine("RunTime " + elapsedTime);*/
            return elapsedTime;
        }
    }
}
