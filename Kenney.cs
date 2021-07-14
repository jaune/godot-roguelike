using Godot;
using System;

public class Kenney : Node2D
{
  public Guid? Reference = null;

  private int _CurrentHealth = 100;

  [Export(PropertyHint.Range, "1,10000,1")]
  public int CurrentHealth {
    get {
      return _CurrentHealth;
    }
    set {
      _CurrentHealth = value;
      UpdateForeground();
    }
  }

  private int _MaximumHealth = 100;

  [Export(PropertyHint.Range, "1,10000,1")]
  public int MaximumHealth {
    get {
      return _MaximumHealth;
    }
    set {
      _MaximumHealth = value;
      UpdateForeground();
    }
  }

  private void UpdateForeground() {
    var foreground = GetNode<ColorRect>("health/foreground");

    var ratio = (float)_CurrentHealth / (float)_MaximumHealth;

    if (foreground != null) {
      foreground.RectScale = new Vector2(ratio, 1.0f);
    }
  }

  public override void _Ready()
  {
    UpdateForeground();
  }

  public override void _EnterTree()
  {
    base._EnterTree();
  }

  public override void _ExitTree()
  {
    base._ExitTree();

  }
}
