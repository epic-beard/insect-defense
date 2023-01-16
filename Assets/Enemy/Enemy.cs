using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

  public void damageEnemy(float damage) {
    // Mark damage this enemy has taken.
  }

  // Return the new armor total after the tear is applied.
  public float TearArmor(float tear) {
    return 0.0f;
  }

  public float GetArmor() {
    return 0.0f;
  }

  // Returns true if stacks are at max.
  public bool AddAcidStacks(float stacks) {
    return true;
  }

  public bool IsAcidDOTMax() {
    return true;
  }
}