

using System;

namespace Simulation
{
  public class PerceptionMask {
    float Radius;
    float[] Data;

    public PerceptionMask(float radius) {
      Radius = radius;
      var width = (int)Math.Ceiling((Radius * 2) + 1);
      Data = new float[width * width];

      for (var i = 0; i < Data.Length; i++) {
        Data[i] = 0.0f;
      }
    }

    // https://www.redblobgames.com/grids/circle-drawing/#outline
    public void Build () {
      int centerX = 0;
      int centerY = 0;

      for (int r = 0; r <= Math.Floor(Radius * Math.Sqrt(0.5)); r++) {
          int d = (int)(Math.Floor(Math.Sqrt(Radius*Radius - r*r)));

          // draw tile (center.x - d, center.y + r)
          // draw tile (center.x + d, center.y + r)
          for (int x = centerX - d; x < centerX + d; x++) {
            int idx = x + ((centerY + r) * Data.Length);

            Data[idx] = 1.0f;
          }

          // draw tile (center.x - d, center.y - r)
          // draw tile (center.x + d, center.y - r)
          for (int x = centerX - d; x < centerX + d; x++) {
            int idx = x + ((centerY - r) * Data.Length);

            Data[idx] = 1.0f;
          }

          // draw tile (center.x + r, center.y - d)
          // draw tile (center.x - r, center.y - d)
          for (int x = centerX - r; x < centerX + r; x++) {
            int idx = x + ((centerY - d) * Data.Length);

            Data[idx] = 1.0f;
          }

          // draw tile (center.x + r, center.y + d)
          // draw tile (center.x - r, center.y + d)
          for (int x = centerX - r; x < centerX + r; x++) {
            int idx = x + ((centerY + d) * Data.Length);

            Data[idx] = 1.0f;
          }
      }
    }
  }
}
