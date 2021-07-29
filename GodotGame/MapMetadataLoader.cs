using Godot;
using System.Text.Json;

public struct MapMetadata {
  public struct Chunk {
    public Vector2i Position;
  }

  public string DisplayName;
  public string MetadataPath;
  public string BasePath;
  public Vector2i TilePixelSize;
  public Vector2i ChunkTileSize;
  public Chunk[] Chunks;
}

public static class MapMetadataLoader {
  private static Vector2i ToVector2i(JsonElement el) {
    return new Vector2i(el.GetProperty("x").GetInt32(), el.GetProperty("y").GetInt32());
  }

  private static MapMetadata.Chunk[] ToChunks(JsonElement el) {
    var len = el.GetArrayLength();

    var chunks = new MapMetadata.Chunk[len];

    for (var i = 0; i < len; i++) {
      chunks[i] = new MapMetadata.Chunk{
        Position = ToVector2i(el[i].GetProperty("Position")),
      };
    }

    return chunks;
  }

  public static MapMetadata Load (string p) {
    var path = $"{p}/metadata.json";
    var doc = JsonDocumentLoader.Load(path).RootElement;

    return new MapMetadata{
      DisplayName = doc.GetProperty("DisplayName").GetString() ?? throw new JsonStringException(),
      MetadataPath = path,
      BasePath = p,
      ChunkTileSize = ToVector2i(doc.GetProperty("ChunkTileSize")),
      TilePixelSize = ToVector2i(doc.GetProperty("TilePixelSize")),
      Chunks = ToChunks(doc.GetProperty("Chunks"))
    };
  }
}
