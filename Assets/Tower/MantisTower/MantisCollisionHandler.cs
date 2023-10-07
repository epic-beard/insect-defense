using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MantisCollisionHandler : MonoBehaviour {
  MantisTower mantisTower;

  private void Start() {
    mantisTower = GetComponentInParent<MantisTower>();
    if (mantisTower == null) {
      mantisTower = GetComponentInParent<MantisTower>(true);
    }

    //Debug.Log(mantisTower.ToString());
  }
  private void OnTriggerEnter(Collider other) {
    //Debug.Log("Mantis tower parent: " + mantisTower);
    Debug.Log("Collision detected with the sickle specifically.");
  }
}
