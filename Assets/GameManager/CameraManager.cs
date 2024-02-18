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
  private float rotationSpeed = 20.0f;
  [SerializeField]
  private float zoomSpeed = 0.25f;
  [SerializeField]
  private float minHeight = 20;
  [SerializeField]
  private float maxHeight = 100;

  // These mark the extents of the map in each of the four directions.
  private float minX, minZ, maxX, maxZ;

  private void Awake() {
    Instance = this;
    SetExtents();
  }

  private void Start() {
    ResetCamera();
  }

  private void SetExtents() {
    var positions = FindObjectsOfType<Tile>().Select((tile)=>tile.transform.position);

    minX = positions.Min(p => p.x);
    maxX = positions.Max(p => p.x);

    minZ = positions.Min(p => p.z);
    maxZ = positions.Max(p => p.z);
  }

  public void ResetCamera() {
    Vector3 newPosition = new((maxX + minX) / 2, 0, (maxZ + minZ) / 2);
    newPosition -= transform.forward * 100;
    newPosition += Vector3.right * 15;

    transform.position = newPosition;
  }

  public void MoveCamera(Vector2 axis) {
    // f is the forward direction of the camera with the y component removed and renormalized.
    Vector3 f = transform.forward;
    f.y = 0;
    f = f.normalized;
    // f is the right direction of the camera with the y component removed and renormalized.
    Vector3 r = transform.right;
    r.y = 0;
    r = r.normalized;
    // Distance traveled.
    float d =
      Time.unscaledDeltaTime * speed * PlayerState.Instance.Settings.CameraSensitivity;
    // Move the position d * axis.y in the forward direction and d * axis.x in the right direction.
    Vector3 newPosition = transform.position + d * axis.y * f + d * axis.x * r;

    // The distance from the map edge the camera is allowed to move, dependent on camera height.
    float buffer = transform.position.y * heightToBuffer;
    // Clamp the new position to the min/max x/z modified by the buffer ammount.
    newPosition.x = Math.Clamp(newPosition.x, minX - buffer, maxX + buffer);
    newPosition.z = Math.Clamp(newPosition.z, minZ - buffer, maxZ + buffer);

    transform.position = newPosition;
  }

  // Update the camera zoom, using the sentinels to position the camera.
  public void ZoomCamera(float zoom) {
    Vector3 p = transform.position;
    Vector3 f = transform.forward;
    // The distance to move the camera.
    float d = zoom * zoomSpeed * PlayerState.Instance.Settings.ZoomSensitivity * Time.unscaledDeltaTime;
    // The maximum distance allowed.
    float dMax = (minHeight - p.y) / f.y;
    // The maximum distance allowed.
    float dMin = (maxHeight - p.y) / f.y;
    d = Math.Clamp(d, dMin, dMax);
    // Translate the position in the f direction a distance d.
    transform.position += d * f;
  }

  public void RotateCamera(float rotation) {
    float angle = rotation * rotationSpeed * PlayerState.Instance.Settings.RotationSensitivity * Time.unscaledDeltaTime;
    // Rotate about the y axis by angle degrees.
    transform.Rotate(0, angle, 0, Space.World);
  }
}
