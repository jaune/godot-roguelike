using Godot;

class NavigationGridDebugNode : Node2D {
  const int TILE_SIZE = 96;

  [Signal]
  public delegate void OnCommand(Command cmd);

  public void __onCommand(Command cmd) {
    EmitSignal("OnCommand", cmd);
  }

  NavigationGrid? grid = null;

  public override void _Ready()
  {
    // var plainArrow = ResourceLoader.Load<Texture>("res://assets/plain-arrow.png");
    // var charTex = ResourceLoader.Load<Texture>("res://assets/character.png");
    // var haltTex = ResourceLoader.Load<Texture>("res://assets/halt.png");
    // var helpTex = ResourceLoader.Load<Texture>("res://assets/help.png");

    // grid = new NavigationGrid(10);

    // Sprite[] sprites = new Sprite[grid.size * grid.size];

    // var half = (grid.size / 2);
    // var centerIndex = half + (half * grid.size);

    // grid.blocked[5 + (half * grid.size)] = true;

    // grid.compute();

    // float pxSize = ((float)grid.size * (float)TILE_SIZE) / 2.0f;

    // Position = new Vector2(-pxSize + (float)TILE_SIZE/2.0f, -pxSize);

    // for (var x = 0; x < grid.size; x++) {
    //   for (var y = 0; y < grid.size; y++) {
    //     var s = new Sprite();

    //     int index = x + (y * grid.size);

    //     sprites[index] = s;

    //     s.Position = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
    //     s.Scale = new Vector2(0.1875f, 0.1875f);

    //     if (centerIndex == index) {
    //       s.Texture = charTex;
    //       s.RotationDegrees = 0;
    //     }
    //     else if (grid.blocked[index]) {
    //       s.Texture = haltTex;
    //     }
    //     else if (grid.status[index] == NavigationGrid.STATUS_UNKNOWN) {
    //       s.Texture = helpTex;
    //     }
    //     else if (grid.status[index] == NavigationGrid.STATUS_SCORED) {
    //       s.Texture = plainArrow;
    //       s.RotationDegrees = grid.direction[index] + 180;
    //       s.Modulate = new Color(0,0,0, 1.0f - (grid.scores[index] / 12.0f));
    //     }
    //     else {
    //       s.Visible = false;
    //     }

    //     AddChild(s);
    //   }
    // }
  }

  private (int, int) computeGridPositionFromViewport(Vector2 pos) {
    GD.Print(Position);

    return (
      Mathf.CeilToInt(((pos.x) / (float)TILE_SIZE*2.0f) - 0.5f),
      Mathf.CeilToInt(((pos.y) / (float)TILE_SIZE*2.0f) - 0.5f)
    );
  }

  public override void _Input (InputEvent @event) {
    // if (@event is InputEventMouse) {
    //   var @m = (InputEventMouse)@event;

    //   var (x, y) = computeGridPositionFromViewport(@m.Position);

    //   var path = grid?.computePath(x, y);

    //   if (path != null) {
    //     GD.Print("(",x, ",", y, "): ", path.Length);
    //   }
    // }

    if (@event is InputEventMouseButton) {
      var @btn = (InputEventMouseButton)@event;

      if (@btn.IsPressed() && @btn.ButtonIndex == (int)ButtonList.Right) {
        var vector = (@btn.Position - (GetViewport().Size / 2.0f));


        EmitSignal(nameof(OnCommand), new MoveCommand(CardinalDirectionFromVector2(vector)));
      }
    }
  }

  static CardinalDirection[] CARDINAL_MAPPING = new CardinalDirection[]{
    CardinalDirection.North,
    CardinalDirection.NorthEast,
    CardinalDirection.East,
    CardinalDirection.SouthEast,
    CardinalDirection.South,
    CardinalDirection.SouthWest,
    CardinalDirection.West,
    CardinalDirection.NorthWest
  };

  static CardinalDirection CardinalDirectionFromDegrees(float angle) {
    var a = (angle + 360.0f + 22.5f) % 360.0f;
    var idx = Mathf.CeilToInt(a / 45.0f) - 1;

    CardinalDirection direction = CARDINAL_MAPPING[idx];

    return direction;
  }

    static float DegreesFromVector2(Vector2 vector) {
    var v = vector.Normalized();

    var a = ((Mathf.Atan2(v.y, v.x)*180/Mathf.Pi) + 360.0f + 90.0f) % 360.0f;

    return a;
  }

  static CardinalDirection CardinalDirectionFromVector2(Vector2 vector) {
    var a = DegreesFromVector2(vector);

    return CardinalDirectionFromDegrees(a);
  }
}
