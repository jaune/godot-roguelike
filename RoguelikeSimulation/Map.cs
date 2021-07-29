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
  }

  public class Map {
    public enum WalkableValue : byte {
      Yes = 1,
      No = 2,
      Undefined = 0
    }

    public struct Chunk {
      public Vector2i Position;
      public WalkableValue[] Walkable;
    }

    public readonly string DisplayName;
    public readonly Vector2i ChunkSize;
    public readonly Chunk[] Chunks;

    public Map(string name, Vector2i chunkSize, Chunk[] chunks) {
      DisplayName = name;
      ChunkSize = chunkSize;
      Chunks = chunks;
    }

    public Location GetDefaultPlayerSpawnLocation() {
      return CreateLocation(0, 0);
    }

    public Location CreateLocation(int x, int y) {
      return new Location(this, x, y);
    }

    private Chunk? FindChunk(Vector2i pos) {
      var index = Array.FindIndex(Chunks, (c) => c.Position.Equals(pos));

      if (index == -1) {
        return null;
      }

      return Chunks[index];
    }

    public bool IsWalkable(int x, int y) {
      var p = new Vector2i(x / ChunkSize.x, y / ChunkSize.y);
      var l = new Vector2i(Math.Abs(x % ChunkSize.x), Math.Abs(y % ChunkSize.y));

      Console.WriteLine($"=== IsWalkable => {x}, {y}");

      var c = FindChunk(p);

      Console.WriteLine($"=== IsWalkable - chunk => {p.x}, {p.y} == {c != null}");

      if (c == null) {
        return false;
      }

      var index = (l.x) + (l.y * ChunkSize.x);

      Console.WriteLine($"=== IsWalkable - chunk=({p.x}, {p.y}) index={index} {c?.Walkable[index]}");

      return c?.Walkable[index] == WalkableValue.Yes;
    }

    public override string ToString() {
      return $"World({DisplayName})";
    }
  }
}
