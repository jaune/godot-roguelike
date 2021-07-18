namespace Simulation
{
  public interface Mutation {
  }

  public class WalkMutation: Mutation {
    public readonly Actor Subject;
    public readonly Position Destination;

    public WalkMutation(Actor subject, Position destination) {
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

  public class DeathMutation: Mutation {
    public readonly Actor Subject;

    public DeathMutation(Actor subject) {
      this.Subject = subject;
    }
  }

  public static class AddActorMutationExtensions {
    public static void AddActor(this Simulation sim, Actor actor) {
      sim.Mutate(new AddActorMutation(actor));
    }
  }

  public class AddActorMutation: Mutation {
    public Actor Actor;

    public AddActorMutation(Actor actor) {
      Actor = actor;
    }
  }
}
