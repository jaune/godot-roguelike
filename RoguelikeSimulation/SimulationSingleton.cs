namespace Simulation
{
  public class SimulationSingleton {
    private Simulation Simulation;

    private SimulationSingleton() {
      Simulation = new Simulation();
    }

    static private SimulationSingleton? Instance = null;

    static public Simulation GetInstance () {
      if (Instance == null) {
        Instance = new SimulationSingleton();
      }
      return Instance.Simulation;
    }
  }
}

