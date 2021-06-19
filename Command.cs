using System;
using System.Collections.Generic;

// Commands are verb perform by the player with the perspective of the player
public class Command: Godot.Object {
}

class MoveCommand: Command {
  public CardinalDirection direction;

  public MoveCommand (CardinalDirection direction) {
    this.direction = direction;
  }
}

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
