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
  public int AcidStacks {
    get { return data.acid_stacks; }
    set { data.acid_stacks = value; }
  }
  public float AttackSpeed {
    get { return data.attack_speed; }
    set { data.attack_speed = value; }
  }
  public float EffectiveAttackSpeed {
    get { return AttackSpeed * SlimePower; }
  }
  public float AreaOfEffect {
    get { return data.area_of_effect; }
    set { data.area_of_effect = value; }
  }
  public float ArmorPierce {
    get { return data.armor_pierce; }
    set { data.armor_pierce = value; }
  }
  public float BaseAttackSpeed { get; private set; }
  public int BleedStacks {
    get { return data.bleed_stacks; }
    set { data.bleed_stacks = value; }
  }
  public int Cost {
    get { return data.cost; }
    set { data.cost = value; }
  }
  public float Damage {
    get { return data.damage; }
    set { data.damage = value; }
  }
  public float DazzleTime { get; set; }
  public bool IsMutatingUpgrades { get; set; }
  public string Name {
    get { return data.name; }
    set { data.name = value; }
  }
  public float Range {
    get { return data.range; }
    set { data.range = value; }
  }
  public HashSet<Renderer> Renderers { get; private set; }
  public float ProjectileSpeed {
    get { return data.projectile_speed; }
    set { data.projectile_speed = value; }
  }
  public float SecondarySlowPotency {
    get { return data.secondary_slow_potency; }
    set { data.secondary_slow_potency = value; }
  }
  public int SecondarySlowTargets {
    get { return data.secondary_slow_targets; }
    set { data.secondary_slow_targets = value; }
  }
  public float SlimeTime { get; set; }
  public float SlimePower { get; set; } = 1.0f;
  public float SlowDuration {
    get { return data.slow_duration; }
    set { data.slow_duration = value; }
  }
  public float SlowPower {
    get { return data.slow_power; }
    set { data.slow_power = value; }
  }
  public float StunTime {
    get { return data.stun_time; }
    set { data.stun_time = value; }
  }
  public float VenomPower {
    get { return data.venom_power; }
    set { data.venom_power = value; }
  }
  public int VenomStacks {
    get { return data.venom_stacks; }
    set { data.venom_stacks = value; }
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

  // The current state of this tower's upgrade paths.
  protected int[] upgradeIndex = new int[] { -1, -1, -1 };  // Each entry in this array should be [-1 - 4].
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
    if (ability.floatAttributeModifiers != null) {
      AttributeUpgrade(ability.floatAttributeModifiers);
    }
    if (ability.intAttributeModifiers != null) {
      AttributeUpgrade(ability.intAttributeModifiers);
    }

    upgradeIndex[ability.upgradePath]++;
  }

  private void AttributeUpgrade(TowerAbility.AttributeModifier<int>[] mods) {
    foreach (TowerAbility.AttributeModifier<int> modifier in mods) {
      switch (modifier.mode) {
      case TowerAbility.Mode.MULTIPLICATIVE:
        data.SetIntStat(modifier.attribute, data.GetIntStat(modifier.attribute) * modifier.mod);
        break;
      case TowerAbility.Mode.ADDITIVE:
        data.SetIntStat(modifier.attribute, data.GetIntStat(modifier.attribute) + modifier.mod);
        break;
      case TowerAbility.Mode.SET:
        data.SetIntStat(modifier.attribute, modifier.mod);
        break;
      }
      NonStatisticUpdates(modifier.attribute);
    }
  }

  private void AttributeUpgrade(TowerAbility.AttributeModifier<float>[] mods) {
    foreach (TowerAbility.AttributeModifier<float> modifier in mods) {
      switch (modifier.mode) {
      case TowerAbility.Mode.MULTIPLICATIVE:
        data.SetFloatStat(modifier.attribute, data.GetFloatStat(modifier.attribute) * modifier.mod);
        break;
      case TowerAbility.Mode.ADDITIVE:
        data.SetFloatStat(modifier.attribute, data.GetFloatStat(modifier.attribute) + modifier.mod);
        break;
      case TowerAbility.Mode.SET:
        data.SetFloatStat(modifier.attribute, modifier.mod);
        break;
      }
      NonStatisticUpdates(modifier.attribute);
    }
  }

  // Changes agnostic of float or int status of attribute modifier.
  private void NonStatisticUpdates(TowerData.Stat attribute) {
    if (attribute == TowerData.Stat.ATTACK_SPEED) {
      UpdateAnimationSpeed(GetAnimationSpeedMultiplier());
    }
    if (attribute == TowerData.Stat.RANGE) {
      rangeIndicator.localScale = new Vector3(Range, normalRangeHeight, Range);
    }
  }

  // Returns true if the specified upgrade is legal, which is defined as only being able to buy
  // upgrades 2 and 3 with 2 upgrade paths, and upgrades 4 and 5 with 1 upgrade path.
  public bool IsLegalUpgrade(int upgradePath, int purchaseLevel) {
    // The first upgrade is always legal.
    if (purchaseLevel == 0) { return true; }

    // Set up a data structure containing the upgrade number of the other upgrade paths.
    int[] otherTrees = new int[] { -1, -1 };
    for (int i = 0; i < UpgradeIndex.Length; i++) {
      if (i == upgradePath) { continue; }
      if (otherTrees[0] == -1) {
        otherTrees[0] = UpgradeIndex[i];
      } else {
        otherTrees[1] = UpgradeIndex[i];
      }
    }

    if ((purchaseLevel == 1 || purchaseLevel == 2)
        && (otherTrees[0] < 1 || otherTrees[1] < 1)) {
      return true;
    }

    if ((purchaseLevel == 3 || purchaseLevel == 4)
        && otherTrees[0] < 3 && otherTrees[1] < 3) {
      return true;
    }

    return false;
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
    BaseAttackSpeed = data.attack_speed;
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
    if (targetingIndicator != null
        && Targeting.IsTargetValidAndInRange(enemy, this.transform.position, Range, CamoSight, AntiAir)) {
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
        + "  Attack speed: " + AttackSpeed + "\n"
        + "  Base attack speed: " + BaseAttackSpeed + "\n"
        + "  Cost: " + Cost + "\n"
        + "  Damage: " + Damage + "\n"
        + "  Damage over time: " + AcidStacks + "\n"
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
