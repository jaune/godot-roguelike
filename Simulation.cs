
using System;
using System.Collections.Generic;

public class Simulation {
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

  public class Position {
    public int x;
    public int y;

    public Position() {
      this.x = 0;
      this.y = 0;
    }

    public Position(Position pos) {
      this.x = pos.x;
      this.y = pos.y;
    }

    public Position(int x, int y) {
      this.x = x;
      this.y = y;
    }

    public void Set(int x, int y) {
      this.x = x;
      this.y = y;
    }

    public bool Equals(Position other) {
      return other.x == this.x && other.y == this.y;
    }

    public Position Project(CardinalDirection d) {
      var copy = new Position(this);

      copy.Move(d);

      return copy;
    }

    public void Move(CardinalDirection d) {
      switch (d) {
        case CardinalDirection.North:
          this.y -= 1;
          break;
        case CardinalDirection.NorthEast:
          this.x += 1;
          this.y -= 1;
          break;
        case CardinalDirection.East:
          this.x += 1;
          break;
        case CardinalDirection.SouthEast:
          this.x += 1;
          this.y += 1;
          break;
        case CardinalDirection.South:
          this.y += 1;
          break;
        case CardinalDirection.SouthWest:
          this.x -= 1;
          this.y += 1;
          break;
        case CardinalDirection.West:
          this.x -= 1;
          break;
        case CardinalDirection.NorthWest:
          this.x -= 1;
          this.y -= 1;
          break;
      }
    }
  }

  public class Character {
    public readonly Guid Reference;
    public Position Position;
    public int CurrentHealth = 100;
    public int MaximumHealth = 100;

    public Character() {
      Reference = Guid.NewGuid();
      Position = new Position();
    }
  }

  public class State {
    public Character player;
    public List<Character> characters;

    public State() {
      this.player = new Character();
      this.characters = new List<Character>(1);

      this.characters.Add(this.player);
    }
  }

  private State state;

  public State GetState() {
    return state;
  }

  private Simulation() {
    var state = new State();

    this.state = state;
  }

  static private Simulation? Instance = null;

  static public Simulation GetInstance () {
    if (Instance == null) {
      Instance = new Simulation();
    }
    return Instance;
  }


  private void PerformMove(Position destination) {
    var other = state.characters.Find(c => c.Position.Equals(destination));

    if (other == null) {
      state.player.Position.x = destination.x;
      state.player.Position.y = destination.y;
    }
  }

  private void PerformDefaultAttack(Character target) {
    target.CurrentHealth -= 10;
  }


  private void ExecuteDefaultCommand(DefaultCommand cmd) {
    var destination = state.player.Position.Project(cmd.direction);
    var target = QueryEnemyAt(destination);

    if (target == null) {
      PerformMove(destination);
    }
    else {
      PerformDefaultAttack(target);
    }
  }

  private void ExecuteMoveCommand(MoveCommand cmd) {
    var destination = state.player.Position.Project(cmd.direction);

    PerformMove(destination);
  }

  private void ExecuteDefaultAttackCommand(DefaultAttackCommand cmd) {
    var destination = state.player.Position.Project(cmd.direction);
    var target = QueryEnemyAt(destination);

    if (target != null) {
      PerformDefaultAttack(target);
    }
  }

  public void Execute(Command cmd) {
    if (cmd is MoveCommand) {
      ExecuteMoveCommand((MoveCommand)cmd);
    }
    else if (cmd is DefaultAttackCommand) {
      ExecuteDefaultAttackCommand((DefaultAttackCommand)cmd);
    }
    else if (cmd is DefaultCommand) {
      ExecuteDefaultCommand((DefaultCommand)cmd);
    }
  }

  public Character? QueryEnemyAt(int x, int y) {
    return QueryEnemyAt(new Position(x, y));
  }

  public Character? QueryEnemyAt(Position pos) {
    return state.characters.Find(c => c.Position.Equals(pos));
  }
}
