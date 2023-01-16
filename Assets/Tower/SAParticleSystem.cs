using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// The implementation here has some serious bugs. AoE strikes won't work correctly.
public class SAParticleSystem : MonoBehaviour {
  SpittingAntTower origin;

  void Start() {
    origin = transform.GetComponentInParent<SpittingAntTower>();
  }

  private void OnParticleCollision(GameObject other) {
    float onHitDamage = origin.GetOnHitDamage();
    float damageOverTime = origin.GetDamageOverTime();
    Enemy enemy = other.GetComponentInChildren<Enemy>();

    // Armor tear effects.
    if (origin.AcidStun && enemy.TearArmor(origin.GetArmorTear()) == 0.0f) {
      // Stun the enemy.
    }
    if (origin.TearBonusDamage && enemy.GetArmor() == 0.0f) {
      onHitDamage *= origin.GetArmorTear();
      damageOverTime *= origin.GetArmorTear();
    }

    // DoT effects.
    if (enemy.AddAcidStacks(damageOverTime)) {
      if (origin.DotSlow) {
        // Apply a slow to the enemy unless the enemy is already slowed.
      }
      if (origin.DotExplosion) {
        // Trigger an explosion.
        // Reset acid stacks to 0.
      }
    }

    enemy.damageEnemy(onHitDamage);
  }
}
