// Commands are actions performed by the player with the perspective of the player
public class Command {
}

class MoveCommand: Command {
  public Simulation.CardinalDirection direction;

  public MoveCommand (Simulation.CardinalDirection direction) {
    this.direction = direction;
  }
}

class DefaultAttackCommand: Command {
  public Simulation.CardinalDirection direction;

  public DefaultAttackCommand (Simulation.CardinalDirection direction) {
    this.direction = direction;
  }
}

class DefaultCommand: Command {
  public Simulation.CardinalDirection direction;

  public DefaultCommand (Simulation.CardinalDirection direction) {
    this.direction = direction;
  }
}
