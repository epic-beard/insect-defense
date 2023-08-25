using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour {
  public static CameraManager Instance;

  private Camera camera;
  private Vector3 initialPosition;
  private Vector2 halfWidth;

  private static readonly int speed = 100;
  private static readonly float zoomSpeed = 3;

  private void Awake() {
    Instance = this;
    camera = GetComponent<Camera>();
    initialPosition = camera.transform.position;
    halfWidth = GetHalfWidth();
  }

  private Vector2 GetHalfWidth() {
    var positions = FindObjectsOfType<Tile>().Select((tile)=>tile.transform.position);
    Vector2 halfWidth;
    halfWidth.x = (positions.Max((p) => p.x) - positions.Min((p) => p.x))/2;
    halfWidth.y = (positions.Max((p) => p.z) - positions.Min((p) => p.z))/2;
    return halfWidth;
  } 

  public void MoveCamera(Vector2 axis) {
    Vector3 delta = new Vector3(-axis.y, 0.0f, axis.x) * Time.deltaTime;
    Vector3 position = transform.position 
      + PlayerState.Instance.Settings.CameraSensitivity * speed * delta;
    if (position.x > initialPosition.x + halfWidth.x) {
      position.x = initialPosition.x + halfWidth.x;
    }
    if (position.x < initialPosition.x - halfWidth.x) {
      position.x = initialPosition.x - halfWidth.x;
    }
    //if (position.z > initialPosition.z + halfWidth.y) {
    //  position.z = initialPosition.z + halfWidth.y;
    //}
    //if (position.z < initialPosition.z - halfWidth.y) {
    //  position.z = initialPosition.z - halfWidth.y;
    //}
    transform.position = position;
  }

  public void ZoomCamera(float zoom) {
    camera.orthographicSize += - PlayerState.Instance.Settings.ZoomSensitivity * zoom * zoomSpeed * Time.deltaTime;
  }
}
