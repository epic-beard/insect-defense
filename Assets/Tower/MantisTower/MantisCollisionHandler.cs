using UnityEngine;

public class MantisCollisionHandler : MonoBehaviour {
  [SerializeField] MantisTower.MantisAttackType mantisAttackType;

  MantisTower mantisTower;

  private void Start() {
    mantisTower = GetComponentInParent<MantisTower>();
  }
  private void OnTriggerEnter(Collider other) {
    mantisTower.ProcessDamageAndEffects(other.GetComponentInParent<Enemy>(), mantisAttackType);
  }
}
