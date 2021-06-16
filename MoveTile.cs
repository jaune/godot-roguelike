using Godot;
using System;

public enum Direction {
  None = 0,
  North = 0x1,
  NorthEast = 0x2,
  East = 0x3,
  SouthEast = 0x4,

  South = 0x5,
  SouthWest = 0x6,
  West = 0x7,
  NorthWest = 0x8,
}

public class MoveTile : Node2D
{
  [Export(PropertyHint.Enum)]
  public Direction direction = Direction.None;

  private ColorRect rect;

  private Sprite icon;

  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    rect = GetNode<ColorRect>("./Rect");

    rect.Connect("mouse_entered", this, nameof(_on_ColorRect_mouse_entered));
    rect.Connect("mouse_exited", this, nameof(_on_ColorRect_mouse_exited));

    icon = GetNode<Sprite>("./move-icon");
  }

  public void _on_ColorRect_mouse_entered () {
    rect.Color = new Color(1, 0, 1, 0.30f);
    icon.Visible = true;
    // Input.SetCustomMouseCursor()
  }


  public void _on_ColorRect_mouse_exited () {
    rect.Color = new Color(1, 0, 1, 0.12f);
    icon.Visible = false;
  }

  public override void _Input (InputEvent @event) {
    if (@event is InputEventMouseButton && @event.IsPressed() && icon.Visible) {
      GD.Print(" ==#== ", direction);
    }
  }
}
