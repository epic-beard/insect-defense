using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class Tower : MonoBehaviour {
  protected Dictionary<TowerData.Stat, float> attributes = new() {
    { TowerData.Stat.ATTACK_SPEED, 0.0f },
    { TowerData.Stat.AREA_OF_EFFECT, 0.0f },
    { TowerData.Stat.ARMOR_PIERCE, 0.0f },
    { TowerData.Stat.ARMOR_TEAR, 0.0f },
    { TowerData.Stat.DAMAGE, 0.0f },
    { TowerData.Stat.DAMAGE_OVER_TIME, 0.0f },
    { TowerData.Stat.RANGE, 0.0f },
  };
  protected Dictionary<ParticleSystem.Particle, Enemy> attacks = new();

  public float AttackSpeed { get { return attributes[TowerData.Stat.ATTACK_SPEED]; } }
  public float AreaOfEffect { get { return attributes[TowerData.Stat.AREA_OF_EFFECT]; } }
  public float ArmorPierce { get { return attributes[TowerData.Stat.ARMOR_PIERCE]; } }
  public float ArmorTear { get { return attributes[TowerData.Stat.ARMOR_TEAR]; } }
  public float Damage { get { return attributes[TowerData.Stat.DAMAGE]; } }
  public float DamageOverTime { get { return attributes[TowerData.Stat.DAMAGE_OVER_TIME]; } }
  public float Range { get { return attributes[TowerData.Stat.RANGE]; } }

  protected Dictionary<TowerData.TowerAbility, bool> towerAbilities = new() {
    { TowerData.TowerAbility.ANTI_AIR, false },
    { TowerData.TowerAbility.CAMO_SIGHT, false },
    { TowerData.TowerAbility.CRIPPLE, false }
  };
  int[] upgradeLevels = new int[] { 0, 0, 0 };  // Each entry in this array should be 0-4.

  // TODO: Add an enforcement mechanic to make sure the player can get at most 9 upgrades.
  public void Upgrade(Ability ability) {
    if (ability.Mode == TowerData.Mode.SPECIAL) {
      SpecialAbilityUpgrade(ability.SpecialAbility);
    } else {
      foreach (Ability.AttributeModifier mod in ability.AttributeModifiers) {
        attributes[mod.attribute] *= mod.mult;
      }
    }

    upgradeLevels[ability.UpgradePath]++;
  }

  public virtual void SpecialAbilityUpgrade(Ability.SpecialAbilityEnum ability) {}

  protected virtual void processParticleCollision() {}

  private void LateUpdate() {
    // TODO for the processing we want:
    //  1. Iterate through attacks.
    //  2. For each attack, check to see if it is 'close enough' to the enemy to trigger 'contact'
    //  3. When contact is triggered, call a virtual method each sub tower will implmenent.
  }

  protected void GeneralAttackHandler(ParticleSystem activeParticleSystem, Enemy target) {
    ParticleSystem.Particle[] particles = null;
    int numActiveParticles = activeParticleSystem.GetParticles(particles);
    Vector3 targetPosition = target.transform.position;

    for (int i = 0; i < numActiveParticles; i++) {
      float cursor = 0.5f;

      particles[i].velocity = Vector3.zero;
      particles[i].position = Vector3.Lerp(particles[i].position, targetPosition, cursor);
      // m_rParticlesArray[iParticle].position = Vector3.Lerp(m_rParticlesArray[iParticle].position, m_vParticlesTarget, m_fCursor * m_fCursor);
    }
  }
}
