using Godot;
using System;

public class MainMenu : Control
{
  public void onQuitPressed() {
    GetTree().Notification(MainLoop.NotificationWmQuitRequest);
  }

}
