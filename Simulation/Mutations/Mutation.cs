namespace Simulation
{
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
