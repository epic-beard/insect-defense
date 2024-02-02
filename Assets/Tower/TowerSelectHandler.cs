using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSelectHandler : MonoBehaviour {

  SpittingAntTower spittingAntTower;

  private void Start() {
    spittingAntTower = GetComponentInParent<SpittingAntTower>();
  }

  private void OnMouseUp() {
    //GameStateManager.Instance.SetNewSelectedTower(spittingAntTower);
  }
}
