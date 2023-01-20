#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour {

  void Awake() {
    PopulateWaypoints(FindObjectsOfType<Waypoint>());
  }

  // Populates the next/prev_waypoints lists and the DistanceToEnd in Waypoint
  void PopulateWaypoints(Waypoint[] waypoints) {
    if (waypoints.Length == 0) {
      Debug.Log("[ERROR] No waypoints found");
      return;
    }

    // Get the extents of the tile grid.
    var coords = waypoints.ToList().Select(w => w.GetCoordinates());
    int minX = coords.Select(c => c.x).Min();
    int maxX = coords.Select(c => c.x).Max() - minX;
    int minY = coords.Select(c => c.y).Min();
    int maxY = coords.Select(c => c.y).Max() - minY;

    Vector2Int coordOffset = new(minX, minY);
    // Arrange the Waypoints into a grid.
    Waypoint[,] grid = new Waypoint[maxX + 1, maxY + 1];
    foreach (var waypoint in waypoints) {
      Vector2Int coord = waypoint.GetCoordinates() - coordOffset;
      grid[coord.x, coord.y] = waypoint;
    }

    // Loop through each waypoint, finding its neigbhors and setting the
    // next/prev_waypoints according to the Directions in exits.
    foreach (var waypoint in waypoints) {
      Vector2Int coord = waypoint.GetCoordinates() - coordOffset;
      foreach (var exit in waypoint.exits) {
        Waypoint.Direction dir = GetRotatedDirection(waypoint.transform, exit);
        Waypoint? neighbor = GetNeigbhor(dir, coord, grid);
        if (neighbor == null) continue;
        neighbor.prevWaypoints.Add(waypoint);
        waypoint.nextWaypoints.Add(neighbor);
      }
    }

    GetDistanceToEnd(waypoints);
  }

  // Gets the neigbhor in the dir Direcion if it is a valid grid location,
  // otherwise returns null.
  Waypoint? GetNeigbhor(Waypoint.Direction dir, Vector2Int coord, Waypoint[,] grid) {
    if (dir == Waypoint.Direction.UP && coord.y + 1 < grid.GetLength(1)) {
      return grid[coord.x, coord.y + 1];
    } else if (dir == Waypoint.Direction.RIGHT && coord.x + 1 < grid.GetLength(0)) {
      return grid[coord.x + 1, coord.y];
    } else if (dir == Waypoint.Direction.DOWN && coord.y - 1 >= 0) {
      return grid[coord.x, coord.y - 1];
    } else if (dir == Waypoint.Direction.LEFT && coord.x - 1 >= 0) {
      return grid[coord.x - 1, coord.y];
    } else {
      return null;
    }
  }

  // Rotates a Direction by the 90 degree steps of the z rotation of the transform.
  private Waypoint.Direction GetRotatedDirection(Transform transform, Waypoint.Direction dir) {
    int steps = Mathf.RoundToInt(transform.rotation.eulerAngles.y / 90);
    return (Waypoint.Direction)(((int)dir + steps) % 4);
  }
  private void GetDistanceToEnd(Waypoint[] waypoints) {
    foreach (Waypoint waypoint in waypoints) {
      GetDistanceToEnd(waypoint);
    }
  }

  private float GetDistanceToEnd(Waypoint waypoint) {
    if (waypoint.nextWaypoints.Count == 0) return 0.0f;
    if (waypoint.DistanceToEnd != 0) return waypoint.DistanceToEnd;

    waypoint.DistanceToEnd = waypoint.nextWaypoints
      .Select(w => GetDistanceToEnd(w) + Vector3.Distance(waypoint.transform.position, w.transform.position))
      .Min();

    return waypoint.DistanceToEnd;
  }
}