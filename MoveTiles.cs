using Godot;

public class MoveTiles : Node2D
{
    private PackedScene? tileScene;

    [Signal]
    public delegate void OnCommand(Command cmd);

    public override void _Ready()
    {
      tileScene = GD.Load<PackedScene>("./MoveTile.tscn");

      AddTile(CardinalDirection.North, new Vector2(0, -96));
      AddTile(CardinalDirection.NorthEast, new Vector2(96, -96));
      AddTile(CardinalDirection.South, new Vector2(0, 96));
      AddTile(CardinalDirection.SouthEast, new Vector2(96, 96));
      AddTile(CardinalDirection.East, new Vector2(96, 0));
      AddTile(CardinalDirection.SouthWest, new Vector2(-96, 96));
      AddTile(CardinalDirection.West, new Vector2(-96, 0));
      AddTile(CardinalDirection.NorthWest, new Vector2(-96, -96));
    }

    public void __onCommand(Command cmd) {
      EmitSignal("OnCommand", cmd);
    }

    private Node? AddTile(CardinalDirection direction, Vector2 position) {
      if (tileScene == null) {
        return null;
      }

      var tile = tileScene.Instance<MoveTile>();

      tile.direction = direction;

      tile.Position = position;

      tile.Connect("OnCommand", this, nameof(__onCommand));

      this.AddChild(tile);

      return tile;
    }
}
