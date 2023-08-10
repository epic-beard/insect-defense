using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour {
  public static CameraManager Instance;

  private Camera camera;

  private static readonly int speed = 100;
  private static readonly float zoomSpeed = 3;

  private void Awake() {
    Instance = this;
    camera = GetComponent<Camera>();
  }
  public void MoveCamera(Vector2 axis) {
    Vector3 delta = new Vector3(-axis.y, 0.0f, axis.x) * Time.deltaTime;
    transform.position += PlayerState.Instance.Settings.CameraSensitivity * speed * delta;
  }

  public void ZoomCamera(float zoom) {
    camera.orthographicSize += - zoom * zoomSpeed * Time.deltaTime;
    //transform.Translate(Vector3.forward * zoom * zoomSpeed * Time.deltaTime);
  }
}
