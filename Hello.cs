using Godot;
using System;

public class Hello : Node2D
{
  // Declare member variables here. Examples:
  private int counter = 0;
  // private string b = "text";

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {

  }

  public override void _UnhandledInput(InputEvent @event)
  {
    var subject = GetNode<Node2D>("GameLayer/Kenney");

    if (@event is InputEventKey eventKey)
      if (eventKey.Pressed && eventKey.Scancode == (uint)KeyList.Up) {
        subject.Translate(new Vector2(0, -32));
      }
      else if (eventKey.Pressed && eventKey.Scancode == (uint)KeyList.Down) {
        subject.Translate(new Vector2(0, 32));
      }
      else if (eventKey.Pressed && eventKey.Scancode == (uint)KeyList.Right) {
        subject.Translate(new Vector2(32, 0));
      }
      else if (eventKey.Pressed && eventKey.Scancode == (uint)KeyList.Left) {
        subject.Translate(new Vector2(-32, 0));
      }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
