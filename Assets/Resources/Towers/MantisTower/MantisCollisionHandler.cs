using UnityEngine;

public class MantisCollisionHandler : MonoBehaviour {
  [SerializeField] MantisTower.MantisAttackType mantisAttackType;

  MantisTower mantisTower;

  private void Start() {
    mantisTower = GetComponentInParent<MantisTower>();
  }
  private void OnTriggerEnter(Collider other) {
    Enemy enemy = other.GetComponentInParent<Enemy>();
    if (enemy == null) return;
    //mantisTower.ProcessDamageAndEffects(enemy, mantisAttackType);
  }
}
