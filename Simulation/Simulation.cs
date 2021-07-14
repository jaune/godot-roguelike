using System;

namespace Simulation
{
  class Simulation {
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

    private Mutation[] PerformMove(Position destination) {
      var other = state.characters.Find(c => c.Position.Equals(destination));

      if (other == null) {
        state.player.Position.x = destination.x;
        state.player.Position.y = destination.y;
      }

      return new Mutation[0];
    }

    private Mutation[] PerformDefaultAttack(Character target) {
      target.CurrentHealth -= 10;

      return new Mutation[0];
    }

    public Character? QueryEnemyAt(int x, int y) {
      return QueryEnemyAt(new Position(x, y));
    }

    public Character? QueryEnemyAt(Position pos) {
      return state.characters.Find(c => c.Position.Equals(pos));
    }

    public Character? QueryCharacterByReference(Guid reference) {
      return state.characters.Find(c => c.Reference == reference);
    }

    private Mutation[] ExecuteDefaultCommand(DefaultCommand cmd) {
      var destination = state.player.Position.Project(cmd.direction);
      var target = QueryEnemyAt(destination);

      if (target == null) {
        return PerformMove(destination);
      }
      else {
        return PerformDefaultAttack(target);
      }
    }

    private Mutation[] ExecuteMoveCommand(MoveCommand cmd) {
      var destination = state.player.Position.Project(cmd.direction);

      return PerformMove(destination);
    }

    private Mutation[] ExecuteDefaultAttackCommand(DefaultAttackCommand cmd) {
      var destination = state.player.Position.Project(cmd.direction);
      var target = QueryEnemyAt(destination);

      if (target != null) {
        return PerformDefaultAttack(target);
      }

      return new Mutation[0];
    }

    public Mutation[] Execute(Command cmd) {
      if (cmd is MoveCommand) {
        return ExecuteMoveCommand((MoveCommand)cmd);
      }
      else if (cmd is DefaultAttackCommand) {
        return ExecuteDefaultAttackCommand((DefaultAttackCommand)cmd);
      }
      else if (cmd is DefaultCommand) {
        return ExecuteDefaultCommand((DefaultCommand)cmd);
      }

      return new Mutation[0];
    }
  }

  public class Mutation {
  }
}

