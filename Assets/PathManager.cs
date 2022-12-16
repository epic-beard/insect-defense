using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour {
    
    void Start() {
        Waypoint[] waypoints = GetComponents<Waypoint>();
        PopulateWaypoints(waypoints);
    }

    void Update() {
        
    }

    void PopulateWaypoints(Waypoint[] waypoints) {
        var coords = waypoints.ToList().Select(w => w.GetCoordinates());
        int maxX = coords.Select(c => c.x).Max();
        int maxY = coords.Select(c => c.y).Max();

        Waypoint[,] grid = new Waypoint[maxX, maxY];
        foreach (var waypoint in waypoints) {
            Vector2Int coord = waypoint.GetCoordinates();
            grid[coord.x, coord.y] = waypoint;
        }

        foreach (var waypoint in waypoints) {
            Vector2Int coord = waypoint.GetCoordinates();
            int rotations = waypoint.GetRotationSteps();
            foreach (var exit in waypoint.exits) {
                Waypoint.Direction dir = (Waypoint.Direction)(((int)exit + rotations) % 4);
                Waypoint neighbor = new();
                if (dir == Waypoint.Direction.UP && coord.y + 1 < maxY) {
                    neighbor = grid[coord.x, coord.y + 1];
                } else if (dir == Waypoint.Direction.RIGHT && coord.x + 1 < maxX) {
                    neighbor = grid[coord.x + 1, coord.y];
                } else if (dir == Waypoint.Direction.DOWN && coord.y - 1 >= 0) {
                    neighbor = grid[coord.x, coord.y - 1];
                } else if (dir == Waypoint.Direction.LEFT && coord.x - 1 >= 0) {
                    neighbor = grid[coord.x + 1, coord.y];
                } else {
                    continue;
                }

                neighbor.prevWaypoints.Add(waypoint);
                waypoint.nextWaypoints.Add(waypoint);

            }
        }
    }


}