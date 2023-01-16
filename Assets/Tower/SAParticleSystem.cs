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
    float onHitDamage = origin.getOnHitDamage();
    float damageOverTime = origin.getDamageOverTime();
    Enemy enemy = other.GetComponentInChildren<Enemy>();

    // Armor tear effects.
    if (origin.AcidStun && enemy.TearArmor(origin.getArmorTear()) == 0.0f) {
      // Stun the enemy.
    }
    if (origin.TearBonusDamage && enemy.GetArmor() == 0.0f) {
      onHitDamage *= origin.getArmorTear();
      damageOverTime *= origin.getArmorTear();
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
