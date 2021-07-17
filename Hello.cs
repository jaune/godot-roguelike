using Godot;

public class Hello : Node2D
{
  const int TILE_SIZE = 96;

  Simulation.Simulation sim = Simulation.Simulation.GetInstance();

  private bool busy = false;

  PackedScene? kenneyPackedScene = null;

  public override void _Ready()
  {
    var arrow = ResourceLoader.Load<Resource>("res://assets/arrow-cursor.png");

    Input.SetCustomMouseCursor(arrow, Input.CursorShape.Arrow, new Vector2(6, 0));

    kenneyPackedScene = ResourceLoader.Load<PackedScene>("res://Kenney.tscn");

    var move = GetNode<Node2D>("./Player/DefaultActions");

    move.Connect("OnCommand", this, nameof(__onCommand));

    initializeState();

    synchronizeScene();
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventKey eventKey) {
      if (eventKey.Pressed) {
        switch (eventKey.Scancode) {
          case (int)KeyList.Up:
            __onCommand(new DefaultCommand(Simulation.CardinalDirection.North));
            break;
          case (int)KeyList.Down:
            __onCommand(new DefaultCommand(Simulation.CardinalDirection.South));
            break;
          case (int)KeyList.Right:
            __onCommand(new DefaultCommand(Simulation.CardinalDirection.East));
            break;
          case (int)KeyList.Left:
            __onCommand(new DefaultCommand(Simulation.CardinalDirection.West));
            break;
        }
      }
    }
  }

  public void __onCommand (Command cmd) {
    if (busy) {
      return;
    }

    busy = true;

    var mutations = sim.Execute(cmd);

    GetTree().CallGroup("simulation.mutations.listener", "_OnMutations");

    interpolateScene();

    busy = false;
  }

  private void initializeState() {
    Simulation.State state = sim.GetState();

    {
      var enemy = new Simulation.Character("Enemy");

      enemy.Position.Set(5, 5);

      state.characters.Add(enemy);
    }

    {
      var enemy = new Simulation.Character("Enemy");

      enemy.Position.Set(8, 8);
      enemy.CurrentHealth = 10;

      state.characters.Add(enemy);
    }
  }

  private void synchronizeScene () {
    Simulation.State state = sim.GetState();

    var player = GetNode<Node2D>("./Player");

    if (kenneyPackedScene == null) {
      GD.PrintErr("kenneyPackedScene == null");
      return;
    }

    foreach (var c in state.characters) {
      if (c == state.player) {
        player.Position = new Vector2(c.Position.x * TILE_SIZE, c.Position.y * TILE_SIZE);
      }
      else {
        var kenney = kenneyPackedScene.Instance<Kenney>();

        kenney.Reference = c.Reference;
        kenney.Position = new Vector2(c.Position.x * TILE_SIZE, c.Position.y * TILE_SIZE);
        kenney.MaximumHealth = c.MaximumHealth;
        kenney.CurrentHealth = c.CurrentHealth;
        kenney.AddToGroup("simulation.mutations.listener");

        AddChild(kenney);

      }
    }
  }

  private void interpolateScene () {
    var player = GetNode<Node2D>("./Player");
    var tween = player.GetNode<Tween>("./MoveTween");

    Simulation.State state = sim.GetState();

    var destination = new Vector2(state.player.Position.x * TILE_SIZE, state.player.Position.y * TILE_SIZE);

    tween.InterpolateProperty(player, "position", player.Position, destination.Round(), 0.3f);
    tween.Start();
  }
}
