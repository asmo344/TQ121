using System.Diagnostics;

namespace System.Threading
{
    public static class MyThread
    {
        public static float Wait(this System.Threading.Thread thread, int timeout = Timeout.Infinite)
        {
            var sw = Stopwatch.StartNew();
            while (thread.IsAlive)
            {
                if (sw.ElapsedMilliseconds > timeout && timeout != Timeout.Infinite)
                    break;
            }
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public static void WaitAll(params System.Threading.Thread[] thread)
        {
            while (IsAnyAlive()) ;
            bool IsAnyAlive()
            {
                for (int i = 0; i < thread.Length; i++)
                {
                    if (thread[i].IsAlive)
                        return true;
                }
                return false;
            }
        }
    }
}