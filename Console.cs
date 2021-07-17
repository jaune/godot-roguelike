using Godot;
using Simulation;

public class Console : RichTextLabel
{
  public void _OnMutations() {
    var mutations = SimulationSingleton.GetInstance().GetLastMutations();

    foreach (var mutation in mutations) {
      AppendBbcode("\n" + mutation.ToConsoleString());
    }
  }
}

public static class MutationConsoleExtensions
{
  public static string ToConsoleString(this Simulation.Mutation mutation)
  {
    if (mutation is Simulation.MoveMutation) {
      return MoveMutationToConsoleString((Simulation.MoveMutation)mutation);
    }
    else if (mutation is Simulation.DefaultAttackMutation) {
      return DefaultAttackMutationToConsoleString((Simulation.DefaultAttackMutation)mutation);
    }

    return "";
  }

  private static string MoveMutationToConsoleString(Simulation.MoveMutation mutation) {
    return $"{mutation.Subject.DisplayName} moved";
  }

  private static string DefaultAttackMutationToConsoleString(Simulation.DefaultAttackMutation mutation) {
    return $"{mutation.Subject.DisplayName} dealt {mutation.Damage} to {mutation.Target.DisplayName}";
  }

}
