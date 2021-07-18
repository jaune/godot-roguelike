using Godot;

public class MoveTiles : Node2D
{
    const int TILE_SIZE = 96;

    private PackedScene? tileScene;

    public override void _Ready()
    {
      tileScene = GD.Load<PackedScene>("./MoveTile.tscn");

      AddTile(Simulation.CardinalDirection.North, new Vector2(0, -TILE_SIZE));
      AddTile(Simulation.CardinalDirection.NorthEast, new Vector2(TILE_SIZE, -TILE_SIZE));
      AddTile(Simulation.CardinalDirection.South, new Vector2(0, TILE_SIZE));
      AddTile(Simulation.CardinalDirection.SouthEast, new Vector2(TILE_SIZE, TILE_SIZE));
      AddTile(Simulation.CardinalDirection.East, new Vector2(TILE_SIZE, 0));
      AddTile(Simulation.CardinalDirection.SouthWest, new Vector2(-TILE_SIZE, TILE_SIZE));
      AddTile(Simulation.CardinalDirection.West, new Vector2(-TILE_SIZE, 0));
      AddTile(Simulation.CardinalDirection.NorthWest, new Vector2(-TILE_SIZE, -TILE_SIZE));
    }

    private Node? AddTile(Simulation.CardinalDirection direction, Vector2 position) {
      if (tileScene == null) {
        return null;
      }

      var tile = tileScene.Instance<MoveTile>();

      tile.direction = direction;

      tile.Position = position;

      this.AddChild(tile);

      return tile;
    }
}
