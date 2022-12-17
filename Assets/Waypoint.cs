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

    public Waypoint GetNextWaypoint() {
        return GetRandomWaypoint(nextWaypoints);
    }

    public Waypoint GetPrevWaypoint() {
        return GetRandomWaypoint(prevWaypoints);
    }

    private Waypoint GetRandomWaypoint(List<Waypoint> waypoints) {
        int n = nextWaypoints.Count;
        return nextWaypoints[Random.Range(0, n) % waypoints.Count];
    }

    // Finds the coordinates in tiles.
    public Vector2Int GetCoordinates() {
        int x = Mathf.RoundToInt(transform.position.x / TileSpacing);
        int y = Mathf.RoundToInt(transform.position.y / TileSpacing);
        return new Vector2Int(x, y);
    }
}
