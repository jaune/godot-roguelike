using Godot;
using System.Text.Json;

public static class SimulationMapLoader {
  private static Vector2i ToVector2i(JsonElement el) {
    return new Vector2i(el.GetProperty("x").GetInt32(), el.GetProperty("y").GetInt32());
  }

  private static Simulation.Vector2i ToSimulationVector2i(JsonElement el) {
    return new Simulation.Vector2i(el.GetProperty("x").GetInt32(), el.GetProperty("y").GetInt32());
  }

  private static MapMetadata.Chunk[] ToChunks(JsonElement el) {
    var arr = el.GetProperty("Chunks");
    var len = arr.GetArrayLength();

    var chunks = new MapMetadata.Chunk[len];

    var i = 0;
    foreach (var item in arr.EnumerateArray()) {
      chunks[i++] = new MapMetadata.Chunk{
        Position = ToVector2i(item.GetProperty("Position")),
      };
    }

    return chunks;
  }

  private static Simulation.Map.Chunk LoadChunk (MapMetadata meta, Vector2i pos) {
    var path = $"{meta.BasePath}/chunk.{pos.x}.{pos.y}.json";
    var root = JsonDocumentLoader.Load(path).RootElement;
    var position = ToSimulationVector2i(root.GetProperty("Position"));

    if (position.x != pos.x || position.y != pos.y) {
      throw new System.Exception("Mismatch position.");
    }

    var count = meta.ChunkTileSize.x * meta.ChunkTileSize.y;
    var data = root.GetProperty("Walkable");

    if (data.GetArrayLength() != count) {
      throw new System.Exception("Mismatch array size.");
    }

    var walkable = new Simulation.Map.WalkableValue[count];

    var i = 0;
    foreach (var item in data.EnumerateArray()) {
      switch (item.GetByte()) {
        case 1:
          walkable[i++] = Simulation.Map.WalkableValue.Yes;
          break;
        case 2:
          walkable[i++] = Simulation.Map.WalkableValue.No;
          break;
        default:
          walkable[i++] = Simulation.Map.WalkableValue.Undefined;
          break;
      }
    }

    return new Simulation.Map.Chunk{
      Position = position,
      Walkable = walkable
    };
  }

  public static Simulation.Map Load (MapMetadata meta) {
    Simulation.Vector2i chunkSize = new Simulation.Vector2i(
      meta.ChunkTileSize.x,
      meta.ChunkTileSize.y
    );

    var chunks = new Simulation.Map.Chunk[meta.Chunks.Length];

    for (var idx = 0; idx < meta.Chunks.Length; idx++) {
      var pos = meta.Chunks[idx].Position;

      chunks[idx] = LoadChunk(meta, pos);
    }

    return new Simulation.Map(meta.DisplayName, chunkSize, chunks);
  }
}
