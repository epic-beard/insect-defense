using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour {
  public static CameraManager Instance;

  [SerializeField]
  private float heightToBuffer = 1.0f;
  [SerializeField]
  private float cameraOffset = 20;
  [SerializeField]
  private float speed = 200;
  [SerializeField]
  private float rotationSpeed = 5;
  [SerializeField]
  private float zoomSpeed = 0.25f;
  [SerializeField]
  private float minHeight = 20;
  [SerializeField]
  private float maxHeight = 100;

  // These mark the extents of the map in each of the four directions.
  private float minX, minZ, maxX, maxZ;

  // This is how much padding, in world space, we give to the map
  //private static readonly float cameraOffset = 20;
  //private static readonly float speed = 100;
  //private static readonly float rotateSpeed = 1;
  //private static readonly float zoomSpeed = 10;

  private void Awake() {
    Instance = this;
    SetExtents();
  }

  private void SetExtents() {
    var positions = FindObjectsOfType<Tile>().Select((tile)=>tile.transform.position);

    minX = positions.Min(p => p.x);
    maxX = positions.Max(p => p.x);

    minZ = positions.Min(p => p.z);
    maxZ = positions.Max(p => p.z);
  } 

  public void MoveCamera(Vector2 axis) {
    Vector3 f = transform.forward;
    Vector3 r = transform.right;
    float d = transform.position.y * heightToBuffer;
    Vector3 v = new(axis.x, 0.0f, axis.y);
    float delta =
      Time.unscaledDeltaTime * speed * PlayerState.Instance.Settings.CameraSensitivity;
    Vector3 newPosition = transform.position + delta * axis.y * f + delta * axis.x * r;

    newPosition.x = Math.Clamp(newPosition.x, minX - d, maxX + d);
    newPosition.y = transform.position.y;
    newPosition.z = Math.Clamp(newPosition.z, minZ - d, maxZ + d);

    transform.position = newPosition;
  }

  // Update the camera zoom, using the sentinels to position the camera.
  public void ZoomCamera(float zoom) {
    Vector3 p = transform.position;
    Vector3 f = transform.forward;
    float delta = zoom *zoomSpeed * PlayerState.Instance.Settings.ZoomSensitivity;
    float deltaMax = (minHeight - p.y) / f.y;
    float deltaMin = (maxHeight - p.y) / f.y;
    delta = Math.Clamp(delta, deltaMin, deltaMax);
    transform.position += delta * f;
  }

  public void RotateCamera(float rotation) {
    float angle = rotation * rotationSpeed * PlayerState.Instance.Settings.RotationSensitivity;
    // Rotate about the y axis by angle degrees.

    transform.Rotate(0, angle, 0, Space.World);
  }
}
