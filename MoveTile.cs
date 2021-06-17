using Godot;
using System;
using System.Collections.Generic;

public enum CardinalDirection {
  None = 0,
  North = 0x1,
  NorthEast = 0x2,
  East = 0x3,
  SouthEast = 0x4,

  South = 0x5,
  SouthWest = 0x6,
  West = 0x7,
  NorthWest = 0x8,
}

class Position {
  public int x = 0;
  public int y = 0;

  public Position(Position p) {
    this.x = p.x;
    this.y = p.y;
  }

  public Position(int x, int y) {
    this.x = x;
    this.y = y;
  }
}

// Commands are verb perform by the player with the perspective of the player
interface Command {
}

struct MoveCommand : Command {
  public CardinalDirection direction;

  public MoveCommand (CardinalDirection direction) {
    this.direction = direction;
  }
}

class Actor {
  public Guid id;
  public Position position = new Position(0, 0);
}

// Actions are verb perform by actors
interface Mutation {
}

struct MoveToMutation : Mutation {
  Actor subject;
  Position destination;

  public MoveToMutation(Actor subject, Position destination) {
    this.subject = subject;
    this.destination = destination;
  }
}

struct State {
  public Guid player_id;
  public Dictionary<Guid, Actor> actors;
}

struct Selector {
  static public Actor selectPlayerActor (State state) {
    var a = state.actors[state.player_id];

    return a;
  }
}

class StateBuilder {
  public static State build() {
    var player_id = new Guid();

    var actors = new Dictionary<Guid, Actor>();

    actors.Add(player_id, new Actor {
      id = player_id,
    });

    State state = new State {
      player_id = player_id,
      actors = actors,
    };

    return state;
  }
}

public class MoveTile : Node2D
{
  [Export(PropertyHint.Enum)]
  public CardinalDirection direction = CardinalDirection.None;

  private ColorRect? rect;

  private Sprite? icon;

  // Declare member variables here. Examples:
  // private int a = 2;
  // private string b = "text";

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    rect = GetNode<ColorRect>("./Rect");

    rect.Connect("mouse_entered", this, nameof(_on_ColorRect_mouse_entered));
    rect.Connect("mouse_exited", this, nameof(_on_ColorRect_mouse_exited));

    icon = GetNode<Sprite>("./move-icon");
  }

  public void _on_ColorRect_mouse_entered () {
    if (rect != null) {
      rect.Color = new Color(1, 0, 1, 0.30f);
    }
    if (icon != null) {
      icon.Visible = true;
    }
    // Input.SetCustomMouseCursor()
  }


  public void _on_ColorRect_mouse_exited () {
    if (rect != null) {
      rect.Color = new Color(1, 0, 1, 0.12f);
    }
    if (icon != null) {
      icon.Visible = false;
    }
  }

  State state = StateBuilder.build();

  public override void _Input (InputEvent @event) {
    if (@event is InputEventMouseButton && @event.IsPressed() && icon != null && icon.Visible) {
      var r = MutationPlaner.plan(new MoveCommand(direction), state);

      if (r.mutations != null) {
        foreach (var mutation in r.mutations) {
          GD.Print(mutation);
        }
      }
    }
  }
}

class MutationPlaner {
  public interface Error {}

  public struct Result {
    public Mutation[] mutations;
    public Error? error;

    public Result(Mutation[] mutations)
    {
        this.mutations = mutations;
        this.error = null;
    }

    public Result(Error error)
    {
        this.mutations = new Mutation[0];
        this.error = error;
    }
  }

  static Result plan (MoveCommand command, State state) {
    var player = Selector.selectPlayerActor(state);

    var destination = new Position(player.position);

    switch (command.direction) {
      case CardinalDirection.North:
        destination.y += 1;
        break;
      case CardinalDirection.South:
        destination.y -= 1;
        break;
      case CardinalDirection.East:
        destination.x += 1;
        break;
      case CardinalDirection.West:
        destination.x -= 1;
        break;
    }

    Mutation[] mutations = {
      new MoveToMutation(player, destination),
    };

    return new Result(mutations);
  }

  public static Result plan (Command command, State state) {
    if (command is MoveCommand) {
      return plan((MoveCommand)command, state);
    }

    return new Result {
      error = null,
    };
  }
}

class MutationApplyer {
  public interface Error {}

  public struct Result {
    public State? state;
    public Error? error;

    public Result(State state)
    {
        this.state = state;
        this.error = null;
    }

    public Result(Error error)
    {
        this.state = null;
        this.error = error;
    }
  }

  public static Result apply (Mutation[] mutations, State state) {
    return new Result(state);
  }
}
