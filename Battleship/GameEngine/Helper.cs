using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ConsoleGameEngineCore;

namespace GameEngine
{
    public static class Helper
    {
        
        // https://stackoverflow.com/questions/7162834/determine-if-current-application-is-activated-has-focus
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero) {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        
        
        public static int FixConsole()
        {
            if (NativeMethods.GetStdHandle(-11) == new IntPtr(-1)) {
                return Marshal.GetLastWin32Error();
            }

            NativeMethods.CONSOLE_FONT_INFO_EX cfi = new NativeMethods.CONSOLE_FONT_INFO_EX();
            cfi.cbSize = (uint)Marshal.SizeOf(cfi);
            cfi.nFont = 0;

            cfi.dwFontSize.X = 8;
            cfi.dwFontSize.Y = 16;
            
            cfi.FaceName = "Consolas";

            NativeMethods.SetCurrentConsoleFontEx(NativeMethods.GetStdHandle(-11), false, ref cfi);
            return 0;
        }
    }
}