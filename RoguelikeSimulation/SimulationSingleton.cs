namespace Simulation
{
  public class SimulationSingleton {
    static private Simulation? Instance = null;

    static public Simulation SetInstance (Simulation i) {
      if (Instance != null) {
        throw new System.Exception("SimulationSingleton: Instance already set.");
      }
      return Instance = i;
    }

    static public Simulation GetInstance () {
      if (Instance == null) {
        throw new System.Exception("SimulationSingleton: Instance should be set before using it.");
      }
      return Instance;
    }
  }
}

