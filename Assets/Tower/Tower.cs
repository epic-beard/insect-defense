using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour {
  [SerializeField] protected TowerData data;
  public float AttackSpeed {
    get { return data[TowerData.Stat.ATTACK_SPEED]; }
    set { data[TowerData.Stat.ATTACK_SPEED] = value; }
  }
  public float AreaOfEffect {
    get { return data[TowerData.Stat.AREA_OF_EFFECT]; }
    set { data[TowerData.Stat.AREA_OF_EFFECT] = value; }
  }
  public float ArmorPierce {
    get { return data[TowerData.Stat.ARMOR_PIERCE]; }
    set { data[TowerData.Stat.ARMOR_PIERCE] = value; }
  }
  public float ArmorTear {
    get { return data[TowerData.Stat.ARMOR_TEAR]; }
    set { data[TowerData.Stat.ARMOR_TEAR] = value; }
  }
  public float Damage {
    get { return data[TowerData.Stat.DAMAGE]; }
    set { data[TowerData.Stat.DAMAGE] = value; }
  }
  public float DamageOverTime {
    get { return data[TowerData.Stat.DAMAGE_OVER_TIME]; }
    set { data[TowerData.Stat.DAMAGE_OVER_TIME] = value; }
  }
  public float Range {
    get { return data[TowerData.Stat.RANGE]; }
    set { data[TowerData.Stat.RANGE] = value; }
  }
  public float ProjectileSpeed {
    get { return data[TowerData.Stat.PROJECTILE_SPEED]; }
    set { data[TowerData.Stat.PROJECTILE_SPEED] = value; }
  }
  public float SlowDuration {
    get { return data[TowerData.Stat.SLOW_DURATION]; }
    set { data[TowerData.Stat.SLOW_DURATION] = value; }
  }
  public float SlowPower {
    get { return data[TowerData.Stat.SLOW_POWER]; }
    set { data[TowerData.Stat.SLOW_POWER] = value; }
  }
  public float StunTime {
    get { return data[TowerData.Stat.STUN_TIME]; }
    set { data[TowerData.Stat.STUN_TIME] = value; }
  }

  protected Dictionary<TowerAbility.Type, bool> towerAbilities = new() {
    { TowerAbility.Type.ANTI_AIR, false },
    { TowerAbility.Type.CAMO_SIGHT, false },
    { TowerAbility.Type.CRIPPLE, false }
  };
  public bool AntiAir {
    get { return towerAbilities[TowerAbility.Type.ANTI_AIR]; }
    set { towerAbilities[TowerAbility.Type.ANTI_AIR] = value; }
  }
  public bool CamoSight {
    get { return towerAbilities[TowerAbility.Type.CAMO_SIGHT]; }
    set { towerAbilities[TowerAbility.Type.CAMO_SIGHT] = value; }
  }
  public bool Cripple {
    get { return towerAbilities[TowerAbility.Type.CRIPPLE]; }
    set { towerAbilities[TowerAbility.Type.CRIPPLE] = value; }
  }
  int[] upgradeLevels = new int[] { 0, 0, 0 };  // Each entry in this array should be 0-4.

  // How close a particle needs to get to consider it a hit.
  protected readonly static float hitRange = 0.1f;

  protected Dictionary<Int64, Enemy> particleIDsToEnemies = new();
  protected Int64 particleIDTracker = 100;

  // TODO: Add an enforcement mechanic to make sure the player can get at most 9 upgrades.
  public void Upgrade(TowerAbility ability) {
    if (ability.mode == TowerAbility.Mode.SPECIAL) {
      SpecialAbilityUpgrade(ability.specialAbility);
    } else {
      foreach (TowerAbility.AttributeModifier mod in ability.attributeModifiers) {
        data[mod.attribute] *= mod.mult;
      }
    }

    upgradeLevels[ability.upgradePath]++;
  }

  public abstract void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability);

  protected abstract void ProcessDamageAndEffects(Enemy target);

  // Handle individual particle movement. This method takes control of particle movement and collision initiation
  // from Unity.
  protected void GeneralAttackHandler(ParticleSystem activeParticleSystem, Enemy target, float projectileSpeed) {
    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[activeParticleSystem.main.maxParticles];
    int numActiveParticles = activeParticleSystem.GetParticles(particles);

    // Code intending to change particle position/behavior must use particles[i] rather than a helper variable.
    for (int i = 0; i < numActiveParticles; i++) {
      particles[i].velocity = Vector3.zero;

      // Add to the particle to enemy tracker if necessary. Tracking individual particles can be difficult because
      // ParticleSystem.GetParticles returns a value rather than a reference.
      if (particles[i].startLifetime < 100) {
        particles[i].startLifetime = particleIDTracker;
        particleIDsToEnemies.Add(particleIDTracker, target);
        particleIDTracker++;
      }

      // Destroy any particles targeting an enemy that is no longer alive.
      Enemy enemy = particleIDsToEnemies[(int)particles[i].startLifetime];
      if (!enemy.enabled) {
        particles[i].remainingLifetime = 0;
        continue;
      }
      Vector3 targetPosition = GetSafeChildPosition(enemy);

      // Obtain the direction of travel
      Vector3 vec = targetPosition - particles[i].position;
      float dist = vec.magnitude;
      Vector3 deltaTravel = vec.normalized;

      // Make distance traveled frame rate independent and ensure we cannot 'overshoot' a target.
      deltaTravel *= Mathf.Min(Time.deltaTime * projectileSpeed, dist);
      particles[i].position += deltaTravel;

      // Initiate particle 'collision'. Destroy the particle and call the tower's particle collision handler.
      if (Vector3.Distance(targetPosition, particles[i].position) < hitRange) {
        ProcessDamageAndEffects(enemy);
        particles[i].remainingLifetime = 0;
      }
    }

    // Update all particle positions.
    activeParticleSystem.SetParticles(particles, numActiveParticles);
  }

  // Get a safe position for the shots of the tower. Ideally, the actual mesh, but if that isn't present,
  // the enemy container itself.
  protected Vector3 GetSafeChildPosition(Enemy enemy) {
    if (enemy.transform.childCount == 0) {
      return enemy.transform.position;
    }
    return enemy.transform.GetChild(0).position;
  }
}
                