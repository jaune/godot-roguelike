using Godot;
using System.Text.Json;

public class JsonStringException: System.Exception {
}

static public class JsonString {
  public static Vector2i ToVector2i(JsonElement el) {
    return new Vector2i(el.GetProperty("x").GetInt32(), el.GetProperty("y").GetInt32());
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

