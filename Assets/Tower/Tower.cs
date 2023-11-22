using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Tower : MonoBehaviour {
  [SerializeField] protected TowerData data;

  #region PublicProperties
  public float AttackSpeed {
    get { return data[TowerData.Stat.ATTACK_SPEED]; }
    set { data[TowerData.Stat.ATTACK_SPEED] = value; }
  }
  public float EffectiveAttackSpeed {
    get { return AttackSpeed * SlimePower; }
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
  public float DazzleTime { get; set; }
  public virtual int EnemiesHit {
    get { return data.enemies_hit; }
    set { data.enemies_hit = value; }
  }
  public string Name {
    get { return data.name; }
    set { data.name = value; }
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
    get { return data[TowerData.Stat.SECONDARY_SLOW_POTENCY]; }
    set { data[TowerData.Stat.SECONDARY_SLOW_POTENCY] = value; }
  }
  public int SecondarySlowTargets {
    get { return (int)data[TowerData.Stat.SECONDARY_SLOW_TARGETS]; }
    set { data[TowerData.Stat.SECONDARY_SLOW_TARGETS] = value; }
  }
  public float SlimeTime { get; set; }
  public float SlimePower { get; set; } = 1.0f;
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
  #endregion

  protected int[] upgradeLevels = new int[] { 0, 0, 0 };  // Each entry in this array should be 0-4.
  public int[] UpgradeLevels { get { return upgradeLevels; } }

  // How close a particle needs to get to consider it a hit.
  public readonly static float hitRange = 0.1f;

  protected Targeting targeting = new();
  public Targeting.Behavior Behavior {
    get { return targeting.behavior; }
    set { targeting.behavior = value; }
  }
  public Targeting.Priority Priority {
    get { return targeting.priority; }
    set { targeting.priority = value; }
  }

  // Get the ugprade path name corresponding to the given index. No value other than 0, 1, 2 should be passed in.
  public string GetUpgradePathName(int index) {
    return index switch {
      0 => data.upgradeTreeData.first,
      1 => data.upgradeTreeData.second,
      2 => data.upgradeTreeData.third,
      _ => "[ERROR] Bad upgrade path index: " + index,
    };
  }

  // Get the upgrade path corresponding to the given index. No value other than 0, 1, 2 should be passed in.
  public TowerAbility[] GetUpgradePath(int index) {
    return index switch {
      0 => data.upgradeTreeData.firstPathUpgrades,
      1 => data.upgradeTreeData.secondPathUpgrades,
      2 => data.upgradeTreeData.thirdPathUpgrades,
      _ => null,
    };
  }

  private void Update() {
    TowerUpdate();
  }

  // Abstract methods
  protected abstract void TowerUpdate();
  public abstract TowerData.Type TowerType { get; set; }
  public abstract void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability);

  // TODO: Add an enforcement mechanic to make sure the player follows the 5-3-1 structure.
  public void Upgrade(TowerAbility ability) {
    if (ability.specialAbility != TowerAbility.SpecialAbility.NONE) {
      SpecialAbilityUpgrade(ability.specialAbility);
    }

    if (ability.attributeModifiers != null) {
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
    }

    upgradeLevels[ability.upgradePath]++;
  }

  // Fetch enemies in explosionRange of target. This excludes target itself.
  protected List<Enemy> GetEnemiesInExplosionRange(HashSet<Enemy> enemiesInRange, Enemy target, float explosionRange) {
    return enemiesInRange
          .Where(e => Vector3.Distance(e.transform.position, target.transform.position) < explosionRange)
          .Where(e => !e.Equals(target))
          .ToList();
  }

  public void SetTowerData(TowerData data) {
    this.data = data;
  }

  public void ApplyDazzle(float duration) {
    if (DazzleTime > 0) {
      DazzleTime = Mathf.Max(DazzleTime, duration);
    } else {
      DazzleTime = duration;
      StartCoroutine(HandleDazzle());
    }
  }

  private IEnumerator HandleDazzle() {
    while (DazzleTime > 0) {
      yield return null;
      DazzleTime -= Time.deltaTime;
    }

    DazzleTime = 0.0f;
    yield return null;
  }

  public void ApplySlime(float duration, float power) {
    if (SlimeTime > 0) {
      SlimeTime = Mathf.Max(SlimeTime, duration);
      SlimePower = Mathf.Max(SlimePower, power);
    } else {
      SlimeTime = duration;
      SlimePower = power;
      StartCoroutine(HandleSlime());
    }
  }

  private IEnumerator HandleSlime() {
    while (SlimeTime > 0) {
      yield return null;
      SlimeTime -= Time.deltaTime;
    }
    SlimePower = 1.0f;
    SlimeTime = 0.0f;
    yield return null;
  }

  public override string ToString() {
    return Name + "\n"
        + "  Area of effect: " + AreaOfEffect + "\n"
        + "  Armor piercing: " + ArmorPierce + "\n"
        + "  Armor tear: " + ArmorTear + "\n"
        + "  Attack speed: " + AttackSpeed + "\n"
        + "  Damage: " + Damage + "\n"
        + "  Damage over time: " + DamageOverTime + "\n"
        + "  Projectile speed: " + ProjectileSpeed + "\n"
        + "  Range: " + Range + "\n"
        + "  Secondary slow potency: " + SecondarySlowPotency + "\n"
        + "  Secondary slow targets: " + SecondarySlowTargets + "\n"
        + "  Slow duration: " + SlowDuration + "\n"
        + "  Slow power: " + SlowPower + "\n"
        + "  Stun time: " + StunTime + "\n"
        + "  Upgrade Tree Data: " + data.upgradeTreeData.ToString() + "\n";
  }
}
