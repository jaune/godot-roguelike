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
    if (@event is InputEventKey eventKey)
      if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape) {
        var label = new Label();

        label.Text = "Escape";
        label.RectPosition = new Vector2(0, counter+=30);

        GetNode(".").AddChild(label);
      }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
