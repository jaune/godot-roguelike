using Godot;

public class InGameMenu : Control
{
  [Signal]
  public delegate void RequestClose();

  public override void _UnhandledInput(InputEvent @event)
  {
    if(!Visible) {
      return;
    }

    if (@event is InputEventKey eventKey) {
      if (eventKey.Pressed) {
        switch (eventKey.Scancode) {
          case (int)KeyList.Escape:
            onRequestClose();
            GetTree().SetInputAsHandled();
            break;
        }
      }
    }
  }

  public void onRequestClose () {
    EmitSignal(nameof(RequestClose));
  }
}
