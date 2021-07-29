namespace Simulation
{
  public class Location {
    public int x;
    public int y;
    public readonly Map map;

    public Location(Location pos) {
      this.x = pos.x;
      this.y = pos.y;
      this.map = pos.map;
    }

    public Location(Map map, int x, int y) {
      this.x = x;
      this.y = y;
      this.map = map;
    }

    public bool IsWalkable() {
      return this.map.IsWalkable(x, y);
    }

    public bool Equals(Location other) {
      return (other.x == this.x) && (other.y == this.y) && (other.map == this.map);
    }

    public override string ToString() {
      return $"Location({x}, {y}, {map})";
    }

    public Location Project(CardinalDirection d) {
      var copy = new Location(this);

      copy.Move(d);

      return copy;
    }

    public void Move(CardinalDirection d) {
      switch (d) {
        case CardinalDirection.North:
          this.y -= 1;
          break;
        case CardinalDirection.NorthEast:
          this.x += 1;
          this.y -= 1;
          break;
        case CardinalDirection.East:
          this.x += 1;
          break;
        case CardinalDirection.SouthEast:
          this.x += 1;
          this.y += 1;
          break;
        case CardinalDirection.South:
          this.y += 1;
          break;
        case CardinalDirection.SouthWest:
          this.x -= 1;
          this.y += 1;
          break;
        case CardinalDirection.West:
          this.x -= 1;
          break;
        case CardinalDirection.NorthWest:
          this.x -= 1;
          this.y -= 1;
          break;
      }
    }
  }
}
