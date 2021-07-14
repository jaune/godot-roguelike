using Godot;

class NavigationGridDebugNode : Node2D {
  const int TILE_SIZE = 96;

  [Signal]
  public delegate void OnCommand(Command cmd);

  public void __onCommand(Command cmd) {
    EmitSignal("OnCommand", cmd);
  }

  NavigationGrid? grid = null;

  Sprite? moveSprite = null;

  Texture? moveTex = null;
  Texture? attackTex = null;

  public override void _Ready()
  {
    moveTex = ResourceLoader.Load<Texture>("res://assets/move-icon.png");
    attackTex = ResourceLoader.Load<Texture>("res://assets/sabers-choc.png");

    moveSprite = new Sprite();

    moveSprite.Texture = moveTex;
    // moveSprite.Visible = false;
    moveSprite.Scale = new Vector2(0.1875f, 0.1875f);
    moveSprite.ZAsRelative = false;
    moveSprite.ZIndex = 100;

    AddChild(moveSprite);

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
    var sim = Simulation.Simulation.GetInstance();
    var state = sim.GetState();

    if (@event is InputEventMouseMotion) {
      var @m = (InputEventMouseMotion)@event;
      var vector = (@m.Position - (GetViewport().Size / 2.0f));

      if (moveSprite != null){
        var dir = CardinalDirectionFromVector2(vector);
        var v = Vector2FromCardinalDirection(dir);

        var dest = state.player.Position.Project(dir);
        var enemy = sim.QueryEnemyAt(dest);

        if (enemy == null) {
          moveSprite.Texture = moveTex;
        }
        else {
          moveSprite.Texture = attackTex;
        }

        moveSprite.Position = new Vector2(
           v.x * (float)TILE_SIZE,
           v.y * (float)TILE_SIZE
        );
      }
    }
    else if (@event is InputEventMouseButton) {
      var @btn = (InputEventMouseButton)@event;

      if (@btn.IsPressed() && @btn.ButtonIndex == (int)ButtonList.Right) {
        var vector = (@btn.Position - (GetViewport().Size / 2.0f));
        var dir = CardinalDirectionFromVector2(vector);

        var dest = state.player.Position.Project(dir);
        var enemy = sim.QueryEnemyAt(dest);

        if (enemy == null) {
          EmitSignal(nameof(OnCommand), new MoveCommand(dir));
        }
        else {
          EmitSignal(nameof(OnCommand), new DefaultAttackCommand(dir));
        }
      }
    }
  }

  static Vector2 Vector2FromCardinalDirection(Simulation.CardinalDirection d) {
    var destination = new Vector2();

    switch (d) {
      case Simulation.CardinalDirection.North:
        destination.y -= 1;
        break;
      case Simulation.CardinalDirection.NorthEast:
        destination.x += 1;
        destination.y -= 1;
        break;
      case Simulation.CardinalDirection.East:
        destination.x += 1;
        break;
      case Simulation.CardinalDirection.SouthEast:
        destination.x += 1;
        destination.y += 1;
        break;
      case Simulation.CardinalDirection.South:
        destination.y += 1;
        break;
      case Simulation.CardinalDirection.SouthWest:
        destination.x -= 1;
        destination.y += 1;
        break;
      case Simulation.CardinalDirection.West:
        destination.x -= 1;
        break;
      case Simulation.CardinalDirection.NorthWest:
        destination.x -= 1;
        destination.y -= 1;
        break;
    }

    return destination;
  }

  static Simulation.CardinalDirection[] CARDINAL_MAPPING = new Simulation.CardinalDirection[]{
    Simulation.CardinalDirection.North,
    Simulation.CardinalDirection.NorthEast,
    Simulation.CardinalDirection.East,
    Simulation.CardinalDirection.SouthEast,
    Simulation.CardinalDirection.South,
    Simulation.CardinalDirection.SouthWest,
    Simulation.CardinalDirection.West,
    Simulation.CardinalDirection.NorthWest
  };

  static Simulation.CardinalDirection CardinalDirectionFromDegrees(float angle) {
    var a = (angle + 360.0f + 22.5f) % 360.0f;
    var idx = Mathf.CeilToInt(a / 45.0f) - 1;

    Simulation.CardinalDirection direction = CARDINAL_MAPPING[idx];

    return direction;
  }

    static float DegreesFromVector2(Vector2 vector) {
    var v = vector.Normalized();

    var a = ((Mathf.Atan2(v.y, v.x)*180/Mathf.Pi) + 360.0f + 90.0f) % 360.0f;

    return a;
  }

  static Simulation.CardinalDirection CardinalDirectionFromVector2(Vector2 vector) {
    var a = DegreesFromVector2(vector);

    return CardinalDirectionFromDegrees(a);
  }
}
