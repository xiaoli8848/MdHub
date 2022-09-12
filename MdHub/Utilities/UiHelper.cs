using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WinRT.Interop;

namespace MdHub.Utilities
{
    public static class UiHelper
    {
        private const int WaActive = 0x01;
        private const int WaInactive = 0x00;
        private const int WmActivate = 0x0006;

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        public static void SetTitleBarTransparent(Window window) => SetTitleBarTransparent(WindowNative.GetWindowHandle(window));

        public static void SetTitleBarTransparent(IntPtr windowHandle)
        {
            var res = App.Current.Resources;
            res["WindowCaptionBackground"] = Colors.Transparent;
            res["WindowCaptionBackgroundDisabled"] = Colors.Transparent;
            res["WindowCaptionForeground"] = Colors.Black;
            res["WindowCaptionForegroundDisabled"] = Colors.Black;

            var activeWindow = GetActiveWindow();
            if (windowHandle == activeWindow)
            {
                SendMessage(windowHandle, WmActivate, WaInactive, IntPtr.Zero);
                SendMessage(windowHandle, WmActivate, WaActive, IntPtr.Zero);
            }
            else
            {
                SendMessage(windowHandle, WmActivate, WaActive, IntPtr.Zero);
                SendMessage(windowHandle, WmActivate, WaInactive, IntPtr.Zero);
            }
        }
    }
}
