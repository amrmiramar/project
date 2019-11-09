using System;

namespace house_inviter
{
  internal static class HwndFinder
  {
    internal static IntPtr GetProcessWindow(int processId)
    {
      IntPtr childAfter = IntPtr.Zero;
      IntPtr windowEx;
      while (true)
      {
        IntPtr desktopWindow = NativeMethods.GetDesktopWindow();
        if (!(desktopWindow == IntPtr.Zero))
        {
          windowEx = NativeMethods.FindWindowEx(desktopWindow, childAfter, (string) null, (string) null);
          if (!(windowEx == IntPtr.Zero))
          {
            uint lpdwProcessId = 0U;
            int num = (int) NativeMethods.GetWindowThreadProcessId(windowEx, out lpdwProcessId);
            if ((long) lpdwProcessId != (long) processId || (!NativeMethods.IsWindowVisible(windowEx) || !(NativeMethods.GetParent(windowEx) == IntPtr.Zero)))
              childAfter = windowEx;
            else
              break;
          }
          else
            goto label_6;
        }
        else
          goto label_6;
      }
      return windowEx;
label_6:
      return IntPtr.Zero;
    }
  }
}
