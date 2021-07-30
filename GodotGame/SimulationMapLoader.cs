using Godot;
using System.Text.Json;

public static class SimulationMapLoader {
  private static Simulation.Vector2i ToVector2i(JsonElement el) {
    return new Simulation.Vector2i(el.GetProperty("x").GetInt32(), el.GetProperty("y").GetInt32());
  }

  private static Simulation.Map.WalkableValue[] ToWalkable(JsonElement el, int len) {
    if (el.GetArrayLength() != len) {
      throw new System.Exception("Mismatch array size.");
    }

    var walkable = new Simulation.Map.WalkableValue[len];

    for (var i = 0; i < len; i++) {
      switch (el[i].GetByte()) {
        case 2:
          walkable[i] = Simulation.Map.WalkableValue.Yes;
          break;
        case 1:
          walkable[i] = Simulation.Map.WalkableValue.No;
          break;
        default:
          walkable[i] = Simulation.Map.WalkableValue.Undefined;
          break;
      }
    }
    return walkable;
  }

  public static Simulation.Map Load (string path) {
    var doc = JsonDocumentLoader.Load(path).RootElement;

    Simulation.Vector2i chunkSize = ToVector2i(doc.GetProperty("ChunkSize"));

    var displayName = doc.GetProperty("DisplayName").GetString() ?? throw new System.Exception("DisplayName missing");

    var chunksEl = doc.GetProperty("Chunks");
    var len = chunksEl.GetArrayLength();

    var chunks = new Simulation.Map.Chunk[len];

    for (var idx = 0; idx < len; idx++) {
      chunks[idx] = new Simulation.Map.Chunk{
        Position = ToVector2i(chunksEl[idx].GetProperty("Position")),
        Walkable = ToWalkable(chunksEl[idx].GetProperty("Walkable"), chunkSize.x * chunkSize.y),
      };
    }

    var defaultPlayerSpawnEl = doc.GetProperty("DefaultPlayerSpawn");

    Simulation.Vector2i? defaultPlayerSpawn = (defaultPlayerSpawnEl.ValueKind == JsonValueKind.Null) ? null : ToVector2i(defaultPlayerSpawnEl);

    return new Simulation.Map(displayName, chunkSize, chunks, defaultPlayerSpawn);
  }
}
