namespace Simulation
{
  class Rect2i {
    Vector2i Position;
    Vector2i Size;

    public Rect2i(Vector2i position, Vector2i size) {
      Position = position;
      Size = size;
    }

    public bool Contains(Vector2i point) {
      return (point.x >= Position.x) && (point.x <= (Position.x + Size.x)) && (point.y >= Position.y) && (point.y <= (Position.y + Size.y));
    }
  }
}
