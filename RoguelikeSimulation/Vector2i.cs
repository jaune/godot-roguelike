  using System;

namespace Simulation
{
  public class Vector2i {
    public int x;
    public int y;

    public Vector2i(int x, int y) {
      this.x = x;
      this.y = y;
    }

    public bool Equals (Vector2i o) {
      return o.x == x && o.y == y;
    }

    public static Vector2i operator -(Vector2i left, Vector2i right)
    {
        left.x -= right.x;
        left.y -= right.y;
        return left;
    }

    public static Vector2i operator -(Vector2i vec)
    {
        vec.x = -vec.x;
        vec.y = -vec.y;
        return vec;
    }
  }
}
