using System;
using System.Collections;
using UnityEngine;

public class LabCamera : MonoBehaviour {
  static public LabCamera Instance;

  // Initial state of the Camera
  [SerializeField] private Vector3 cameraLocation;
  [SerializeField] private Quaternion cameraRotation;
  [SerializeField] private float cameraSpeed = 100;

  private void Awake() {
    Instance = this;
  }

  void OnEnable() {
    cameraLocation = transform.position;
    cameraRotation = transform.rotation;
  }

  public void MoveTo(Vector3 position, Quaternion rotation, Action onComplete) {
    StartCoroutine(MoveCamera(position, rotation, onComplete));
  }

  public void MoveToFocus(Vector3 position, Quaternion rotation) {
    LabState.Instance.IsFocused = true;
    MoveTo(position, rotation, () => FocusedUI.Instance.OpenScreen());
    LabInputManager.Instance.EnableSelectedActionMap();
  }

  public void MoveToTerrarium(Terrarium terrarium) {
    LabState.Instance.IsFocused = true;
    Vector3 pos = terrarium.transform.position + Vector3.forward * terrarium.cameraOffsetZ; 
    MoveTo(pos, terrarium.transform.rotation,
      () => lab.TerrariumUI.Instance.OpenScreen(terrarium));
  }

  public void MoveToTerrarium(Terrarium terrarium, Action onComplete) {
    LabState.Instance.IsFocused = true;
    Vector3 pos = terrarium.transform.position + Vector3.forward * terrarium.cameraOffsetZ;
    MoveTo(pos, terrarium.transform.rotation, onComplete);
  }

  public void ReturnCamera() {
    MoveTo(cameraLocation, cameraRotation, ()=> LabState.Instance.IsFocused = false);
    LabInputManager.Instance.EnableLabActionMap();
  }

  private IEnumerator MoveCamera(Vector3 endPosition, Quaternion endRotation, Action onComplete) {
    Vector3 startingPosition = transform.position;
    Quaternion startingRotation = transform.rotation;
    float travelPercent = 0.0f;

    while (travelPercent < 1.0f) {
      travelPercent += Time.deltaTime * cameraSpeed;
      transform.SetPositionAndRotation(
        Vector3.Lerp(startingPosition, endPosition, travelPercent),
        Quaternion.Slerp(startingRotation, endRotation, travelPercent));
      yield return null;
    }
    onComplete();
  }
}
