using Godot;
using System;
using Simulation;

public class Kenney: ActorAppearance
{
  public Guid Reference = Guid.Empty;

  PackedScene? HealthParticulePackedScene = null;

  public override void _Ready()
  {
    HealthParticulePackedScene = ResourceLoader.Load<PackedScene>("res://HealthParticule.tscn");
  }

  private void TravelAnimationTreeTo(string name) {
    var animationTree = GetNode<AnimationTree>("AnimationTree");
    var playback = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
    playback.Travel(name);
  }

  public override void SetActor (Simulation.Actor a) {
    Reference = a.Reference;
    HealthBar.MaximumHealth = a.MaximumHealth;
    HealthBar.CurrentHealth = a.CurrentHealth;
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

  HealthBar HealthBar {
    get {
      return GetNode<HealthBar>("./health");
    }
  }

  public void _OnMutations() {
    if (Reference != Guid.Empty) {
      var mutations = SimulationSingleton.GetInstance().GetLastMutations();

      foreach (var mutation in mutations) {
        if (mutation is DefaultAttackMutation) {
          var m = (DefaultAttackMutation)mutation;

          if(m.Target.Reference == Reference) {
            HealthBar.CurrentHealth = m.Target.CurrentHealth;
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
