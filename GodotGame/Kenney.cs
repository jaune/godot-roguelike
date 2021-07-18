using Godot;
using System;
using Simulation;

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
    var p = GetNode<Node2D>("health").Position;

    var particule = HealthParticulePackedScene?.Instance<HealthParticule>();

    if (particule != null) {
      particule.Value = damage;

      particule.Position = p + new Vector2(0, 15);

      AddChild(particule);
    }

    TravelAnimationTreeTo("hit");
  }

  public void _OnMutations() {
    if (Reference != Guid.Empty) {
      var mutations = SimulationSingleton.GetInstance().GetLastMutations();

      foreach (var mutation in mutations) {
        if (mutation is DefaultAttackMutation) {
          var m = (DefaultAttackMutation)mutation;

          if(m.Target.Reference == Reference) {
            CurrentHealth = m.Target.CurrentHealth;
            _OnHit(m.Damage);
          }
        }
        else if (mutation is DeathMutation) {
          var m = (DeathMutation)mutation;

          if (m.Subject.Reference == Reference) {
            _OnDeath();
          }
        }
      }
    }
  }
}
