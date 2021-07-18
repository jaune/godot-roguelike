using Godot;
using Simulation;


public class Hello : Node2D
{
  const int TILE_SIZE = 96;

  Simulation.Simulation sim = SimulationSingleton.GetInstance();

  PackedScene? kenneyPackedScene = null;

  public override void _Ready()
  {
    var arrow = ResourceLoader.Load<Resource>("res://assets/arrow-cursor.png");

    Input.SetCustomMouseCursor(arrow, Input.CursorShape.Arrow, new Vector2(6, 0));

    kenneyPackedScene = ResourceLoader.Load<PackedScene>("res://Kenney.tscn");

    var move = GetNode<Node2D>("./Player/DefaultActions");

    initializeState();

    synchronizeScene();

    SimulationSingleton.GetInstance().Subscribe(() => {
      GetTree().CallGroup("simulation.mutations.listener", "_OnMutations");
      interpolateScene();
    });
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventKey eventKey) {
      if (eventKey.Pressed) {
        switch (eventKey.Scancode) {
          case (int)KeyList.Up:
            sim.Execute(new DefaultCommand(Simulation.CardinalDirection.North));
            break;
          case (int)KeyList.Down:
            sim.Execute(new DefaultCommand(Simulation.CardinalDirection.South));
            break;
          case (int)KeyList.Right:
            sim.Execute(new DefaultCommand(Simulation.CardinalDirection.East));
            break;
          case (int)KeyList.Left:
            sim.Execute(new DefaultCommand(Simulation.CardinalDirection.West));
            break;
        }
      }
    }
  }

  private void initializeState() {
    {
      var enemy = new Simulation.Actor("Enemy");

      enemy.Position.Set(5, 5);

      sim.AddActor(enemy);
    }

    {
      var enemy = new Simulation.Actor("Enemy");

      enemy.Position.Set(8, 8);
      enemy.CurrentHealth = 10;

      sim.AddActor(enemy);
    }
  }

  private void synchronizeScene () {
    var p = sim.GetPlayer();

    var actors = sim.FindActorsNear(p);

    var player = GetNode<Node2D>("./Player");

    if (kenneyPackedScene == null) {
      GD.PrintErr("kenneyPackedScene == null");
      return;
    }

    foreach (var a in actors) {
      if (a == p) {
        player.Position = new Vector2(a.Position.x * TILE_SIZE, a.Position.y * TILE_SIZE);
      }
      else {
        var kenney = kenneyPackedScene.Instance<Kenney>();

        kenney.Reference = a.Reference;
        kenney.Position = new Vector2(a.Position.x * TILE_SIZE, a.Position.y * TILE_SIZE);
        kenney.MaximumHealth = a.MaximumHealth;
        kenney.CurrentHealth = a.CurrentHealth;
        kenney.AddToGroup("simulation.mutations.listener");

        AddChild(kenney);

      }
    }
  }

  private void interpolateScene () {
    var player = GetNode<Node2D>("./Player");
    var tween = player.GetNode<Tween>("./MoveTween");

    var p = sim.GetPlayer();
    var destination = new Vector2(p.Position.x * TILE_SIZE, p.Position.y * TILE_SIZE);

    tween.InterpolateProperty(player, "position", player.Position, destination.Round(), 0.3f);
    tween.Start();
  }
}
