using System.Runtime.InteropServices;

namespace Game.Core.Diagnostics
{
    public static class CrashLogHelper
    {
        public static void LogUnhandledException(Exception e)
        {
            using (var fs = File.CreateText("crashlog.txt"))
            {
                fs.WriteLine(RuntimeInformation.OSDescription + "\n");
                fs.WriteLine(e.ToString());
            }

            Environment.Exit(1);
        }
    }
}
