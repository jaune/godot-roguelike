namespace Simulation
{
  public class Position {
    public int x;
    public int y;

    public Position() {
      this.x = 0;
      this.y = 0;
    }

    public Position(Position pos) {
      this.x = pos.x;
      this.y = pos.y;
    }

    public Position(int x, int y) {
      this.x = x;
      this.y = y;
    }

    public void Set(int x, int y) {
      this.x = x;
      this.y = y;
    }

    public bool Equals(Position other) {
      return other.x == this.x && other.y == this.y;
    }

    public override string ToString() {
      return $"Position({this.x},{this.y})";
    }

    public Position Project(CardinalDirection d) {
      var copy = new Position(this);

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
