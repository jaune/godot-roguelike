using System;

namespace Simulation
{

  public class RegionField<T> {
    public readonly Vector2i Position;
    public readonly T[] Data;
    public readonly int Size;
    public readonly T DefaultValue;

    public RegionField(Vector2i position, int size, T defaultValue) {
      if (size <= 0) {
        throw new ArgumentException("size sould be greater than 0");
      }

      Position = position;
      Size = size;
      DefaultValue = defaultValue;
      Data = new T[Size * Size];

      for (var i = 0; i < Data.Length; i++) {
        Data[i] = defaultValue;
      }
    }

    public T GetValueAt(Vector2i point) {
      var o = point - Position;
      int x = o.x + (Size / 2);
      int y = o.y + (Size / 2);

      if (x < 0 || x >= Size || y < 0 || y >= Size) {
        return DefaultValue;
      }

      var idx = x + (y * Size);

      return Data[idx];
    }
  }

}
