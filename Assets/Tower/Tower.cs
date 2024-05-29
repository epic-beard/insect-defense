using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets;

public abstract class Tower : MonoBehaviour {
  [SerializeField] protected TowerData data;
  [SerializeField] protected Transform targetingIndicator;
  [SerializeField] protected Transform rangeIndicator;

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
  public float BaseAttackSpeed { get; private set; }
  public float Cost {
    get { return data[TowerData.Stat.COST]; }
    set { data[TowerData.Stat.COST] = value; }
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
  public bool IsMutatingUpgrades { get; set; }
  public string Name {
    get { return data.name; }
    set { data.name = value; }
  }
  public float Range {
    get { return data[TowerData.Stat.RANGE]; }
    set { data[TowerData.Stat.RANGE] = value; }
  }
  public HashSet<Renderer> Renderers { get; private set; }
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
  public TowerData.Tooltip tooltip {
    get { return data.tooltip; }
  }
  public bool IsPreviewTower = false;

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
  public Tile Tile { get; set; }
  public Enemy Target { get; protected set; }

  protected float normalRangeHeight = 2.0f;
  protected float antiAirRangeHeight = 4.0f;

  // The total Nu the tower has cost the player.
  public int Value {
    get {
      int value = TowerManager.Instance.GetPreviousTowerCost(Type);

      foreach (var level in UpgradeIndex) {
        for (int i = 0; i <= level; i++) {
          value += GetUpgradePath(level)[i].cost;
        }
      }

      return value;
    }
  }

  public string IconPath { get { return data.icon_path; } set { data.icon_path = value; } }
  #endregion

  protected int[] upgradeIndex = new int[] { -1, -1, -1 };  // Each entry in this array should be -1-4.
  public int[] UpgradeIndex { get { return upgradeIndex; } }

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

  private MeshRenderer targetingIndicatorMeshRenderer;

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

  private void Start() {
    TowerStart();
    if (targetingIndicator != null) {
      targetingIndicatorMeshRenderer = targetingIndicator.GetComponentInChildren<MeshRenderer>();
    }
  }

  private void Update() {
    Enemy oldTarget = Target;
    TowerUpdate();
    RemoveTargetMark(oldTarget);
    MarkTarget(Target);
  }

  private void OnDestroy() {
    if (Tile != null) {
      Tile.ResetTile();
    }
  }

  // Abstract methods
  protected abstract void TowerUpdate();
  protected abstract void TowerStart();
  public abstract TowerData.Type Type { get; set; }
  public abstract void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability);
  // This method is intended to set the animation speed, thus newAttackSpeed is understood to be a
  // percentage modifier of the base animation speed. Thus a newAttackSpeed of 2.0 would have the
  // animation play twice as fast as the base speed.
  protected abstract void UpdateAnimationSpeed(float newAttackSpeed);

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
        if (modifier.attribute == TowerData.Stat.ATTACK_SPEED) {
          UpdateAnimationSpeed(GetAnimationSpeedMultiplier());
        }
        if (modifier.attribute == TowerData.Stat.RANGE) {
          rangeIndicator.localScale = new Vector3(Range, normalRangeHeight, Range);
        }
      }
    }

    upgradeIndex[ability.upgradePath]++;
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
    BaseAttackSpeed = data[TowerData.Stat.ATTACK_SPEED];
  }

  public void CacheTowerRenderers() {
    // Get only those renderers without the tag NoTowerTransluscencyChanges.
    Renderers = this.GetComponentsInChildren<Renderer>()
        .Where(r => !r.CompareTag("NoTowerTransluscencyChanges")).ToHashSet();

    foreach (Renderer r in Renderers) {
      r.AllMaterialsToFadeMode();
    }
  }

  public float GetAnimationSpeedMultiplier() {
    return SlimePower * AttackSpeed / BaseAttackSpeed;
  }

  public void ApplyDazzle(float duration) {
    float time = Time.time + duration;
    if (DazzleTime > Time.time) {
      DazzleTime = Mathf.Max(DazzleTime, time);
    } else {
      DazzleTime = time;
      StartCoroutine(HandleDazzle());
    }
  }

  public void ResetTransientState() {
    SlimePower = 1.0f;
    SlimeTime = Time.time;

    DazzleTime = Time.time;

    RemoveTargetMark(Target);
    Target = null;
    upgradeIndex = new int[] { -1, -1, -1 };
    SetRangeIndicatorScale();
  }

  private IEnumerator HandleDazzle() {
    float time = Time.time;
    while (DazzleTime > time) {
      yield return new WaitForSeconds(DazzleTime - time);
    }
    yield return null;
  }

  protected bool IsDazzled() {
    return DazzleTime > Time.time;
  }

  public void ApplySlime(float duration, float power) {
    float time = Time.time + duration;
    if (SlimeTime > Time.time) {
      SlimeTime = Mathf.Max(SlimeTime, time);
      SlimePower = Mathf.Min(SlimePower, power);
    } else {
      SlimeTime = time;
      SlimePower = power;
      StartCoroutine(HandleSlime());
    }
    UpdateAnimationSpeed(GetAnimationSpeedMultiplier());
  }

  private IEnumerator HandleSlime() {
    while (SlimeTime > Time.time) {
      yield return new WaitForSeconds(SlimeTime - Time.time);
    }
    SlimePower = 1.0f;
    UpdateAnimationSpeed(GetAnimationSpeedMultiplier());
    yield return null;
  }

  protected bool IsSlimed() {
    return SlimeTime > Time.time;
  }

  // Sets a targeting texture beneath the given enemy.
  private void MarkTarget(Enemy enemy) {
    if (targetingIndicator != null && enemy != null) {
      targetingIndicatorMeshRenderer.enabled = true;

      Bounds enemyBound = new();
      if (enemy.Renderers != null) {
        enemyBound = enemy.Renderers.Length == 0 ? new() : enemy.Renderers[0].bounds;
        foreach (Renderer r in enemy.Renderers) {
          enemyBound.Encapsulate(r.transform.position);
        }
      }

      float ratio = 1.5f;
      targetingIndicator.transform.position = new Vector3(enemyBound.center.x, 0, enemyBound.center.z);
      float radius = Mathf.Max(enemyBound.size.x, enemyBound.size.z);
      targetingIndicator.transform.localScale =
          new Vector3(
              radius * ratio,
              1,
              radius * ratio);

      targetingIndicator.LookAt(this.transform);
    }
  }

  // Hides the targeting texture.
  private void RemoveTargetMark(Enemy enemy) {
    if (targetingIndicator != null && enemy != null) {
      targetingIndicatorMeshRenderer.enabled = false;
    }
  }

  public void SetRangeIndicatorVisible(bool visible) {
    if (rangeIndicator == null) return;

    rangeIndicator.gameObject.SetActive(visible);
  }

  public void SetRangeIndicatorScale() {
    if (rangeIndicator != null) {
      rangeIndicator.localScale = new Vector3(Range, normalRangeHeight, Range);
    }
  }

  public override string ToString() {
    return Name + "\n"
        + "  Area of effect: " + AreaOfEffect + "\n"
        + "  Armor piercing: " + ArmorPierce + "\n"
        + "  Armor tear: " + ArmorTear + "\n"
        + "  Attack speed: " + AttackSpeed + "\n"
        + "  Base attack speed: " + BaseAttackSpeed + "\n"
        + "  Cost: " + Cost + "\n"
        + "  Damage: " + Damage + "\n"
        + "  Damage over time: " + DamageOverTime + "\n"
        + "  Effective attack speed: " + EffectiveAttackSpeed + "\n"
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
