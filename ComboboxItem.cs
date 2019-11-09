namespace house_inviter
{
  public class ComboboxItem
  {
    public string Text { get; set; }

    public object Value { get; set; }

    public override string ToString()
    {
      return this.Text;
    }
  }
}
