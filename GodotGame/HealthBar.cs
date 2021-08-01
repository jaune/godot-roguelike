using Godot;

public class HealthBar : Node2D
{
  private int _CurrentHealth = 100;

  [Export(PropertyHint.Range, "1,10000,1")]
  public int CurrentHealth {
    get {
      return _CurrentHealth;
    }
    set {
      _CurrentHealth = value;
      UpdateHealthForeground();
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
      UpdateHealthForeground();
    }
  }

  private void UpdateHealthForeground() {
    var foreground = GetNode<ColorRect>("foreground");

    var ratio = Mathf.Max(0.0f, (float)_CurrentHealth / (float)_MaximumHealth);

    if (foreground != null) {
      if (ratio >= 1.0f) {
        Visible = false;
      }
      else {
        Visible = true;
        foreground.RectScale = new Vector2(ratio, 1.0f);
      }

    }
  }

  public override void _Ready()
  {
    UpdateHealthForeground();
  }

}
