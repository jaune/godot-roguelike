class NavigationGrid {
  public int[] direction;
  public int[] scores;
  public int[] status;
  public bool[] blocked;

  public int radius;
  public int size;

  public const int STATUS_UNKNOWN = 0;
  public const int STATUS_SCORED = 5;

  public NavigationGrid(int radius) {
    this.radius = radius;
    this.size = 1 + (radius * 2);

    this.blocked = new bool[this.size * this.size];

    for (var i = 0; i < this.blocked.Length; i++) {
      this.blocked[i] = false;
    }

    this.scores = new int[this.size * this.size];
    this.status = new int[this.size * this.size];
    this.direction = new int[this.size * this.size];
  }

  public int[]? computePath (int x, int y) {
    var idx = x + (y * size);

    if (x < 0 || x >= this.size) {
      return null;
    }

    if (y < 0 || y >= this.size) {
      return null;
    }

    if (this.blocked[idx]) {
      return null;
    }

    if (this.status[idx] != STATUS_SCORED) {
      return null;
    }

    var path = new int[this.scores[idx]];

    return path;
  }

  public void compute () {
    var half = (size / 2);

    for (var o = -1; o <= 1; o++) {

      // ⬅
      for (var i = 1; i <= half; i++) {
        var x = half - i;
        var y = half + o;

        var idx = x + (y * size);

        if (blocked[idx]) { break; }

        scores[idx] = i;
        direction[idx] = 90;
        status[idx] = NavigationGrid.STATUS_SCORED;
      }

      // ➡
      for (var i = 1; i <= half; i++) {
        var x = half + i;
        var y = half + o;

        var idx = x + (y * size);

        if (blocked[idx]) { break; }

        scores[idx] = i;
        direction[idx] = 90 + 180;
        status[idx] = NavigationGrid.STATUS_SCORED;
      }

      // ⬆
      for (var i = 1; i <= half; i++) {
        var x = half + o;
        var y = half + i;

        var idx = x + (y * size);

        if (blocked[idx]) { break; }

        scores[idx] = i;
        direction[idx] = 0;
        status[idx] = NavigationGrid.STATUS_SCORED;
      }

      // ⬇
      for (var i = 1; i <= half; i++) {
        var x = half + o;
        var y = half - i;

        var idx = x + (y * size);

        if (blocked[idx]) { break; }

        scores[idx] = i;
        direction[idx] = 180;
        status[idx] = NavigationGrid.STATUS_SCORED;
      }
    }

    // ↘
    for (var i = 1; i <= half; i++) {
      var x = half + i;
      var y = half + i;

      var idx = x + (y * size);

      if (blocked[idx]) { break; }

      scores[idx] = i;
      direction[idx] = 270 + 45;
      status[idx] = NavigationGrid.STATUS_SCORED;
    }

    // ↗
    for (var i = 1; i <= half; i++) {
      var x = half + i;
      var y = half - i;

      var idx = x + (y * size);

      if (blocked[idx]) { break; }

      scores[idx] = i;
      direction[idx] = 180 + 45;
      status[idx] = NavigationGrid.STATUS_SCORED;
    }

    // ↙
    for (var i = 1; i <= half; i++) {
      var x = half - i;
      var y = half + i;

      var idx = x + (y * size);

      if (blocked[idx]) { break; }

      scores[idx] = i;
      direction[idx] = 45;
      status[idx] = NavigationGrid.STATUS_SCORED;
    }

    // ↖
    for (var i = 1; i <= half; i++) {
      var x = half - i;
      var y = half - i;

      var idx = x + (y * size);

      if (blocked[idx]) { break; }

      scores[idx] = i;
      direction[idx] = 90 + 45;
      status[idx] = NavigationGrid.STATUS_SCORED;
    }
  }
}
