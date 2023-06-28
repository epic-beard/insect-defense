using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
  public float SecondarySlowPotency {
    get { return data[TowerData.Stat.SECDONARY_SLOW_POTENCY]; }
    set { data[TowerData.Stat.SECDONARY_SLOW_POTENCY] = value; }
  }
  public int SecondarySlowTargets {
    get { return (int)data[TowerData.Stat.SECONDARY_SLOW_TARGETS]; }
    set { data[TowerData.Stat.SECONDARY_SLOW_TARGETS] = value; }
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
  public abstract TowerData.Type TowerType { get; set; }

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
  protected int[] upgradeLevels = new int[] { 0, 0, 0 };  // Each entry in this array should be 0-5.

  // How close a particle needs to get to consider it a hit.
  public readonly static float hitRange = 0.1f;

  // TODO: Add an enforcement mechanic to make sure the player follows the 5-3-1 structure.
  public void Upgrade(TowerAbility ability) {
    if (ability.specialAbility != TowerAbility.SpecialAbility.NONE) {
      SpecialAbilityUpgrade(ability.specialAbility);
    }

    foreach (TowerAbility.AttributeModifier modifier in ability.attributeModifiers) {
      switch (modifier.mode) {
        case TowerAbility.Mode.MULTIPLICATIVE:
          data[modifier.attribute] *= modifier.mod;
          break;
        case TowerAbility.Mode.ADDITIVE:
          data[modifier.attribute] += modifier.mod;
          break;
        case TowerAbility.Mode.SET:
          data[modifier.attribute] = modifier.mod;
          break;
      }
    }

    upgradeLevels[ability.upgradePath]++;
  }

  public abstract void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability);

  protected abstract void ProcessDamageAndEffects(Enemy target);

  // Fetch enemies in explosionRange of target. This excludes target itself.
  protected List<Enemy> GetEnemiesInExplosionRange(HashSet<Enemy> enemiesInRange, Enemy target, float explosionRange) {
    return enemiesInRange
          .Where(e => Vector3.Distance(e.transform.position, target.transform.position) < explosionRange)
          .Where(e => !e.Equals(target))
          .ToList();
  }
}
                