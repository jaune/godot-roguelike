using Godot;

public class MoveTile : Node2D
{
  [Export(PropertyHint.Enum)]
  public Simulation.CardinalDirection direction = Simulation.CardinalDirection.None;

  private ColorRect? rect;

  private Sprite? icon;


  public override void _Ready()
  {
    rect = GetNode<ColorRect>("./Rect");

    rect.Connect("mouse_entered", this, nameof(_on_ColorRect_mouse_entered));
    rect.Connect("mouse_exited", this, nameof(_on_ColorRect_mouse_exited));

    icon = GetNode<Sprite>("./move-icon");

  }

  public void _on_ColorRect_mouse_entered () {
    if (rect != null) {
      rect.Color = new Color(1, 0, 1, 0.30f);
    }
    if (icon != null) {
      icon.Visible = true;
    }
  }

  public void _on_ColorRect_mouse_exited () {
    if (rect != null) {
      rect.Color = new Color(1, 0, 1, 0.12f);
    }
    if (icon != null) {
      icon.Visible = false;
    }
  }

  // private bool hover = false;

  public override void _Input (InputEvent @event) {
    // if (@event is InputEventMouse) {
    //   var @ev = (InputEventMouse)@event;

    //   // TODO cache?
    //   var aabox = new Rect2(this.Position, new Vector2(96, 96));
    //   var isInside = aabox.HasPoint(@ev.Position);

    //   // GD.Print("_Input && InputEventMouse", direction, @ev.Position, @ev.GlobalPosition, "yes !!!!");

    //   if (!hover && isInside) {
    //     hover = true;
    //     _on_ColorRect_mouse_entered();
    //   }
    //   else if (hover && !isInside) {
    //     hover = false;
    //     _on_ColorRect_mouse_exited();
    //   }
    // }


    if (@event is InputEventMouseButton) {
      var @btn = (InputEventMouseButton)@event;

      if (@btn.IsPressed() && icon != null && icon.Visible && @btn.ButtonIndex == (int)ButtonList.Right) {
        Simulation.SimulationSingleton.GetInstance().Execute(new Simulation.WalkCommand(direction));
      }
    }
  }
}
