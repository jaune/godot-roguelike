using Godot;
using System.Collections.Generic;

class Simulation {
  public class Position {
    public int x;
    public int y;

    public Position() {
      this.x = 0;
      this.y = 0;
    }

    public Position(Position pos) {
      this.x = pos.x;
      this.y = pos.y;
    }

    public void set(int x, int y) {
      this.x = x;
      this.y = y;
    }
  }

  public class Character {
    public Position position;

    public Character(){
      this.position = new Position();
    }
  }

  public class State {
    public Character player;
    public List<Character> characters;

    public State() {
      this.player = new Character();
      this.characters = new List<Character>(1);

      this.characters.Add(this.player);
    }
  }

  private State state;

  public State getState() {
    return state;
  }

  public Simulation() {
    var state = new State();

    this.state = state;
  }

  private void executeMoveCommand (MoveCommand cmd) {
    var destination = new Position(state.player.position);

    switch (cmd.direction) {
      case CardinalDirection.North:
        destination.y -= 1;
        break;
      case CardinalDirection.NorthEast:
        destination.x += 1;
        destination.y -= 1;
        break;
      case CardinalDirection.East:
        destination.x += 1;
        break;
      case CardinalDirection.SouthEast:
        destination.x += 1;
        destination.y += 1;
        break;
      case CardinalDirection.South:
        destination.y += 1;
        break;
      case CardinalDirection.SouthWest:
        destination.x -= 1;
        destination.y += 1;
        break;
      case CardinalDirection.West:
        destination.x -= 1;
        break;
      case CardinalDirection.NorthWest:
        destination.x -= 1;
        destination.y -= 1;
        break;
    }

    state.player.position.x = destination.x;
    state.player.position.y = destination.y;
  }

  public void execute(Command cmd) {
      if (cmd is MoveCommand) {
      executeMoveCommand((MoveCommand)cmd);
    }
  }
}

public class Hello : Node2D
{
  const int TILE_SIZE = 96;

  Simulation sim = new Simulation();

  private bool busy = false;

  public override void _Ready()
  {
    var arrow = ResourceLoader.Load<Resource>("res://assets/arrow-cursor.png");

    Input.SetCustomMouseCursor(arrow, Input.CursorShape.Arrow, new Vector2(6, 0));

    var self = GetNode<Node2D>(".");

    var kenneyPackedScene = ResourceLoader.Load<PackedScene>("res://Kenney.tscn");

    var kenney = kenneyPackedScene.Instance<Node2D>();

    kenney.Position = new Vector2(200, 336);

    self.AddChild(kenney);

    var move = GetNode<Node2D>("./Player/DefaultActions");

    move.Connect("OnCommand", this, nameof(__onCommand));

    initializeState();

    synchronizeScene();
  }

  public void __onCommand (Command cmd) {
    if (busy) {
      return;
    }

    busy = true;

    sim.execute(cmd);

    interpolateScene();

    busy = false;
  }

  private void initializeState() {
    Simulation.State state = sim.getState();

    var enemy = new Simulation.Character();

    enemy.position.set(5, 5);

    state.characters.Add(enemy);
  }

  private void synchronizeScene () {
    Simulation.State state = sim.getState();

    var player = GetNode<Node2D>("./Player");

    player.Position = new Vector2(state.player.position.x * TILE_SIZE, state.player.position.y * TILE_SIZE);

    // foreach (var c in state.characters) {
    //   var node = GetNode<Node2D>("./Player");

    //   node.Position = new Vector2(c.position.x * TILE_SIZE, c.position.y * TILE_SIZE);
    // }
  }

  private void interpolateScene () {
    var player = GetNode<Node2D>("./Player");
    var tween = player.GetNode<Tween>("./MoveTween");

    Simulation.State state = sim.getState();

    var destination = new Vector2(state.player.position.x * TILE_SIZE, state.player.position.y * TILE_SIZE);

    tween.InterpolateProperty(player, "position", player.Position, destination.Round(), 0.3f);
    tween.Start();
  }
}
