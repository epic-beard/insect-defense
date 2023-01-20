#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
  public enum Direction {
    UP = 0,
    RIGHT = 1,
    DOWN = 2,
    LEFT = 3
  }
  public static readonly int TileSpacing = 10;

  public List<Direction> exits = new();

  // Populated by the PathManager
  public List<Waypoint> nextWaypoints = new();
  public List<Waypoint> prevWaypoints = new();

  public Waypoint? GetNextWaypoint() {
    return GetRandomWaypoint(nextWaypoints);
  }

  public Waypoint? GetPrevWaypoint() {
    return GetRandomWaypoint(prevWaypoints);
  }

  private Waypoint? GetRandomWaypoint(List<Waypoint> waypoints) {
    if (waypoints.Count == 0) return null;
    return waypoints[Random.Range(0, waypoints.Count - 1)];
  }

  // Finds the coordinates in tiles.
  public Vector2Int GetCoordinates() {
    int x = Mathf.RoundToInt(transform.position.x / TileSpacing);
    int y = Mathf.RoundToInt(transform.position.z / TileSpacing);
    return new Vector2Int(x, y);
  }
}
