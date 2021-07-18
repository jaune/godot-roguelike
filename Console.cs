using Godot;
using Simulation;

public class Console : RichTextLabel
{
  public void _OnMutations() {
    var mutations = SimulationSingleton.GetInstance().GetLastMutations();

    foreach (var mutation in mutations) {
      var s = mutation.ToConsoleString();

      if (s.Length > 0) {
        AppendBbcode("\n" + s);
      }
    }
  }
}

public static class MutationConsoleExtensions
{
  public static string ToConsoleString(this Simulation.Mutation mutation)
  {
    switch (mutation) {
      case Simulation.WalkMutation m:
        return m.ToConsoleString();
      case Simulation.DefaultAttackMutation m:
        return m.ToConsoleString();
      case Simulation.DeathMutation m:
        return m.ToConsoleString();
    }

    return "";
  }

  private static string ToConsoleString(this Simulation.WalkMutation mutation) {
    return $"{mutation.Subject.DisplayName} moved";
  }

  private static string ToConsoleString(this Simulation.DefaultAttackMutation mutation) {
    return $"{mutation.Subject.DisplayName} dealt {mutation.Damage} to {mutation.Target.DisplayName}";
  }

    private static string ToConsoleString(this Simulation.DeathMutation mutation) {
    return $"{mutation.Subject.DisplayName} die";
  }
}
