using System;

namespace Simulation
{
  class Simulation {
    private State state;
    private Mutation[] lastMutations;

    public State GetState() {
      return state;
    }

    public Mutation[] GetLastMutations() {
      return lastMutations;
    }

    private Simulation() {
      this.state = new State();
      this.lastMutations = new Mutation[0];
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

      return new Mutation[]{
        new MoveMutation(state.player, destination)
      };
    }

    private Mutation[] PerformDefaultAttack(Actor target) {
      target.CurrentHealth -= 10;

      // TODO: if (target.CurrentHealth <= 0) death mutation

      return new Mutation[]{
        new DefaultAttackMutation(state.player, target, 10)
      };
    }

    public Actor? QueryPlayer() {
      return state.player;
    }

    public Actor? QueryEnemyAt(int x, int y) {
      return QueryEnemyAt(new Position(x, y));
    }

    public Actor? QueryEnemyAt(Position pos) {
      return state.characters.Find(c => c.Position.Equals(pos));
    }

    public Actor? QueryCharacterByReference(Guid reference) {
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
      var mutations = new Mutation[0];

      if (cmd is MoveCommand) {
        mutations = ExecuteMoveCommand((MoveCommand)cmd);
      }
      else if (cmd is DefaultAttackCommand) {
        mutations = ExecuteDefaultAttackCommand((DefaultAttackCommand)cmd);
      }
      else if (cmd is DefaultCommand) {
        mutations = ExecuteDefaultCommand((DefaultCommand)cmd);
      }

      this.lastMutations = mutations;

      return mutations;
    }
  }

  public interface Mutation {
  }

  public class MoveMutation: Mutation {
    public readonly Actor Subject;
    public readonly Position Destination;

    public MoveMutation(Actor subject, Position destination) {
      this.Subject = subject;
      this.Destination = destination;
    }
  }

  public class DefaultAttackMutation: Mutation {
    public readonly Actor Subject;
    public readonly Actor Target;
    public readonly int Damage;

    public DefaultAttackMutation(Actor subject, Actor target, int damage) {
      this.Subject = subject;
      this.Target = target;
      this.Damage = damage;
    }
  }
}

