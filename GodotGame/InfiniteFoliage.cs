using Godot;

public class InfiniteFoliage : Node2D
{
  private StreamTexture[] textures = new StreamTexture[0];

  private Sprite[] sprites = new Sprite[10];

  private RandomNumberGenerator rng = new RandomNumberGenerator();

  public override void _Ready()
  {
    textures = new StreamTexture[]{
      ResourceLoader.Load<StreamTexture>("res://assets/KenneyFoliage/foliagePack_001.png"),
      ResourceLoader.Load<StreamTexture>("res://assets/KenneyFoliage/foliagePack_002.png"),
      ResourceLoader.Load<StreamTexture>("res://assets/KenneyFoliage/foliagePack_003.png"),
      ResourceLoader.Load<StreamTexture>("res://assets/KenneyFoliage/foliagePack_004.png"),
      ResourceLoader.Load<StreamTexture>("res://assets/KenneyFoliage/foliagePack_019.png"),
    };

    rng.Seed = (ulong)Position.x + (ulong)Position.y;

    for (var i = 0; i < sprites.Length; i ++) {
      var s = new Sprite();
      s.Texture = textures[rng.RandiRange(0, textures.Length - 1)];

      s.Position = new Vector2(
        rng.RandfRange(-512.0f, 512.0f),
        rng.RandfRange(-512.0f, 512.0f)
      );

      sprites[i] = s;

      AddChild(s);
    }

    var tile = GetCurrentTile();
  }

  Vector2? currentTile = null;

  private Vector2 GetCurrentTile () {
    var screenPosition = (GetViewportTransform() * GetGlobalTransform()).Xform(Position);
    var tile = (screenPosition.Round() / 512).Round();

    return tile;
  }

  public override void _Process(float delta)
  {
    var tile = GetCurrentTile();

    if (currentTile == null || currentTile != tile) {
      // GD.Print(tile);
      currentTile = tile;
    }
  }
}
