using Godot;
using System.Collections.Generic;

public class ChunkedTileMapNode : Node2D
{
  class Chunk {
    public Vector2i Position;
    public Node2D? Container = null;

    public Chunk(Vector2i position) {
      Position = position;
    }
  }

  string Path = "res://maps/Test";
  ChunkedTileMap? ChunkedTileMap = null;
  Vector2i CurrentPosition = new Vector2i(0, 0);

  Vector2 GetChunksPerViewport(ChunkedTileMap map) {
    return (GetViewport().Size / map.ChunkPixelSize);
  }

  private Vector2i GetCurrentChunk (ChunkedTileMap map) {
    var screenPosition = (GetViewportTransform() * GetGlobalTransform()).Xform(Position);
    var chunk = (screenPosition.Round() / map.ChunkPixelSize).Round();

    return new Vector2i(Mathf.RoundToInt(-chunk.x), Mathf.RoundToInt(-chunk.y));
  }

  public override void _Ready()
  {
    ChunkedTileMap = new ChunkedTileMap(MapMetadataLoader.Load(Path));

    UpdateCurrentPosition(ChunkedTileMap, GetCurrentChunk(ChunkedTileMap));
  }

  public override void _Process(float delta) {
    if (ChunkedTileMap != null) {
      var pos = GetCurrentChunk(ChunkedTileMap);

      if (!CurrentPosition.Equals(pos)) {
        UpdateCurrentPosition(ChunkedTileMap, pos);
      }
    }
  }

  List<Chunk> Chunks = new List<Chunk>();

  private Chunk AddChunk(ChunkedTileMap map, Vector2i pos) {
    // GD.Print(
    //   "=== add chunk (", pos.x, ",", pos.y, ") ==="
    // );

    var chunk = new Chunk(pos);

    var container = new Node2D();
    container.Position = new Vector2(map.ChunkPixelSize * pos);

    var scn = map.LoadScene(pos);

    if (scn != null) {
      container.AddChild(scn);
    }

    var label = new Label();
    label.Text = $"({pos.x}, {pos.y})";
    container.AddChild(label);

    chunk.Container = container;

    AddChild(container);

    Chunks.Add(chunk);

    return chunk;
  }

  private Chunk EnsureChunk(ChunkedTileMap map, Vector2i pos) {
    var c = Chunks.Find((c) => { return c.Position.Equals(pos); });

    if (c == null) {
      c = AddChunk(map, pos);
    }

    return c;
  }

  private void RemoveChunk(Chunk c) {
    // GD.Print(
    //   "=== remove chunk (", c.Position.x, ",", c.Position.y, ") ==="
    // );
    RemoveChild(c.Container);
    c.Container?.Dispose();
    Chunks.Remove(c);
  }

  private void UpdateCurrentPosition(ChunkedTileMap map, Vector2i pos) {
    // GD.Print(
    //   "=== update current position (", pos.x, ",", pos.y, ") === \n"
    // );

    CurrentPosition = pos;
    var count = GetChunksPerViewport(map).Ceil();

    var minX = CurrentPosition.x - ((int)count.x * 2);
    var maxX = CurrentPosition.x + ((int)count.x * 2);

    var minY = CurrentPosition.y - ((int)count.y * 2);
    var maxY = CurrentPosition.y + ((int)count.y * 2);

    var r = new Rect2i(minX, minY, maxX - minX, maxY - minY);

    // GD.Print(
    //   "=== ensure chunks === \n",
    //   r
    // );

    for (int x = minX; x < maxX; x++) {
      for (int y = minY; y < maxY; y++) {
        EnsureChunk(map, new Vector2i(
          x,
          y
        ));
      }
    }

    var chached = r.Grow(1);

    // GD.Print(
    //   "=== chached chunks === \n",
    //   chached
    // );

    var disposable = new List<Chunk>();

    foreach (var c in Chunks) {
      if (!chached.HasPoint(c.Position)) {
        disposable.Add(c);
      }
    }

    foreach (var c in disposable) {
      RemoveChunk(c);
    }
  }
}

