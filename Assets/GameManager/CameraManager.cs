using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour {
  public static CameraManager Instance;

  private Camera camera;
  // This is how much padding, in world space, we give to the map.
  // This value was made up whole cloth without much thought, but it doesn't look bad.
  private float screenOffset = 20;
  // These act as sentinels for our camera, they mark the extents in each of the four
  // screen directions.
  private Vector3 minX, minZ, maxX, maxZ;
  // The smallest size value allowed.
  private float minSize = 1.0f;

  private static readonly int speed = 100;
  private static readonly float zoomSpeed = 3;

  private void Awake() {
    Instance = this;
    camera = GetComponent<Camera>();
    SetExtents();
    CenterX(minX, maxX);
    CenterY(minZ, maxZ);
  }

  private void SetExtents() {
    var positions = FindObjectsOfType<Tile>().Select((tile)=>tile.transform.position);
    minX.x = positions.Min(p => p.x) - screenOffset;
    maxX.x = positions.Max(p => p.x) + screenOffset;

    minZ.z = positions.Min(p => p.z) - screenOffset;
    maxZ.z = positions.Max(p => p.z) + screenOffset;
  } 

  public void MoveCamera(Vector2 axis) {
    // Check if the sentinels are in view.
    if (camera.WorldToScreenPoint(minX).x > 0.0f && axis.x < 0.0f) { axis.x = 0.0f; }
    if (camera.WorldToScreenPoint(maxX).x < Screen.width && axis.x > 0.0f) { axis.x = 0.0f; }

    if (camera.WorldToScreenPoint(minZ).y > 0.0f && axis.y < 0.0f) { axis.y = 0.0f; }
    if (camera.WorldToScreenPoint(maxZ).y < Screen.height && axis.y > 0.0f) { axis.y = 0.0f; }

    Vector3 delta = new Vector3(axis.x, 0.0f, axis.y) * Time.deltaTime;
    Vector3 position = transform.position 
      + PlayerState.Instance.Settings.CameraSensitivity * speed * delta;
    transform.position = position;
  }

  // Update the camera zoom, using the sentinels to position the camera.
  public void ZoomCamera(float zoom) {
    // If all four sentinels are on the screen then dissable zooming out.
    if (IsOnScreenX(minX) && IsOnScreenX(maxX) &&
        IsOnScreenY(minZ) && IsOnScreenY(maxZ) && zoom < 0.0f) { return; }

    // Actually update the size of the camera.
    camera.orthographicSize += - PlayerState.Instance.Settings.ZoomSensitivity * zoom * zoomSpeed * Time.deltaTime;

    // If we're too far zoomed in, go no further.
    camera.orthographicSize = Mathf.Max(camera.orthographicSize, minSize);

    // If both x sentinels are on screen center in the x direction.
    if (IsOnScreenX(minX) && IsOnScreenX(maxX)) {
      CenterX(minX, maxX);
      // If minX is on screen move right
    } else if (IsOnScreenX(minX)) {
      Vector3 v = ScreenToWorld(Vector3.zero);
      Vector3 p = transform.position;
      p.x += minX.x - v.x;
      transform.position = p;
      // If we moved maxX onto screen, center.
      if (IsOnScreenX(maxX)) {
        CenterX(minX, maxX);
      }
    // If maxX is on screen move left.
    } else if (IsOnScreenX(maxX)) {
      Vector3 v = ScreenToWorld(Screen.width * Vector3.right);
      Vector3 p = transform.position;
      p.x += maxX.x - v.x;
      transform.position = p;
      // If we moved minX onto screen, center.
      if (IsOnScreenX(minX)) {
        CenterX(minX, maxX);
      }
    }

    // If both z sentinels are on screen center.
    if (IsOnScreenY(minZ) && IsOnScreenY(maxZ)) {
      CenterY(minZ, maxZ);
    // If minZ is on screen move up.
    } else if (IsOnScreenY(minZ)) {
      Vector3 v = ScreenToWorld(Vector3.zero);
      Vector3 p = transform.position;
      p.z += minZ.z - v.z;
      transform.position = p;

      // If we moved maxZ onto screen, center.
      if (IsOnScreenY(maxZ)) {
        CenterY(minZ, maxZ);
      }
    // If maxZ is on screen move down.
    } else if (IsOnScreenY(maxZ)) {
      Vector3 v = ScreenToWorld(Screen.height * Vector3.up);
      Vector3 p = transform.position;
      p.z += maxZ.z - v.z;
      transform.position = p;

      // If we moved minZ onto screen, center.
      if (IsOnScreenY(minZ)) {
        CenterY(minZ, maxZ);
      }
    }
  }

  // Unproject a screen point onto the x,z plane centered at zero.
  private Vector3 ScreenToWorld(Vector3 screenVector) {
    Ray ray = camera.ScreenPointToRay(screenVector);
    Plane plane = new(Vector3.up, Vector3.zero);
    plane.Raycast(ray, out var x);
    return ray.GetPoint(x);
  }

  // Center the screen in the x direction on the average of worldMin and worldMax.
  private void CenterX(Vector3 worldMin, Vector3 worldMax) {
    Vector3 min = ScreenToWorld(Vector3.zero);
    Vector3 max = ScreenToWorld(Screen.width * Vector3.right);
    float delta = (worldMax.x + worldMin.x) / 2 - ((max.x + min.x)) / 2;
    Vector3 p = transform.position;
    p.x += delta;
    transform.position = p;
  }

  // Center the screen in the y direction on the average of worldMin and worldMax.
  private void CenterY(Vector3 worldMin, Vector3 worldMax) {
    Vector3 min = ScreenToWorld(Vector3.zero);
    Vector3 max = ScreenToWorld(Screen.height * Vector3.up);
    float delta = (worldMax.z + worldMin.z) / 2 - ((max.z + min.z)) / 2;
    Vector3 p = transform.position;
    p.z += delta;
    transform.position = p;
  }

  // Checks if a point in world space fits into the x extents of the screen.
  private bool IsOnScreenX(Vector3 worldVector) {
    var v = camera.WorldToScreenPoint(worldVector);
    return v.x > 0.0f && v.x < Screen.width;
  }

  // Checks if a point in world space fits into the y extents of the screen.
  private bool IsOnScreenY(Vector3 worldVector) {
    var v = camera.WorldToScreenPoint(worldVector);
    return v.y > 0.0f && v.y < Screen.height;
  }
}
