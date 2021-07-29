using Godot;
using System;

public class ChunkedTileMap
{
  private MapMetadata meta;

  public ChunkedTileMap(MapMetadata meta) {
    this.meta = meta;
  }

  public Vector2i TilePixelSize {
    get {
      return this.meta.TilePixelSize;
    }
  }
  public Vector2i ChunkTileSize {
    get {
      return this.meta.ChunkTileSize;
    }
  }

  public Vector2i ChunkPixelSize {
    get {
      return TilePixelSize * ChunkTileSize;
    }
  }

  public Node? LoadScene(Vector2i pos) {
    var index = Array.FindIndex(this.meta.Chunks, (c) => c.Position.Equals(pos));

    if (index == -1) {
      return null;
    }

    var path = $"{this.meta.BasePath}/chunk.{pos.x}.{pos.y}.tscn";

    if (ResourceLoader.Exists(path)) {
      var chunkPackedScene = ResourceLoader.Load<PackedScene>(path, null, true);
      return chunkPackedScene.Instance();
    }

    // TODO: add fake data when missing
    GD.PrintErr($"ChunkedTileMap: Missing map chunk {path}");
    return null;
  }

  public override string ToString() {
    return $"TilePixelSize: {TilePixelSize}\nChunkTileSize: {ChunkTileSize}\n Chunks: [{meta.Chunks.Length}]";
  }
}
