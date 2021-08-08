using System;
using System.Collections.Generic;

namespace Simulation
{
  public class Simulation {
    private Map[] maps;
    private Actor? player = null;
    private List<Actor> actors;
    private Mutation[] lastMutations;

    private Simulation(Map[] worlds) {
      this.maps = worlds;
      this.actors = new List<Actor>();

      var spawn = FindDefaultPlayerSpawnLocation();

      if (spawn != null) {
        this.player = new Actor("Player", spawn);
        this.player.Appearance = Appearance.Human;
        this.actors.Add(this.player);
      }

      this.lastMutations = new Mutation[0];
    }

    public static Simulation Create(Map[] worlds) {
      if (worlds.Length < 1) {
        throw new Exception("Simulation.Create: must have a world.");
      }

      return new Simulation(worlds);
    }

    public Map GetDefaultWorld() {
      return this.maps[0];
    }

    public Location? FindDefaultPlayerSpawnLocation() {
      return GetDefaultWorld().FindDefaultPlayerSpawnLocation();
    }

    public Mutation[] GetLastMutations() {
      return lastMutations;
    }

    private List<Action> subscritions = new List<Action>();

    public Action Subscribe (Action next) {
      subscritions.Add(next);
      return () => {
        subscritions.Remove(next);
      };
    }

    public Actor? FindPlayer() {
      return player;
    }

    public Actor GetPlayer() {
      var player = FindPlayer();

      if (player == null) {
        throw new System.Exception("Missing player");
      }

      return player;
    }

    public List<Actor> FindActorsNear(Actor subject) {
      return actors;
    }

    public Actor? FindEnemyAt(Location pos) {
      return actors.Find(c => c.Location.Equals(pos) && (c.CurrentHealth > 0));
    }

    public Actor? FindActorByReference(Guid reference) {
      return actors.Find(c => c.Reference == reference);
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

        actors.Add(m.Actor);
      }
      else if (um is DefaultAttackMutation) {
        var m = (DefaultAttackMutation)um;

        m.Target.CurrentHealth -= m.Damage;
      }
      else if (um is WalkMutation) {
        var m = (WalkMutation)um;

        m.Subject.Location = m.Destination;
      }
    }

    public bool IsWalkableBy(Actor subject, Location destination) {
      if (!destination.IsWalkable()) {
        return false;
      }
      return actors.Find(c => c.Location.Equals(destination)) == null;
    }
  }

  static class ExecuteWalkCommand {
    static public Mutation[] Execute(this WalkCommand cmd, Simulation sim) {
      var subject = sim.GetPlayer();
      var destination = subject.Location.Project(cmd.direction);

      return new Mutation[]{
        new WalkMutation(subject, destination)
      };
    }
  }

  static class ExecuteDefaultCommand {
    static public Mutation[] Execute(this DefaultCommand cmd, Simulation sim) {
      var subject = sim.GetPlayer();
      var destination = subject.Location.Project(cmd.direction);
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
      var destination = subject.Location.Project(cmd.direction);
      var target = sim.FindEnemyAt(destination);

      if (target != null) {
        return Execute(subject, target, sim);
      }

      return new Mutation[0];
    }
  }
}
