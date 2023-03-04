using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour {
  protected Dictionary<TowerData.Stat, float> attributes = new() {
    { TowerData.Stat.ATTACK_SPEED, 0.0f },
    { TowerData.Stat.AREA_OF_EFFECT, 0.0f },
    { TowerData.Stat.ARMOR_PIERCE, 0.0f },
    { TowerData.Stat.ARMOR_TEAR, 0.0f },
    { TowerData.Stat.DAMAGE, 0.0f },
    { TowerData.Stat.DAMAGE_OVER_TIME, 0.0f },
    { TowerData.Stat.RANGE, 0.0f },
    { TowerData.Stat.PROJECTILE_SPEED, 0.0f },
    { TowerData.Stat.SLOW_DURATION, 0.0f },
    { TowerData.Stat.SLOW_POWER, 0.0f },
    { TowerData.Stat.STUN_TIME, 0.0f },
  };
  public float AttackSpeed {
    get { return attributes[TowerData.Stat.ATTACK_SPEED]; }
    set { attributes[TowerData.Stat.ATTACK_SPEED] = value; }
  }
  public float AreaOfEffect {
    get { return attributes[TowerData.Stat.AREA_OF_EFFECT]; }
    set { attributes[TowerData.Stat.AREA_OF_EFFECT] = value; }
  }
  public float ArmorPierce {
    get { return attributes[TowerData.Stat.ARMOR_PIERCE]; }
    set { attributes[TowerData.Stat.ARMOR_PIERCE] = value; }
  }
  public float ArmorTear {
    get { return attributes[TowerData.Stat.ARMOR_TEAR]; }
    set { attributes[TowerData.Stat.ARMOR_TEAR] = value; }
  }
  public float Damage {
    get { return attributes[TowerData.Stat.DAMAGE]; }
    set { attributes[TowerData.Stat.DAMAGE] = value; }
  }
  public float DamageOverTime {
    get { return attributes[TowerData.Stat.DAMAGE_OVER_TIME]; }
    set { attributes[TowerData.Stat.DAMAGE_OVER_TIME] = value; }
  }
  public float Range {
    get { return attributes[TowerData.Stat.RANGE]; }
    set { attributes[TowerData.Stat.RANGE] = value; }
  }
  public float ProjectileSpeed {
    get { return attributes[TowerData.Stat.PROJECTILE_SPEED]; }
    set { attributes[TowerData.Stat.PROJECTILE_SPEED] = value; }
  }
  public float SlowDuration {
    get { return attributes[TowerData.Stat.SLOW_DURATION]; }
    set { attributes[TowerData.Stat.SLOW_DURATION] = value; }
  }
  public float SlowPower {
    get { return attributes[TowerData.Stat.SLOW_POWER]; }
    set { attributes[TowerData.Stat.SLOW_POWER] = value; }
  }
  public float StunTime {
    get { return attributes[TowerData.Stat.STUN_TIME]; }
    set { attributes[TowerData.Stat.STUN_TIME] = value; }
  }

  protected Dictionary<TowerData.TowerAbility, bool> towerAbilities = new() {
    { TowerData.TowerAbility.ANTI_AIR, false },
    { TowerData.TowerAbility.CAMO_SIGHT, false },
    { TowerData.TowerAbility.CRIPPLE, false }
  };
  public bool AntiAir {
    get { return towerAbilities[TowerData.TowerAbility.ANTI_AIR]; }
    set { towerAbilities[TowerData.TowerAbility.ANTI_AIR] = value; }
  }
  public bool CamoSight {
    get { return towerAbilities[TowerData.TowerAbility.CAMO_SIGHT]; }
    set { towerAbilities[TowerData.TowerAbility.CAMO_SIGHT] = value; }
  }
  public bool Cripple {
    get { return towerAbilities[TowerData.TowerAbility.CRIPPLE]; }
    set { towerAbilities[TowerData.TowerAbility.CRIPPLE] = value; }
  }
  int[] upgradeLevels = new int[] { 0, 0, 0 };  // Each entry in this array should be 0-4.

  // How close a particle needs to get to consider it a hit.
  protected float hitRange = 0.1f;

  protected Dictionary<Int64, Enemy> particleIDsToEnemies = new();
  protected Int64 particleIDTracker = 100;

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

  public abstract void SpecialAbilityUpgrade(Ability.SpecialAbilityEnum ability);

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
