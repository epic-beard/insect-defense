using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I am alive!!!
public class Waypoint : MonoBehaviour {
    public enum Direction {
        UP = 0,
        RIGHT = 1,
        DOWN = 2,
        LEFT = 3
    }

    private readonly int tileSpacing = 10;

    [SerializeField] public List<Direction> exits = new();
    //[SerializeField] List<Vector2Int> entrances;
    private void Start() {
        
    }

    private void Update() {
        
    }

    public List<Waypoint> nextWaypoints = new();
    public List<Waypoint> prevWaypoints = new();

    public Vector2Int GetCoordinates() {
        int x = Mathf.RoundToInt(transform.position.x / tileSpacing);
        int y = Mathf.RoundToInt(transform.position.y / tileSpacing);
        return new Vector2Int(x, y);
    }

    public int GetRotationSteps() {
        return Mathf.RoundToInt(transform.rotation.eulerAngles.z / 90);
    }

    public Vector2Int DirectionToVec(Direction dir) {
        return dir switch {
            Direction.UP => new Vector2Int(0, 1),
            Direction.RIGHT => new Vector2Int(1, 0),
            Direction.DOWN => new Vector2Int(0, -1),
            Direction.LEFT => new Vector2Int(-1, 0),
            _ => new Vector2Int(0, 0),
        };
    }
}
