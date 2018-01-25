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
        public static string Start(string url)
        {
            if (isRunning)
                return "";
            SelfRecall.url = url;
            isRunning = true;
            System.Timers.Timer myTimer = new System.Timers.Timer(1000 * 60 * 10);
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