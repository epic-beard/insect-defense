using System.Collections;
using UnityEngine;

public class LabCamera : MonoBehaviour {
  // Initial state of the Camera
  [SerializeField] private Vector3 cameraLocation;
  [SerializeField] private Quaternion cameraRotation;
  [SerializeField] private float cameraSpeed = 100;
  // Start is called before the first frame update
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
    Debug.Log("coroutine");
    Vector3 startingPosition = transform.position;
    Quaternion startingRotation = transform.rotation;
    float travelPercent = 0.0f;

    while (travelPercent < 1.0f) {
      travelPercent += Time.deltaTime * cameraSpeed;
      Debug.Log("deltaTime: " + Time.deltaTime);
      Debug.Log("cameraSpeed: " + cameraSpeed);
      transform.SetPositionAndRotation(
        Vector3.Lerp(startingPosition, endPosition, travelPercent),
        Quaternion.Slerp(startingRotation, endRotation, travelPercent));
      Debug.Log("moving: " + transform.position);
      Debug.Log("travelPercent: " + travelPercent);
      yield return null;
    }
  }
}
