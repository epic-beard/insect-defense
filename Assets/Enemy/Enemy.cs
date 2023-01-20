using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  public Waypoint PrevWaypoint;
  public Waypoint NextWaypoint;
  public float Speed = 10;

  // PrevWaypoint should be set before OnEnable is called.
  void OnEnable() {
    NextWaypoint = PrevWaypoint.GetNextWaypoint();

    if (NextWaypoint == null) {
      Debug.Log("[ERROR] NextWaypoint not found.");
      return;
    }

    StartCoroutine(FollowPath());
  }

  public void DamageEnemy(float damage) {
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

  // TODO: Complete this method
  public float GetSpeed() {
    return Speed;
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

  private IEnumerator FollowPath() {
    while (NextWaypoint != null) {
      Vector3 startPosition = transform.position;
      Vector3 endPosition = NextWaypoint.transform.position;
      float travelPercent = 0.0f;

      transform.LookAt(endPosition);

      while (travelPercent < 1.0f) {
        travelPercent += Time.deltaTime * GetSpeed();
        transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
        yield return null;
      }

      PrevWaypoint = NextWaypoint;
      NextWaypoint = PrevWaypoint.GetNextWaypoint();
    }

    FinishPath();
  }

  // TODO nnewsom: Implement this when needed.
  private void FinishPath() { }
}
