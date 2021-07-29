using Godot;
using Simulation;
using System.Collections.Generic;

public class MainGame : Node2D
{
  const int TILE_SIZE = 96;

  PackedScene? kenneyPackedScene = null;

  public override void _Ready()
  {
    var arrow = ResourceLoader.Load<Resource>("res://assets/arrow-cursor.png");

    Input.SetCustomMouseCursor(arrow, Input.CursorShape.Arrow, new Vector2(6, 0));

    kenneyPackedScene = ResourceLoader.Load<PackedScene>("res://Kenney.tscn");

    var move = GetNode<Node2D>("./Player/DefaultActions");

    var sim = SimulationSingleton.SetInstance(createSimulation());

    synchronizeScene();

    sim.Subscribe(() => {
      GetTree().CallGroup("simulation.mutations.listener", "_OnMutations");
      interpolateScene();
    });
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventKey eventKey) {
      if (eventKey.Pressed) {
        var sim = Simulation.SimulationSingleton.GetInstance();

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

  Dictionary<Simulation.Map, MapMetadata> MapMetadata_IndexedBy_SimulationMap = new Dictionary<Simulation.Map, MapMetadata>();

  private Simulation.Simulation createSimulation() {
    var builder = new Simulation.Builder();

    var meta = MapMetadataLoader.Load("res://maps/Test");
    var map = SimulationMapLoader.Load(meta);

    MapMetadata_IndexedBy_SimulationMap.Add(map, meta);

    var w = builder.AddMap(map);
    var sim = builder.Build();

    {
      var enemy = new Simulation.Actor("Enemy", w.CreateLocation(5, 5));

      sim.AddActor(enemy);
    }

    {
      var enemy = new Simulation.Actor("Enemy", w.CreateLocation(5, 5));

      enemy.CurrentHealth = 10;

      sim.AddActor(enemy);
    }

    return sim;
  }

  private Vector2 ProjectLocationToScenePosition(Simulation.Location loc) {
    var meta = MapMetadata_IndexedBy_SimulationMap[loc.map];

    return new Vector2(loc.x * meta.TilePixelSize.x, loc.y * meta.TilePixelSize.y);
  }

  private void synchronizeScene () {
    var sim = Simulation.SimulationSingleton.GetInstance();
    var p = sim.GetPlayer();

    var actors = sim.FindActorsNear(p);

    var player = GetNode<Node2D>("./Player");

    if (kenneyPackedScene == null) {
      GD.PrintErr("kenneyPackedScene == null");
      return;
    }

    foreach (var a in actors) {
      var newPos = ProjectLocationToScenePosition(a.Location);

      if (a == p) {
        player.Position = newPos;
      }
      else {
        var kenney = kenneyPackedScene.Instance<Kenney>();

        kenney.Reference = a.Reference;
        kenney.Position = newPos;
        kenney.MaximumHealth = a.MaximumHealth;
        kenney.CurrentHealth = a.CurrentHealth;
        kenney.AddToGroup("simulation.mutations.listener");

        AddChild(kenney);
      }
    }
  }

  private void interpolateScene () {
    var sim = Simulation.SimulationSingleton.GetInstance();
    var player = GetNode<Node2D>("./Player");
    var tween = player.GetNode<Tween>("./MoveTween");

    var p = sim.GetPlayer();
    var destination = ProjectLocationToScenePosition(p.Location);

    tween.InterpolateProperty(player, "position", player.Position, destination.Round(), 0.3f);
    tween.Start();
  }
}
