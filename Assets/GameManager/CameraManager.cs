using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour {
  public static CameraManager Instance;

  private float buffer = 20.0f;
  private float panSpeed = 300;
  private float rotationSpeed = 0.2f;
  private float elevationSpeed = 75.0f;
  private float zoomSpeed = 0.5f;
  private float minHeight = 5;
  [SerializeField]
  private float maxHeight = 200;
  private Vector2 turn;

  // These mark the extents of the map in each of the four directions.
  private readonly float sidePanelSize = 15;
  private float minX, minZ, maxX, maxZ;
  private Vector2 originalRotation;

  private void Awake() {
    Instance = this;
    SetExtents();
    Vector3 euler = transform.eulerAngles;
    turn.x = euler.y;
    turn.y = euler.x;
    originalRotation = turn;
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
    transform.localRotation = Quaternion.Euler(originalRotation.y, originalRotation.x, 0);

    Vector3 newPosition = new((maxX + minX) / 2, 0, (maxZ + minZ) / 2);
    newPosition -= transform.forward * maxHeight;
    newPosition += Vector3.right * sidePanelSize;
    transform.position = newPosition;
  }

  public void MoveCamera(Vector2 axis) {
    // f is the forward direction of the camera with the y component removed and renormalized.
    Vector3 f = transform.forward;
    f.y = 0;
    if (f.magnitude < 0.01f) {
      f = transform.up;
      f.y = 0;
    }
    f = f.normalized;
    // f is the right direction of the camera with the y component removed and renormalized.
    Vector3 r = transform.right;
    r.y = 0;
    r = r.normalized;
    // Distance traveled.
    float d = Time.unscaledDeltaTime * panSpeed * PlayerState.Instance.Settings.CameraSensitivity;
    // Move the position d * axis.y in the forward direction and d * axis.x in the right direction.
    Vector3 newPosition = transform.position + d * axis.y * f + d * axis.x * r;
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
    //float d = zoom * zoomSpeed * PlayerState.Instance.Settings.ZoomSensitivity * Time.unscaledDeltaTime;
    float d = zoom * zoomSpeed * PlayerState.Instance.Settings.ZoomSensitivity;

    float yMax = (maxHeight - p.y) / f.y;
    float yMin = (minHeight - p.y) / f.y;
    if (f.y < 0) {
      (yMin, yMax) = (yMax, yMin);
    }
    float xMax = (maxX + buffer - p.x) / f.x;
    float xMin = (minX - buffer - p.x) / f.x;
    if (f.x < 0) {
      (xMin, xMax) = (xMax, xMin);
    }
    float zMax = (maxZ + buffer - p.z) / f.z;
    float zMin = (minZ - buffer - p.z) / f.z;
    if (f.z < 0) {
      (zMin, zMax) = (zMax, zMin);
    }
    float dMax = Math.Min(Math.Min(yMax, xMax), zMax);
    float dMin = Math.Max(Math.Max(yMin, xMin), zMin);
    
    d = Math.Clamp(d, dMin, dMax);
    // Translate the position in the f direction a distance d.
    transform.position += d * f;
  }


  public void RotateCamera(Vector2 rotation) {
    turn.x += GetAngle(rotation.x);
    turn.y += GetAngle(rotation.y);
    turn.y = Math.Clamp(turn.y, 0, 90);
    transform.localRotation = Quaternion.Euler(turn.y, turn.x, 0);
  }

  private float GetAngle(float rotation) {
    return rotation * rotationSpeed;
  }

  public void ChangeElevation(float delta) {
    Vector3 pos = transform.position;
    pos.y += delta * Time.unscaledDeltaTime * elevationSpeed;
    pos.y = Math.Clamp(pos.y, minHeight, maxHeight);
    transform.position = pos;
  }
}
