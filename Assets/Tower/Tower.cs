using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
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
    { TowerData.Stat.PROJECTILE_SPEED, 0.0f },
  };
  protected Dictionary<ParticleSystem.Particle, Enemy> attacks = new();
  // How close a particle needs to get to consider it a hit.
  protected float hitRange = 0.1f;

  public float AttackSpeed {
    get { return attributes[TowerData.Stat.ATTACK_SPEED]; }
    set { attributes[TowerData.Stat.ATTACK_SPEED] = value; } }
  public float AreaOfEffect {
    get { return attributes[TowerData.Stat.AREA_OF_EFFECT]; }
    set { attributes[TowerData.Stat.AREA_OF_EFFECT] = value; } }
  public float ArmorPierce {
    get { return attributes[TowerData.Stat.ARMOR_PIERCE]; }
    set { attributes[TowerData.Stat.ARMOR_PIERCE] = value; } }
  public float ArmorTear {
    get { return attributes[TowerData.Stat.ARMOR_TEAR]; }
    set { attributes[TowerData.Stat.ARMOR_TEAR] = value; } }
  public float Damage {
    get { return attributes[TowerData.Stat.DAMAGE]; }
    set { attributes[TowerData.Stat.DAMAGE] = value; } }
  public float DamageOverTime {
    get { return attributes[TowerData.Stat.DAMAGE_OVER_TIME]; }
    set { attributes[TowerData.Stat.DAMAGE_OVER_TIME] = value; } }
  public float Range {
    get { return attributes[TowerData.Stat.RANGE]; }
    set { attributes[TowerData.Stat.RANGE] = value; } }
  public float ProjectileSpeed {
    get { return attributes[TowerData.Stat.PROJECTILE_SPEED]; }
    set { attributes[TowerData.Stat.PROJECTILE_SPEED] = value; } }

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

  protected virtual void processParticleCollision(Enemy target) {}

  protected void GeneralAttackHandler(ParticleSystem activeParticleSystem, Enemy target, float projectileSpeed) {
    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[activeParticleSystem.main.maxParticles];
    int numActiveParticles = activeParticleSystem.GetParticles(particles);
    Vector3 targetPosition = target?.transform.GetChild(0).position ?? Vector3.zero;

    // Code intending to change particle position/behavior must use particles[i] rather than a helper variable.
    for (int i = 0; i < numActiveParticles; i++) {
      particles[i].velocity = Vector3.zero;

      // Obtain the direction of travel
      Vector3 vec = targetPosition - particles[i].position;
      float dist = vec.magnitude;
      Vector3 deltaTravel = vec.normalized;
      // Make distance traveled frame rate independent and ensure we cannot 'overshoot' a target.
      deltaTravel *= Mathf.Min(Time.deltaTime * projectileSpeed, dist);
      particles[i].position += deltaTravel;

      if (Vector3.Distance(targetPosition, particles[i].position) < hitRange) {
        processParticleCollision(target);
        particles[i].remainingLifetime = 0;
      }
    }

    // Update all particle positions.
    activeParticleSystem.SetParticles(particles, numActiveParticles);
  }
}
