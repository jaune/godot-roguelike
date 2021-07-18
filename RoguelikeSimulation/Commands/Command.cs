namespace Simulation {
  // Commands are actions performed by the player with the perspective of the player
  public interface Command {
  }

  public class WalkCommand: Command {
    public CardinalDirection direction;

    public WalkCommand (CardinalDirection direction) {
      this.direction = direction;
    }
  }

  public class DefaultAttackCommand: Command {
    public CardinalDirection direction;

    public DefaultAttackCommand (CardinalDirection direction) {
      this.direction = direction;
    }
  }

  public class DefaultCommand: Command {
    public CardinalDirection direction;

    public DefaultCommand (CardinalDirection direction) {
      this.direction = direction;
    }
  }
}
