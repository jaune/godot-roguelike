using Godot;
using Simulation;

public class MainGame : Node2D
{
  Vector2 TilePixelSize = new Vector2(32, 32);

  PackedScene? UnknownAppearancePackedScene = null;
  PackedScene? KenneyZombiePackedScene = null;
  PackedScene? KenneyHumanPackedScene = null;

  public override void _Ready()
  {
    UnknownAppearancePackedScene = ResourceLoader.Load<PackedScene>("res://appearances/UnknownAppearance.tscn");
    KenneyZombiePackedScene = ResourceLoader.Load<PackedScene>("res://appearances/KenneyZombie.tscn");
    KenneyHumanPackedScene = ResourceLoader.Load<PackedScene>("res://appearances/Kenney.tscn");

    var arrow = ResourceLoader.Load<Resource>("res://assets/arrow-cursor.png");

    Input.SetCustomMouseCursor(arrow, Input.CursorShape.Arrow, new Vector2(6, 0));


    var move = GetNode<Node2D>("./Player/DefaultActions");

    var sim = SimulationSingleton.SetInstance(createSimulation());

    var scene = ResourceLoader.Load<PackedScene>("res://maps/Test/Test0.tscn").Instance();

    var cellSize = scene.GetMeta("cell_size");

    if (cellSize != null && cellSize is Vector2) {
      TilePixelSize = (Vector2)cellSize;
    }

    AddChild(scene);
    MoveChild(scene, 0);

    synchronizeScene();

    sim.Subscribe(() => {
      GetTree().CallGroup("simulation.mutations.listener", "_OnMutations");
      interpolateScene();
    });
  }

  ActorAppearance InstanceActorAppearance(Simulation.Appearance a) {
    switch (a) {
      case Simulation.Appearance.Human:
        return KenneyHumanPackedScene?.Instance<Kenney>() ?? throw new System.NullReferenceException("KenneyHumanPackedScene");
      case Simulation.Appearance.Zombie:
        return KenneyZombiePackedScene?.Instance<KenneyZombie>() ?? throw new System.NullReferenceException("KenneyZombiePackedScene");
    }
    return UnknownAppearancePackedScene?.Instance<UnknownAppearance>() ?? throw new System.NullReferenceException("UnknownAppearance");
  }

  ActorAppearance InstanceActorAppearance(Simulation.Actor a) {
    ActorAppearance node = InstanceActorAppearance(a.Appearance);

    node.SetActor(a);
    node.Position = ProjectLocationToScenePosition(a.Location);
    node.AddToGroup("simulation.mutations.listener");

    return node;
  }

  Control InGameMenu {
    get {
      return GetNode<Control>("InGameMenu/InGameMenu");
    }
  }

  public void onInGameMenuRequestClose() {
    InGameMenu.Visible = false;
  }

  private void OpenInGameMenu() {
    InGameMenu.Visible = true;
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (InGameMenu.Visible) {
      return;
    }

    if (@event is InputEventKey eventKey) {
      if (eventKey.Pressed) {
        var sim = Simulation.SimulationSingleton.GetInstance();

        switch (eventKey.Scancode) {
          case (int)KeyList.Escape:
            OpenInGameMenu();
            GetTree().SetInputAsHandled();
            break;
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

  private Simulation.Simulation createSimulation() {
    var builder = new Simulation.Builder();

    var map = SimulationMapLoader.Load("res://maps/Test/Test0.simulation.json");

    var w = builder.AddMap(map);
    var sim = builder.Build();

    {
      var enemy = new Simulation.Actor("Enemy", w.CreateLocation(5, 5));

      enemy.Appearance = Simulation.Appearance.Zombie;

      sim.AddActor(enemy);
    }

    {
      var enemy = new Simulation.Actor("Enemy", w.CreateLocation(10, 10));

      enemy.CurrentHealth = 10;
      enemy.Appearance = Simulation.Appearance.Zombie;

      sim.AddActor(enemy);
    }

    {
      var enemy = new Simulation.Actor("Enemy", w.CreateLocation(-5, -5));

      enemy.CurrentHealth = 25;
      enemy.Appearance = Simulation.Appearance.Human;

      sim.AddActor(enemy);
    }

        {
      var enemy = new Simulation.Actor("Enemy", w.CreateLocation(-10, -10));

      enemy.CurrentHealth = 32;
      enemy.Appearance = Simulation.Appearance.Unknown;

      sim.AddActor(enemy);
    }

    return sim;
  }

  private Vector2 ProjectLocationToScenePosition(Simulation.Location loc) {
    return new Vector2(loc.x * TilePixelSize.x + (TilePixelSize.x / 2), loc.y * TilePixelSize.y + (TilePixelSize.y / 2));
  }

  private void synchronizeScene () {
    var sim = Simulation.SimulationSingleton.GetInstance();
    var p = sim.GetPlayer();

    var actors = sim.FindActorsNear(p);

    var player = GetNode<Node2D>("./Player");

    foreach (var a in actors) {
      if (a == p) {
        player.Position = ProjectLocationToScenePosition(a.Location);
      }
      else {
        AddChild(InstanceActorAppearance(a));
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
