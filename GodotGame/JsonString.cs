using Godot;
using System.Text.Json;

public class JsonStringException: System.Exception {
}

static public class JsonString {
  public static Vector2i ToVector2i(JsonElement el) {
    return new Vector2i(el.GetProperty("x").GetInt32(), el.GetProperty("y").GetInt32());
  }

  public static ChunkedTileMap ToChunkedTileMap(string str) {
    var doc = JsonDocument.Parse(str) ?? throw new JsonStringException();

    var map = new ChunkedTileMap();

    map.TilePixelSize = JsonString.ToVector2i(doc.RootElement.GetProperty("TilePixelSize"));
    map.ChunkTileSize = JsonString.ToVector2i(doc.RootElement.GetProperty("ChunkTileSize"));

    var arr = doc.RootElement.GetProperty("Chunks");
    var len = arr.GetArrayLength();

    map.Chunks = new ChunkedTileMap.ChunkMetadata[len];

    var i = 0;
    foreach (var item in arr.EnumerateArray()) {
      map.Chunks[i++] = new ChunkedTileMap.ChunkMetadata(
        JsonString.ToVector2i(item.GetProperty("Position")),
        item.GetProperty("ScenePath").GetString() ?? ""
      );
    }

    return map;
  }


  public static string ToJsonString(this ChunkedTileMap self) {
    return "{" +
      "\"TilePixelSize\": " + self.TilePixelSize.ToJsonString() + "," +
      "\"ChunkTileSize\": " + self.ChunkTileSize.ToJsonString() +
    "}";
  }

  public static string ToJsonString(this Vector2i self) {
    return "{" + "\"x\":" + self.x + "\"y\":" + self.y + "}";
  }
}

