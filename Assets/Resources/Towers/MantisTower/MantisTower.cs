using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TowerAbility;

public class MantisTower : Tower {
  [SerializeField] public Transform bodyMesh;
  [SerializeField] public Transform lowerLeftArm;
  [SerializeField] public Transform lowerRightArm;

  public override TowerData.Type Type { get; set; } = TowerData.Type.MANTIS_TOWER;

  public float AoECrippleCooldownSpeed { get { return (AttackSpeed * 2f); } }
  public bool BloodyExecution { get; private set; } = false;
  public bool CrippleAttack { get; private set; } = false;
  public float CrippleCooldownSpeed { get { return (AttackSpeed * 1.5f); } }
  public bool FrozenTarget { get; private set; } = false;
  public bool RendingClaws { get; private set; } = false;
  public float RendingClawsDuration { get; private set; } = 3.0f;
  public bool SecondAttack { get; private set; } = false;
  public float SecondaryDamage {
    get { return Damage * secondaryDamageModifier; }
  }
  public bool Shrike { get; private set; } = false;

  public enum MantisAttackType {
    UPPER_RIGHT,
    UPPER_LEFT,
  }

  private Animator animator;
  private Dictionary<MantisAttackType, Transform> attackOriginMap = new();
  private float secondaryDamageModifier = 0.5f;

  // Transform.Find explicitly does not recursively search for a given name. Thus, the name given
  // must include the path to that object within the heirarchy of the Mantis tower.
  private readonly string upperRightName = "Mantis Mesh/UR Arm Holder/UR Shoulder";
  private readonly string upperLeftName = "Mantis Mesh/UL Arm Holder/UL Shoulder";

  protected override void TowerStart() {
    animator = GetComponent<Animator>();

    attackOriginMap = new() {
      { MantisAttackType.UPPER_RIGHT, this.transform.Find(upperRightName) },
      { MantisAttackType.UPPER_LEFT, this.transform.Find(upperLeftName) }
    };

    StartCoroutine(Attack());
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch (ability) {
      case SpecialAbility.M_1_3_RENDING_CLAWS:
        RendingClaws = true;
        break;
      case SpecialAbility.M_1_5_DOUBLE_SLASH:
        SecondAttack = true;
        break;
      case SpecialAbility.M_2_3_JAGGED_CLAWS:
        StartCoroutine(CrippleCooldown());
        CrippleAttack = true;
        break;
      case SpecialAbility.M_2_5_BLOODY_EXECUTION:
        BloodyExecution = true;
        break;
      case SpecialAbility.M_3_2_CAMO_SIGHT:
        CamoSight = true;
        break;
      case SpecialAbility.M_3_5_SHRIKE:
        secondaryDamageModifier = 1.0f;
        Shrike = true;
        break;
      default:
        break;
    }
  }

  protected override void TowerUpdate() {
    if (Target != null) {
      bodyMesh.LookAt(Target.AimPoint - (Vector3.up * 4));
    }
  }

  // This method is called by an AnimationEvent embedded within the Mantis' attack animations.
  public void ProcessDamageAndEffects(MantisAttackType attackType) {
    if (Target == null) return;
    if (Target.enabled) {
      if (BloodyExecution && Target.IsDoomedByBlood()) {
        Target.DealDamage(Target.HP, DamageText.DamageType.BLEED);
      } else {
        Target.DealPhysicalDamage(Damage, ArmorPierce, false);
        Target.AddBleedStacks(BleedStacks);
        if (CrippleAttack) {
          Target.ApplyCripple();
          CrippleAttack = false;
        }
        if (RendingClaws && Target.AcidStacks > 0.0f) {
          Target.TempReduceArmor(ArmorPierce, RendingClawsDuration);
        }
      }
    }

    Vector3 A = attackOriginMap[attackType].position;
    Vector3 B = Target.transform.position;

    A.y = 0;
    B.y = 0;

    // Ensure that the target enemy is not among those reviewed for secondary damage.
    List<Enemy> potentialVictims = Targeting.GetAllValidEnemiesInRange(
        enemies: ObjectPool.Instance.GetActiveEnemies().Where(e => !e.Equals(Target)).ToHashSet(),
        origin: transform.position,
        range: Range,
        camoSight: CamoSight,
        antiAir: AntiAir);

    foreach (Enemy pv in potentialVictims) {
      Vector3 C = pv.transform.position;
      C.y = 0;

      Vector3 vHat = (B - A).normalized;
      float x = Vector3.Dot(C - A, vHat);
      if (x <= 0) continue;
      Vector3 w = x * vHat;
      float dist = (C - w - A).magnitude;

      if (dist < this.AreaOfEffect) {
        pv.DealPhysicalDamage(SecondaryDamage, ArmorPierce, false);
      }
    }
  }

  protected override void UpdateAnimationSpeed(float newAttackSpeed) {
    if (animator != null) {
      animator.SetFloat("Speed", newAttackSpeed);
    }
  }

  // Launch the Mantis' attack animations.
  private void Stab() {
    animator.Play("Basic Attack Layer.UR Attack");
    if (SecondAttack) {
      animator.Play("Second Attack Layer.UL Attack");
    }
  }

  private IEnumerator Attack() {
    while (true) {
      yield return null;

      if (IsDazzled()) continue;

      Target = targeting.FindTarget(
      oldTarget: Target,
      enemies: ObjectPool.Instance.GetActiveEnemies(),
      towerPosition: transform.position,
      towerRange: Range,
      camoSight: CamoSight,
      antiAir: AntiAir);

      if (Target != null) {
        Stab();
        yield return new WaitForSeconds(1 / EffectiveAttackSpeed);
      }
    }
  }


  private IEnumerator CrippleCooldown() {
    while (true) {
      while (!CrippleAttack) {
        CrippleAttack = true;

        yield return new WaitForSeconds(1 / CrippleCooldownSpeed);
      }
      yield return new WaitUntil(() => !CrippleAttack);
    }
  }
}
