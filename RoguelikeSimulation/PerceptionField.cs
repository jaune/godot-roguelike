using System;

namespace Simulation
{
  public class PerceptionField: RegionField<float> {
    public readonly float Radius;

    public PerceptionField(Vector2i position, float radius): base(position, (int)Math.Ceiling(radius * 2), 0.0f) {
      Radius = radius;

      // TODO: Build should be static and linked to grid
      Build();
    }

    // https://www.redblobgames.com/grids/circle-drawing/#outline
    private void Build () {
      int centerX = Size / 2;
      int centerY = Size / 2;
      int maxR = (int)Math.Floor(Radius * Math.Sqrt(0.5));

      for (int r = 0; r <= maxR; r++) {
        int d = (int)(Math.Floor(Math.Sqrt(Radius*Radius - r*r)));

        // draw tile (center.x - d, center.y + r)
        // draw tile (center.x + d, center.y + r)
        for (int x = centerX - d; x <= centerX + d; x++) {
          int idx = x + ((centerY + r) * Size);

          Data[idx] = 1.0f;
        }

        // draw tile (center.x - d, center.y - r)
        // draw tile (center.x + d, center.y - r)
        for (int x = centerX - d; x <= centerX + d; x++) {
          int idx = x + ((centerY - r) * Size);

          Data[idx] = 1.0f;
        }

        // draw tile (center.x + r, center.y - d)
        // draw tile (center.x - r, center.y - d)
        for (int x = centerX - r; x <= centerX + r; x++) {
          int idx = x + ((centerY - d) * Size);

          Data[idx] = 1.0f;
        }

        // draw tile (center.x + r, center.y + d)
        // draw tile (center.x - r, center.y + d)
        for (int x = centerX - r; x <= centerX + r; x++) {
          int idx = x + ((centerY + d) * Size);

          Data[idx] = 1.0f;
        }
      }
    }
  }
}
