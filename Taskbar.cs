using System.Runtime.InteropServices;

public class Taskbar
{
  private const int SW_HIDE = 0;
  private const int SW_SHOW = 1;
  private int _taskbarHandle;

  public Taskbar()
  {
    this._taskbarHandle = Taskbar.FindWindow("Shell_TrayWnd", "");
  }

  [DllImport("user32.dll")]
  private static extern int FindWindow(string className, string windowText);

  [DllImport("user32.dll")]
  private static extern int ShowWindow(int hwnd, int command);

  public void Show()
  {
    Taskbar.ShowWindow(this._taskbarHandle, 1);
  }

  public void Hide()
  {
    Taskbar.ShowWindow(this._taskbarHandle, 0);
  }
}
