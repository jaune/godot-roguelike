using Godot;
using System;

public class HoverArea : ColorRect
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
      this.Connect("mouse_entered", this, nameof(_on_ColorRect_mouse_entered));
      this.Connect("mouse_exited", this, nameof(_on_ColorRect_mouse_exited));
    }

    public void _on_ColorRect_mouse_entered () {
      GetNode<ColorRect>(".").Color = new Color(1, 0, 1, 0.30f);
      // Input.SetCustomMouseCursor()
    }


    public void _on_ColorRect_mouse_exited () {
      GetNode<ColorRect>(".").Color = new Color(1, 0, 1, 0.12f);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
