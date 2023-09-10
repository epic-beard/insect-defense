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

  public void ReturnCamera() {
    MoveTo(cameraLocation, cameraRotation);
  }

  public void MoveTo(Vector3 position) {
    MoveTo(position, transform.rotation);
  }

  public void MoveTo(Vector3 position, Quaternion rotation) {
    StartCoroutine(MoveCamera(position, rotation));
  }

  private IEnumerator MoveCamera(Vector3 endPosition, Quaternion endRotation) {
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
  }
}
