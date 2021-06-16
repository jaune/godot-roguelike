using Godot;
using System;

public class MoveTiles : Node2D
{
    private PackedScene tileScene;

    public override void _Ready()
    {
      tileScene = GD.Load<PackedScene>("./MoveTile.tscn");

      AddTile(Direction.North, new Vector2(0, -96));
      AddTile(Direction.NorthEast, new Vector2(96, -96));
      AddTile(Direction.South, new Vector2(0, 96));
      AddTile(Direction.SouthEast, new Vector2(96, 96));
      AddTile(Direction.East, new Vector2(96, 0));
      AddTile(Direction.SouthWest, new Vector2(-96, 96));
      AddTile(Direction.West, new Vector2(-96, 0));
      AddTile(Direction.NorthWest, new Vector2(-96, -96));
    }

    private Node AddTile(Direction direction, Vector2 position) {
      var tile = tileScene.Instance<MoveTile>();

      tile.direction = direction;

      tile.Position = position;

      this.AddChild(tile);

      return tile;
    }
}
