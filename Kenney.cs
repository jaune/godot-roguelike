using Godot;
using System;

public class Kenney : Node2D
{
  public Guid Reference = Guid.Empty;

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
    var foreground = GetNode<ColorRect>("health/foreground");

    var ratio = Mathf.Max(0.0f, (float)_CurrentHealth / (float)_MaximumHealth);

    if (foreground != null) {
      foreground.RectScale = new Vector2(ratio, 1.0f);
    }
  }

  PackedScene? HealthParticulePackedScene = null;

  public override void _Ready()
  {
    HealthParticulePackedScene = ResourceLoader.Load<PackedScene>("res://HealthParticule.tscn");


    UpdateHealthForeground();
  }

  private void TravelAnimationTreeTo(string name) {
    var animationTree = GetNode<AnimationTree>("AnimationTree");
    var playback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    playback.Travel(name);
  }

  private void _OnDeath() {
    TravelAnimationTreeTo("death");
  }

  private void _OnHit(int damage) {
    var particule = HealthParticulePackedScene?.Instance<HealthParticule>();

    if (particule != null) {
      particule.Value = damage;

      AddChild(particule);
    }

    TravelAnimationTreeTo("hit");
  }

  public void _OnMutations() {
    if (Reference != Guid.Empty) {
      var c = Simulation.Simulation.GetInstance().QueryCharacterByReference(Reference);

      if (c != null) {
        var delta = c.CurrentHealth - _CurrentHealth;

        if (delta < 0) {
          _OnHit(Math.Abs(Mathf.RoundToInt(delta)));

          if (c.CurrentHealth <= 0) {
            _OnDeath();
          }
        }
        if (delta != 0) {
          _CurrentHealth = c.CurrentHealth;
          _MaximumHealth = c.MaximumHealth;
          UpdateHealthForeground();
        }
      }
    }
  }
}
