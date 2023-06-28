using System.Collections.Generic;
using System.Reflection;

public static class SpawnerUtils {
  public static void AddStartingLocation(this Spawner spawner, Waypoint waypoint) {
    var startingLocations = (List<Waypoint>)typeof(Spawner)
      .GetField("spawnLocations", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(spawner);
    startingLocations.Add(waypoint);
  }
}
