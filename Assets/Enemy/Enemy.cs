using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour {
  public float armor = 0.0f;
  public float hp = 0.0f;
  public Vector3 position = Vector3.zero;
  public bool isFlier = false;
  public bool isCamo = false;

  public void DamageEnemy(float damage) {
    // Mark damage this enemy has taken.
  }

  // Return the new armor total after the tear is applied.
  public float TearArmor(float tear) {
    return 0.0f;
  }

  public float GetArmor() {
    return armor;
  }

  // Returns true if stacks are at max.
  public bool AddAcidStacks(float stacks) {
    return true;
  }

  public bool IsAcidDOTMax() {
    return true;
  }

  public float GetHP() {
    return hp;
  }

  public bool IsCamo() {
    return isCamo;
  }

  public bool IsFlier() {
    return isFlier;
  }

  public Vector3 getPosition() {
    return position;
  }
}
