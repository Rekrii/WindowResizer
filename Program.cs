﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ScaleWindowResizer
{
    class Program
    {
        static void Main(string[] args)
        {
            [DllImport("user32.dll", SetLastError = true)]
            static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);
            [DllImport("user32.dll")]
            static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

            //string text;
            //using (var streamReader = new StreamReader(@"file.txt", Encoding.UTF8))
            //{
            //    text = streamReader.ReadToEnd();
            //}

            int count = 0;
            foreach (KeyValuePair<IntPtr, string> window in OpenWindowGetter.GetOpenWindows())
            {
                IntPtr handle = window.Key;
                string title = window.Value;

                int processId;
                GetWindowThreadProcessId(handle, out processId);
                string processPath = Process.GetProcessById(processId).ToString();

                Console.WriteLine("Process: {0}", processPath);

                if (processPath.Contains("(explorer)"))
                {
                    MoveWindow(handle, 50 + 25 * count, 50 + 25 * count, 1150, 650, true);
                    count++;
                }
                if (processPath.Contains("(msedge)"))
                {
                    MoveWindow(handle, 50, 50, 2300, 1300, true);
                    count++;
                }
                if (processPath.Contains("(OUTLOOK)"))
                {
                    MoveWindow(handle, 50, 50, 2300, 1300, true);
                    count++;
                }
            }

        }

    }

    /// <summary>Contains functionality to get all the open windows.</summary>
    public static class OpenWindowGetter
    {
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<IntPtr, string> GetOpenWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();
    }
}
