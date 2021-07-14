using Godot;

public class HealthParticule : Node2D
{
  private int _Value;

  public int Value {
    get { return _Value; }
    set {
      _Value = value;
      GetNode<Label>("./Label").Text = value.ToString();
    }
  }


  public void _AnimationEnd()
  {
    GetParent().RemoveChild(this);
    Dispose();
  }
}
