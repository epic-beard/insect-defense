using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MantisCollisionSickle : MonoBehaviour {
  private void OnTriggerEnter(Collider other) {
    Debug.Log("Collision detected with the sickle specifically.");
  }
}
