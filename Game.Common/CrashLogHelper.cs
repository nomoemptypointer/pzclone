using System.Runtime.InteropServices;
using Vortice.DXGI;

namespace Game.Common
{
    public static class CrashLogHelper
    {
        public static void LogUnhandledException(Exception e, GameBase game)
        {
            using (var fs = File.CreateText("crashlog.txt"))
            {
                fs.WriteLine(RuntimeInformation.OSDescription + "\n");
                fs.WriteLine(e.ToString());
            }
        }
    }
}
