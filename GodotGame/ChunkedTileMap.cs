using Godot;
using System;

public class ChunkedTileMap
{
  public class ChunkMetadata {
    public Vector2i Position;
    public string ScenePath;

    public ChunkMetadata(Vector2i pos, string path) {
      ScenePath = path;
      Position = pos;
    }
  }

  public Vector2i TilePixelSize = new Vector2i(64, 64);
  public Vector2i ChunkTileSize = new Vector2i(32, 32);
  public ChunkMetadata[] Chunks = new ChunkMetadata[0];

  public Vector2i ChunkPixelSize {
    get {
      return TilePixelSize * ChunkTileSize;
    }
  }

  public ChunkMetadata? Find(Vector2i pos) {
    var index = Array.FindIndex(Chunks, (c) => c.Position.Equals(pos));

    if (index == -1) {
      return null;
    }

    return Chunks[index];
  }

  public override string ToString() {
    return $"TilePixelSize: {TilePixelSize}\nChunkTileSize: {ChunkTileSize}\n Chunks: [{Chunks.Length}]";
  }
}
