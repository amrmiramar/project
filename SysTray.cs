using System;
using System.Drawing;
using System.Windows.Forms;

namespace house_inviter
{
  public class SysTray : IDisposable
  {
    private Color m_col = Color.Black;
    private NotifyIcon m_notifyIcon;
    private Font m_font;
    private Icon[] m_animationIcons;
    private Timer m_timer;
    private int m_currIndex;
    private int m_loopCount;
    private Icon m_DefaultIcon;

    public SysTray(string text, Icon icon, ContextMenu menu)
    {
      this.m_notifyIcon = new NotifyIcon();
      this.m_notifyIcon.Text = text;
      this.m_notifyIcon.Visible = true;
      this.m_notifyIcon.Icon = icon;
      this.m_DefaultIcon = icon;
      this.m_notifyIcon.ContextMenu = menu;
      this.m_font = new Font("Helvetica", 8f);
      this.m_timer = new Timer();
      this.m_timer.Interval = 100;
      this.m_timer.Tick += new EventHandler(this.m_timer_Tick);
    }

    public void ShowText(string text)
    {
      this.ShowText(text, this.m_font, this.m_col);
    }

    public void ShowText(string text, Color col)
    {
      this.ShowText(text, this.m_font, col);
    }

    public void ShowText(string text, Font font)
    {
      this.ShowText(text, font, this.m_col);
    }

    public void ShowText(string text, Font font, Color col)
    {
      Bitmap bitmap = new Bitmap(16, 16);
      Brush brush = (Brush) new SolidBrush(col);
      Graphics.FromImage((Image) bitmap).DrawString(text, this.m_font, brush, 0.0f, 0.0f);
      this.m_notifyIcon.Icon = Icon.FromHandle(bitmap.GetHicon());
    }

    public void SetAnimationClip(Icon[] icons)
    {
      this.m_animationIcons = icons;
    }

    public void SetAnimationClip(Bitmap[] bitmap)
    {
      this.m_animationIcons = new Icon[bitmap.Length];
      for (int index = 0; index < bitmap.Length; ++index)
        this.m_animationIcons[index] = Icon.FromHandle(bitmap[index].GetHicon());
    }

    public void SetAnimationClip(Bitmap bitmapStrip)
    {
      this.m_animationIcons = new Icon[bitmapStrip.Width / 16];
      for (int index = 0; index < this.m_animationIcons.Length; ++index)
      {
        Rectangle rect = new Rectangle(index * 16, 0, 16, 16);
        Bitmap bitmap = bitmapStrip.Clone(rect, bitmapStrip.PixelFormat);
        this.m_animationIcons[index] = Icon.FromHandle(bitmap.GetHicon());
      }
    }

    public void StartAnimation(int interval, int loopCount)
    {
      if (this.m_animationIcons == null)
        throw new ApplicationException("Animation clip not set with SetAnimationClip");
      this.m_loopCount = loopCount;
      this.m_timer.Interval = interval;
      this.m_timer.Start();
    }

    public void StopAnimation()
    {
      this.m_timer.Stop();
    }

    public void Dispose()
    {
      this.m_notifyIcon.Dispose();
      if (this.m_font == null)
        return;
      this.m_font.Dispose();
    }

    private void m_timer_Tick(object sender, EventArgs e)
    {
      if (this.m_currIndex < this.m_animationIcons.Length)
      {
        this.m_notifyIcon.Icon = this.m_animationIcons[this.m_currIndex];
        ++this.m_currIndex;
      }
      else
      {
        this.m_currIndex = 0;
        if (this.m_loopCount <= 0)
        {
          this.m_timer.Stop();
          this.m_notifyIcon.Icon = this.m_DefaultIcon;
        }
        else
          --this.m_loopCount;
      }
    }
  }
}
