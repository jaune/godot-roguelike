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

    public Character? QueryEnemyAt(int x, int y) {
      return QueryEnemyAt(new Position(x, y));
    }

    public Character? QueryEnemyAt(Position pos) {
      return state.characters.Find(c => c.Position.Equals(pos));
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
  }
}

