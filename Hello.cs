using Godot;
using System;



public class Hello : Node2D
{
  const int TILE_SIZE = 96;
  Tween tween = new Tween();

  // Declare member variables here. Examples:
  private int counter = 0;
  // private string b = "text";

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    var self = GetNode<Node2D>(".");

    var kenneyPackedScene = ResourceLoader.Load<PackedScene>("res://Kenney.tscn");

    var kenney = kenneyPackedScene.Instance<Node2D>();

    kenney.Position = new Vector2(200, 336);

    self.AddChild(kenney);

    self.AddChild(tween);

    var move = GetNode<Node2D>("./Player/DefaultActions");

    move.Connect("OnCommand", this, nameof(__onCommand));
  }

  private void __onMoveCommand(MoveCommand command) {
    var player = GetNode<Node2D>("./Player");

    var destination = new Vector2(player.Position);

    switch (command.direction) {
      case CardinalDirection.North:
        destination.y -= TILE_SIZE;
        break;
      case CardinalDirection.NorthEast:
        destination.x += TILE_SIZE;
        destination.y -= TILE_SIZE;
        break;
      case CardinalDirection.East:
        destination.x += TILE_SIZE;
        break;
      case CardinalDirection.SouthEast:
        destination.x += TILE_SIZE;
        destination.y += TILE_SIZE;
        break;
      case CardinalDirection.South:
        destination.y += TILE_SIZE;
        break;
      case CardinalDirection.SouthWest:
        destination.x -= TILE_SIZE;
        destination.y += TILE_SIZE;
        break;
      case CardinalDirection.West:
        destination.x -= TILE_SIZE;
        break;
      case CardinalDirection.NorthWest:
        destination.x -= TILE_SIZE;
        destination.y -= TILE_SIZE;
        break;
    }

    tween.InterpolateProperty(player, "position", player.Position, destination.Round(), 0.3f);
    tween.Start();
  }

  public void __onCommand (Command cmd) {
    if (tween.IsActive()) {
      return;
    }

    if (cmd is MoveCommand) {
      __onMoveCommand((MoveCommand)cmd);
    }
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    // var subject = GetNode<Node2D>("GameLayer/Kenney");

    // if (@event is InputEventKey eventKey)
    //   if (eventKey.Pressed && eventKey.Scancode == (uint)KeyList.Up) {
    //     subject.Translate(new Vector2(0, -32));
    //   }
    //   else if (eventKey.Pressed && eventKey.Scancode == (uint)KeyList.Down) {
    //     subject.Translate(new Vector2(0, 32));
    //   }
    //   else if (eventKey.Pressed && eventKey.Scancode == (uint)KeyList.Right) {
    //     subject.Translate(new Vector2(32, 0));
    //   }
    //   else if (eventKey.Pressed && eventKey.Scancode == (uint)KeyList.Left) {
    //     subject.Translate(new Vector2(-32, 0));
    //   }
  }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
