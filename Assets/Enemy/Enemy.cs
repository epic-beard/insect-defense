using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

  public void damageEnemy(float damage) {
    // Mark damage this enemy has taken.
  }

  // Return the new armor total after the tear is applied.
  // TODO: Complete this method
  public float TearArmor(float tear) {
    return 0.0f;
  }

  // TODO: Complete this method
  public float GetArmor() {
    return 0.0f;
  }

  // Returns true if stacks are at max.
  // TODO: Complete this method
  public bool AddAcidStacks(float stacks) {
    return true;
  }

  // TODO: Complete this method
  public bool IsAcidDOTMax() {
    return true;
  }
}
