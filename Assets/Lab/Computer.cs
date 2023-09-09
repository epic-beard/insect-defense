using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour {
  [SerializeField] private Vector3 computerLocation;
  [SerializeField] private Quaternion computerRotation;
  private LabCamera camera;

  private void OnEnable() {
    camera = FindObjectOfType<LabCamera>();
  }

  private void OnMouseDown() {
    Debug.Log("mouse clicked");
    camera.MoveTo(computerLocation, computerRotation);
  }
}
