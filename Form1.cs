using house_inviter.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace house_inviter
{
  public class Form1 : Form
  {
    private bool check = true;
    public const uint WM_KEYDOWN = 256U;
    public const uint VK_CLEAR = 12U;
    public const uint WM_KEYUP = 257U;
    public const uint VK_RIGHT = 27U;
    public const uint VK_CONTROL = 11U;
    public const uint VK_ALT = 12U;
    public const int WM_CLOSE = 16;
    public const int WM_LBUTTONDOWN = 513;
    public const int WM_LBUTTONUP = 514;
    public const int WM_GETTEXT = 13;
    public const string xyz = "Sylluss";
    public const string configFile = "config";
    public const string invFile = "Sylluss/inv";
    private Process[] processlist;
    private IntPtr _window;
    private int selectedProcessId;
    private int x;
    private int y;
    private string fileHash;
    private bool blockWhenChangingStatus;
    private IContainer components;
    private ComboBox comboBox1;
    private Button button1;
    private Button button2;
    private System.Windows.Forms.Timer timer1;
    private Button button4;
    private Label label3;
    private Label label4;
    private LinkLabel linkLabel1;
    private NotifyIcon notifyIcon1;

    public Form1()
    {
      this.InitializeComponent();
      ContextMenu menu = new ContextMenu();
      menu.MenuItems.Add(0, new MenuItem("Exit", new EventHandler(this.Exit_Click)));
      menu.MenuItems.Add(0, new MenuItem("Show", new EventHandler(this.Show_Click)));
      SysTray sysTray = new SysTray("HOUSE AUTO INVITER - Sylluss", Resources.icon, menu);
      Bitmap bitmapStrip = new Bitmap((Image) Resources.icon1);
      bitmapStrip.MakeTransparent();
      sysTray.SetAnimationClip(bitmapStrip);
      sysTray.StartAnimation(200, 30);
      Bitmap icon1 = Resources.icon1;
      icon1.MakeTransparent();
      this.Icon = Icon.FromHandle(icon1.GetHicon());
    }

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SendMessage(IntPtr A_0, int A_1, int A_2, int A_3);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetWindowText(int hWnd, StringBuilder title, int size);

    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr), Out] StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetForegroundWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern short VkKeyScan(char ch);

    public static string Base64Encode(string plainText)
    {
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
    }

    public static string Base64Decode(string base64EncodedData)
    {
      return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
    }

    private static string decypher(string str)
    {
      string str1 = "";
      string plainText = Form1.Base64Encode("Sylluss");
      while (plainText.Length < str.Length)
        plainText = Form1.Base64Encode(plainText);
      for (int index = 0; index < str.Length; ++index)
      {
        int num = (int) Encoding.GetEncoding(1252).GetBytes(str)[index] - (int) plainText[index];
        str1 += ((char) num).ToString();
      }
      return str1;
    }

    private string GetWindowTitle(IntPtr hWn)
    {
      object obj = new object();
      StringBuilder lpString = new StringBuilder(1024);
      Form1.GetWindowText(hWn, lpString, lpString.Capacity);
      return lpString.ToString();
    }

    public static void click(IntPtr hWnd, int x, int y)
    {
      Form1.SendMessage(hWnd, 513, 1, y << 16 | x);
      Form1.SendMessage(hWnd, 514, 0, y << 16 | x);
    }

    private void closeAletaSioModal()
    {
      Form1.click(this._window, this.x, this.y);
    }

    public void LoadTibiasToComboBox()
    {
      this.processlist = Process.GetProcesses();
      this.comboBox1.Items.Clear();
      foreach (Process process in this.processlist)
      {
        if (((!string.IsNullOrEmpty(process.MainWindowTitle) ? 1 : 0) & (process.ProcessName.Contains("Tibia") ? 1 : (process.ProcessName.Contains("Hexera") ? 1 : 0))) != 0)
          this.comboBox1.Items.Add((object) new ComboboxItem()
          {
            Text = String.Format("{0}: {1}", process.Id, process.MainWindowTitle),
            Value = (object) process.Id
          });
      }
    }

    public static string readFileAsUtf8(string fileName)
    {
      Encoding srcEncoding = Encoding.Default;
      string s = string.Empty;
      using (StreamReader streamReader = new StreamReader(fileName, Encoding.Default))
      {
        s = streamReader.ReadToEnd();
        srcEncoding = streamReader.CurrentEncoding;
        streamReader.Close();
      }
      if (srcEncoding == Encoding.UTF8)
        return s;
      byte[] bytes = srcEncoding.GetBytes(s);
      return Encoding.UTF8.GetString(Encoding.Convert(srcEncoding, Encoding.UTF8, bytes));
    }

    public void updateAletaSioList()
    {
      string str1 = "aleta sio";
      if (this.blockWhenChangingStatus)
        return;
      this.blockWhenChangingStatus = true;
      this.Enabled = false;
      Form1.PostMessage(this._window, 256U, 13, 0);
      for (int index = 0; index < str1.Length; ++index)
        Form1.PostMessage(this._window, 256U, (int) char.ToUpper(str1[index]), 0);
      Form1.PostMessage(this._window, 256U, 13, 0);
      Thread.Sleep(300);
      for (int index = 0; index < 500; ++index)
        Form1.PostMessage(this._window, 256U, 8, 0);
      Form1.PostMessage(this._window, 256U, 8, 0);
      Thread.Sleep(300);
      using (StreamReader streamReader = new StreamReader("Sylluss/inv", Encoding.GetEncoding(1252), true))
      {
        string str2 = Form1.decypher(streamReader.ReadLine());
        for (int index = 0; index < str2.Length; ++index)
        {
          if ((int) str2[index] == 47)
            Form1.PostMessage(this._window, 256U, 13, 0);
          else if ((int) str2[index] == 39)
            Form1.PostMessage(this._window, 256U, 222, 0);
          else
            Form1.PostMessage(this._window, 256U, (int) char.ToUpper(str2[index]), 0);
        }
      }
      Thread.Sleep(500);
      this.closeAletaSioModal();
      this.blockWhenChangingStatus = false;
      this.Enabled = true;
    }

    private void Event(object sender, EventArgs e)
    {
      if (this.check)
      {
        Form1.MouseHook.stop();
        this.x = Cursor.Position.X;
        this.y = Cursor.Position.Y - 25;
        this.label3.Text = this.x.ToString();
        this.label4.Text = this.y.ToString();
        StreamWriter streamWriter = new StreamWriter("config");
        streamWriter.WriteLine(this.x);
        streamWriter.WriteLine(this.y);
        streamWriter.Close();
        int num = (int) MessageBox.Show("Config saved.");
        this.check = false;
      }
      else
      {
        Form1.MouseHook.stop();
        int num = (int) MessageBox.Show("Please restart house inviter and reconfigure ok button.");
      }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      this.ShowInTaskbar = false;
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.LoadTibiasToComboBox();
      if (this.comboBox1.Items.Count > 0)
        this.comboBox1.SelectedItem = (object) 0;
      if (File.Exists("config"))
      {
        using (StreamReader streamReader = new StreamReader((Stream) new FileStream("config", FileMode.Open, FileAccess.Read)))
        {
          if (int.TryParse(streamReader.ReadLine(), out this.x) && int.TryParse(streamReader.ReadLine(), out this.y))
          {
            this.label3.Text = this.x.ToString();
            this.label4.Text = this.y.ToString();
          }
          else
          {
            int num = (int) MessageBox.Show("Error");
          }
        }
      }
      else
      {
        using (File.Create("config"))
        {
          int num = (int) MessageBox.Show("Configure OK button.");
        }
      }
      this.LoadTibiasToComboBox();
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.selectedProcessId = Convert.ToInt32(((ComboboxItem) this.comboBox1.SelectedItem).Value);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.LoadTibiasToComboBox();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this._window = HwndFinder.GetProcessWindow(this.selectedProcessId);
      this.timer1.Enabled = true;
      this.Text = "HOUSE AUTO INVITER - Sylluss";
      if (File.Exists("Sylluss/inv"))
      {
        Form1.ShowWindow(this._window, 3);
        Form1.SetForegroundWindow(this._window);
      }
      else
      {
        int num = (int) MessageBox.Show("First execute lua file.");
      }
    }

    public bool IsFileLocked(string filePath)
    {
      try
      {
        using (File.Open(filePath, FileMode.Open))
          ;
      }
      catch (IOException ex)
      {
        return true;
      }
      return false;
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      if (!(File.Exists("Sylluss/inv") & !this.IsFileLocked("Sylluss/inv")))
        return;
      byte[] hash = new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes("Sylluss/inv"));
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("x2"));
      string str = stringBuilder.ToString();
      if (!(str != this.fileHash))
        return;
      using (new StreamReader("Sylluss/inv", Encoding.GetEncoding(1252), true))
      {
        this.fileHash = str;
        this.updateAletaSioList();
      }
    }

    private void button4_Click(object sender, EventArgs e)
    {
      Form1.MouseHook.Start();
      Form1.MouseHook.MouseAction += new EventHandler(this.Event);
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      Process.Start("http://pandoriumx.com/forum/index.php/user/175-elderapo/");
    }

    protected void Exit_Click(object sender, EventArgs e)
    {
      Application.Exit();
      this.Close();
    }

    protected void Hide_Click(object sender, EventArgs e)
    {
      this.Hide();
    }

    protected void Show_Click(object sender, EventArgs e)
    {
      this.Show();
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
      if (this.WindowState == FormWindowState.Minimized)
      {
        this.ShowInTaskbar = false;
        this.notifyIcon1.Visible = true;
      }
      else
      {
        this.ShowInTaskbar = true;
        this.notifyIcon1.Visible = false;
      }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.Hide();
      e.Cancel = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
      this.comboBox1 = new ComboBox();
      this.button1 = new Button();
      this.button2 = new Button();
      this.timer1 = new System.Windows.Forms.Timer();
      this.button4 = new Button();
      this.label3 = new Label();
      this.label4 = new Label();
      this.linkLabel1 = new LinkLabel();
      this.notifyIcon1 = new NotifyIcon();
      this.SuspendLayout();
      this.comboBox1.FlatStyle = FlatStyle.Popup;
      this.comboBox1.Location = new Point(12, 12);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new Size(168, 21);
      this.comboBox1.TabIndex = 0;
      this.comboBox1.SelectedIndexChanged += new EventHandler(this.comboBox1_SelectedIndexChanged);
      this.button1.DialogResult = DialogResult.Cancel;
      this.button1.Location = new Point(191, 12);
      this.button1.Name = "button1";
      this.button1.Size = new Size(81, 23);
      this.button1.TabIndex = 1;
      this.button1.Text = "Reload list";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.button2.Location = new Point(12, 39);
      this.button2.Name = "button2";
      this.button2.Size = new Size(168, 23);
      this.button2.TabIndex = 2;
      this.button2.Text = "Launch";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.button2_Click);
      this.timer1.Tick += new EventHandler(this.timer1_Tick);
      this.button4.Location = new Point(191, 38);
      this.button4.Name = "button4";
      this.button4.Size = new Size(81, 23);
      this.button4.TabIndex = 10;
      this.button4.Text = "Edit config";
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new EventHandler(this.button4_Click);
      this.label3.AutoSize = true;
      this.label3.Location = new Point(199, 64);
      this.label3.Name = "label3";
      this.label3.Size = new Size(12, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "x";
      this.label4.AutoSize = true;
      this.label4.Location = new Point(231, 64);
      this.label4.Name = "label4";
      this.label4.Size = new Size(12, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "y";
      this.linkLabel1.AutoSize = true;
      this.linkLabel1.Location = new Point(12, 65);
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.Size = new Size(80, 13);
      this.linkLabel1.TabIndex = 11;
      this.linkLabel1.TabStop = true;
      this.linkLabel1.Text = "Contact creator";
      this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
      this.notifyIcon1.Text = "notifyIcon1";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.AutoValidate = AutoValidate.EnablePreventFocusChange;
      this.BackgroundImageLayout = ImageLayout.Zoom;
      this.ClientSize = new Size(284, 84);
      this.Controls.Add((Control) this.linkLabel1);
      this.Controls.Add((Control) this.button4);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.button2);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.comboBox1);
      this.FormBorderStyle = FormBorderStyle.Fixed3D;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.ImeMode = ImeMode.Off;
      this.MaximizeBox = false;
      this.Name = "Form1";
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.Text = "Auto inviter";
      this.TopMost = true;
      this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public static class MouseHook
    {
      private static Form1.MouseHook.LowLevelMouseProc _proc = new Form1.MouseHook.LowLevelMouseProc(Form1.MouseHook.HookCallback);
      private static IntPtr _hookID = IntPtr.Zero;
      private const int WH_MOUSE_LL = 14;

      public static event EventHandler MouseAction = (param0, param1) => {};

      public static void Start()
      {
        Form1.MouseHook._hookID = Form1.MouseHook.SetHook(Form1.MouseHook._proc);
      }

      public static void stop()
      {
        Form1.MouseHook.UnhookWindowsHookEx(Form1.MouseHook._hookID);
      }

      private static IntPtr SetHook(Form1.MouseHook.LowLevelMouseProc proc)
      {
        using (Process currentProcess = Process.GetCurrentProcess())
        {
          using (ProcessModule mainModule = currentProcess.MainModule)
            return Form1.MouseHook.SetWindowsHookEx(14, proc, Form1.MouseHook.GetModuleHandle(mainModule.ModuleName), 0U);
        }
      }

      private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
      {
        if (nCode >= 0 && 513 == (int) wParam)
        {
          Form1.MouseHook.MSLLHOOKSTRUCT msllhookstruct = (Form1.MouseHook.MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof (Form1.MouseHook.MSLLHOOKSTRUCT));
          Form1.MouseHook.MouseAction((object) null, new EventArgs());
        }
        return Form1.MouseHook.CallNextHookEx(Form1.MouseHook._hookID, nCode, wParam, lParam);
      }

      [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern IntPtr SetWindowsHookEx(int idHook, Form1.MouseHook.LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

      [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool UnhookWindowsHookEx(IntPtr hhk);

      [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern IntPtr GetModuleHandle(string lpModuleName);

      private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

      private enum MouseMessages
      {
        WM_MOUSEMOVE = 512,
        WM_LBUTTONDOWN = 513,
        WM_LBUTTONUP = 514,
        WM_RBUTTONDOWN = 516,
        WM_RBUTTONUP = 517,
        WM_MOUSEWHEEL = 522,
      }

      private struct POINT
      {
        public int x;
        public int y;
      }

      private struct MSLLHOOKSTRUCT
      {
        public Form1.MouseHook.POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
      }
    }
  }
}
