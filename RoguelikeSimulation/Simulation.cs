using System;
using System.Collections.Generic;

namespace Simulation
{
  public class Simulation {
    private State state;
    private Mutation[] lastMutations;

    public Mutation[] GetLastMutations() {
      return lastMutations;
    }

    public Simulation() {
      this.state = new State();
      this.lastMutations = new Mutation[0];
    }

    private List<Action> subscritions = new List<Action>();

    public Action Subscribe (Action next) {
      subscritions.Add(next);
      return () => {
        subscritions.Remove(next);
      };
    }

    public Actor GetPlayer() {
      return state.player;
    }

    public List<Actor> FindActorsNear(Actor subject) {
      return state.characters;
    }

    public Actor? FindEnemyAt(int x, int y) {
      return FindEnemyAt(new Position(x, y));
    }

    public Actor? FindEnemyAt(Position pos) {
      return state.characters.Find(c => c.Position.Equals(pos) && (c.CurrentHealth > 0));
    }

    public Actor? FindActorByReference(Guid reference) {
      return state.characters.Find(c => c.Reference == reference);
    }

    private Mutation[] _Execute(Command command) {
      switch (command) {
        case WalkCommand cmd:
          return cmd.Execute(this);
        case DefaultAttackCommand cmd:
          return cmd.Execute(this);
        case DefaultCommand cmd:
          return cmd.Execute(this);
      }
      return new Mutation[0];
    }

    public void Execute(Command command) {
      var mutations = _Execute(command);

      foreach (var mutation in mutations) {
        Mutate(mutation);
      }

      foreach (var sub in subscritions) {
        sub();
      }

      this.lastMutations = mutations;
    }

    public void Mutate(Mutation um) {
      if (um is AddActorMutation) {
        var m = (AddActorMutation)um;

        state.characters.Add(m.Actor);
      }
      else if (um is DefaultAttackMutation) {
        var m = (DefaultAttackMutation)um;

        m.Target.CurrentHealth -= m.Damage;
      }
      else if (um is WalkMutation) {
        var m = (WalkMutation)um;

        m.Subject.Position = m.Destination;
      }
    }

    public bool IsWalkableBy(Actor subject, Position destination) {
      return state.characters.Find(c => c.Position.Equals(destination)) == null;
    }
  }

  static class ExecuteWalkCommand {
    static public Mutation[] Execute(this WalkCommand cmd, Simulation sim) {
      var subject = sim.GetPlayer();
      var destination = subject.Position.Project(cmd.direction);

      return new Mutation[]{
        new WalkMutation(subject, destination)
      };
    }
  }

  static class ExecuteDefaultCommand {
    static public Mutation[] Execute(this DefaultCommand cmd, Simulation sim) {
      var subject = sim.GetPlayer();
      var destination = subject.Position.Project(cmd.direction);
      var target = sim.FindEnemyAt(destination);

      if (target == null) {
        if (sim.IsWalkableBy(subject, destination)) {
          return new Mutation[]{
            new WalkMutation(subject, destination)
          };
        }
        return new Mutation[0];
      }
      else {
        return ExecuteDefaultAttackCommand.Execute(subject, target, sim);
      }
    }
  }

  static class ExecuteDefaultAttackCommand {
    static public Mutation[] Execute(Actor subject, Actor target, Simulation sim) {
      int damage = 10;

      if ((target.CurrentHealth - damage) <= 0) {
        return new Mutation[]{
          new DefaultAttackMutation(subject, target, damage),
          new DeathMutation(target),
        };
      }

      return new Mutation[]{
        new DefaultAttackMutation(subject, target, damage)
      };
    }

    static public Mutation[] Execute(this DefaultAttackCommand cmd, Simulation sim) {
      var subject = sim.GetPlayer();
      var destination = subject.Position.Project(cmd.direction);
      var target = sim.FindEnemyAt(destination);

      if (target != null) {
        return Execute(subject, target, sim);
      }

      return new Mutation[0];
    }
  }
}
