using System;

namespace KsDumperClient.Utility
{
    public static class Logger
    {
        public static event Action<string> OnLog;

        public static void SkipLine()
        {
            if (OnLog != null)
            {
                OnLog("\n");
            }
            else
            {
                Console.WriteLine();
            }
        }

        public static void Log(string message, params object[] args)
        {
            message = string.Format("[{0}] {1}\n", DateTime.Now.ToLongTimeString(), string.Format(message, args));

            if (OnLog != null)
            {
                OnLog(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
