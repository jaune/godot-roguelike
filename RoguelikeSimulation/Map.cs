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
      Yes = 2,
      No = 1,
      Undefined = 0
    }

    public struct Chunk {
      public Vector2i Position;
      public WalkableValue[] Walkable;
    }

    public readonly string DisplayName;
    public readonly Vector2i ChunkSize;
    public readonly Chunk[] Chunks;
    public readonly Vector2i? DefaultPlayerSpawn;

    public Map(string name, Vector2i chunkSize, Chunk[] chunks, Vector2i? defaultPlayerSpawn = null) {
      DisplayName = name;
      ChunkSize = chunkSize;
      Chunks = chunks;
      DefaultPlayerSpawn = defaultPlayerSpawn;
    }

    public Location? FindDefaultPlayerSpawnLocation() {
      return DefaultPlayerSpawn != null ? CreateLocation(DefaultPlayerSpawn.x, DefaultPlayerSpawn.y) : null;
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
      var p = new Vector2i(
        (int)Math.Floor((float)x / (float)ChunkSize.x) * ChunkSize.x,
        (int)Math.Floor((float)y / (float)ChunkSize.y) * ChunkSize.y
      );

      Console.WriteLine($"=== IsWalkable => {x}, {y}");

      var c = FindChunk(p);

      if (c == null) {
        return false;
      }

      var chk = c ?? throw new Exception();

      var l = new Vector2i(x - p.x, y - p.y);

      var index = (l.x) + (l.y * ChunkSize.x);

      Console.WriteLine($"=== IsWalkable - absolute({x}, {y}) -- chunk({p.x}, {p.y}) -- relative({l.x}, {l.y}) -- {index} {chk.Walkable[index]}");

      return chk.Walkable[index] == WalkableValue.Yes;
    }

    public override string ToString() {
      return $"World({DisplayName})";
    }
  }
}
