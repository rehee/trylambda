using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace System
{
    public static class SelfRecall
    {
        #region
        static string url { get; set; } = "";
        static void activeUrl(string url)
        {
            checks.ForEach(b => b());
            System.Net.HttpWebRequest myHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            myHttpWebRequest.Timeout = 300000;
            myHttpWebRequest.KeepAlive = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 200;
            try
            {
                var myHttpWebResponse = (System.Net.HttpWebResponse)myHttpWebRequest.GetResponse();
                if (myHttpWebResponse != null)
                {
                    myHttpWebResponse.Close();
                }
                if (myHttpWebRequest != null)
                {
                    myHttpWebRequest.Abort();
                }
            }
            catch
            {
                if (myHttpWebRequest != null)
                {
                    myHttpWebRequest.Abort();
                }
            }
        }
        #endregion

        static bool isRunning { get; set; } = false;
        static List<Action> checks { get; set; }
        public static string Start(string url)
        {
            if (isRunning)
                return "";
            SelfRecall.url = url;
            isRunning = true;
            checks = E.InitCheckFuncList(new List<dynamic>() { E.MyCache, E.MyQuerys }, 30);
            System.Timers.Timer myTimer = new System.Timers.Timer(1000 * 60);
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(SelfRecall.Handler);
            myTimer.Enabled = true;
            myTimer.AutoReset = true;
            return "";
        }
        public static void Handler(object source, ElapsedEventArgs e)
        {
            run();
        }
        public static void Handler()
        {
            run();
        }
        private static void run()
        {
            activeUrl(url);
        }
    }
}