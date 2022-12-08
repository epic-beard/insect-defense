using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
    private readonly int tileSpacing = 10;

    [SerializeField] List<Vector2Int> exits;
    [SerializeField] List<Vector2Int> entrances;
    void Start() {
        
    }

    void Update() {
        
    }

    public Vector2Int GetCoordinates() {
        int x = Mathf.RoundToInt(transform.position.x / tileSpacing);
        int y = Mathf.RoundToInt(transform.position.y / tileSpacing);
        return new Vector2Int(x, y);
    }
}
