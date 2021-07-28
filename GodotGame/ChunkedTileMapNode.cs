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
  ChunkedTileMap ChunkedTileMap = new ChunkedTileMap();
  Vector2i CurrentPosition = new Vector2i(0, 0);

  Vector2 ChunksPerViewport {
    get {
      return (GetViewport().Size / ChunkedTileMap.ChunkPixelSize);
    }
  }

  private Vector2i GetCurrentChunk () {
    var screenPosition = (GetViewportTransform() * GetGlobalTransform()).Xform(Position);
    var chunk = (screenPosition.Round() / ChunkedTileMap.ChunkPixelSize).Round();

    return new Vector2i(Mathf.RoundToInt(-chunk.x), Mathf.RoundToInt(-chunk.y));
  }

  public override void _Ready()
  {
    //  GD.Print(ChunkedTileMap.ToJsonString());

    var path = $"{Path}/index.json";

    var file = new File();

    if (file.Open(path, File.ModeFlags.Read) != Error.Ok) {
      GD.PrintErr($"Unable to read {path}");
      return;
    }

    var map = JsonString.ToChunkedTileMap(file.GetAsText());

    if (map == null) {
      GD.PrintErr($"Unable to deserialize {path}");
      return;
    }

    ChunkedTileMap = map;

    UpdateCurrentPosition(GetCurrentChunk());
  }

  public override void _Process(float delta) {
    var pos = GetCurrentChunk();

    if (!CurrentPosition.Equals(pos)) {
      UpdateCurrentPosition(pos);
    }
  }

  List<Chunk> Chunks = new List<Chunk>();

  private Chunk AddChunk(Vector2i pos) {
    // GD.Print(
    //   "=== add chunk (", pos.x, ",", pos.y, ") ==="
    // );

    var chunk = new Chunk(pos);

    var container = new Node2D();
    container.Position = new Vector2(ChunkedTileMap.ChunkPixelSize * pos);

    var metadata = ChunkedTileMap.Find(pos);

    if (metadata != null) {
      var path = $"{Path}/{metadata.ScenePath}";

      if (ResourceLoader.Exists(path)) {
        var chunkPackedScene = ResourceLoader.Load<PackedScene>(path, null, true);
        container.AddChild(chunkPackedScene.Instance());
      }
      else {
        // TODO: add fake data when missing
        GD.PrintErr($"ChunkedTileMap: Missing map chunk {path}");
      }
    }

    var label = new Label();
    label.Text = $"({pos.x}, {pos.y})";
    container.AddChild(label);

    chunk.Container = container;

    AddChild(container);

    Chunks.Add(chunk);

    return chunk;
  }

  private Chunk EnsureChunk(Vector2i pos) {
    var c = Chunks.Find((c) => { return c.Position.Equals(pos); });

    if (c == null) {
      c = AddChunk(pos);
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

  private void UpdateCurrentPosition(Vector2i pos) {
    // GD.Print(
    //   "=== update current position (", pos.x, ",", pos.y, ") === \n"
    // );

    CurrentPosition = pos;
    var count = ChunksPerViewport.Ceil();

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
        EnsureChunk(new Vector2i(
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

