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
  public bool ApexAttack { get; private set; } = false;
  public bool BloodyExecution { get; private set; } = false;
  public bool CrippleAttack { get; private set; } = false;
  public float CrippleCooldownSpeed { get { return (AttackSpeed * 1.5f); } }
  public bool FrozenTarget { get; private set; } = false;
  public bool SecondAttack { get; private set; } = false;
  public float SecondaryDamage {
    get { return Damage * secondaryDamageModifier; }
  }
  public bool Shrike { get; private set; } = false;

  public enum MantisAttackType {
    UPPER_RIGHT,
    UPPER_LEFT,
    LOWER_RIGHT,
    LOWER_LEFT,
  }

  private Animator animator;
  private bool firing = false;
  private Dictionary<MantisAttackType, Transform> attackOriginMap = new();
  private float secondaryDamageModifier = 0.5f;

  // Transform.Find explicitly does not recursively search for a given name. Thus, the name given
  // must include the path to that object within the heirarchy of the Mantis tower.
  private readonly string upperRightName = "Mantis Mesh/UR Arm Holder/UR Shoulder";
  private readonly string upperLeftName = "Mantis Mesh/UL Arm Holder/UL Shoulder";
  private readonly string lowerRightName = "Mantis Mesh/LR Arm Holder/LR Shoulder";
  private readonly string lowerLeftName = "Mantis Mesh/LL Arm Holder/LL Shoulder";

  protected override void TowerStart() {
    animator = GetComponent<Animator>();

    attackOriginMap = new() {
      { MantisAttackType.UPPER_RIGHT, this.transform.Find(upperRightName) },
      { MantisAttackType.UPPER_LEFT, this.transform.Find(upperLeftName) },
      { MantisAttackType.LOWER_RIGHT, this.transform.Find(lowerRightName) },
      { MantisAttackType.LOWER_LEFT, this.transform.Find(lowerLeftName) }
    };

    MakeLowerArmsVisible(false);
    StartCoroutine(Attack());
  }

  public override void SpecialAbilityUpgrade(TowerAbility.SpecialAbility ability) {
    switch (ability) {
      case SpecialAbility.M_1_3_DOUBLE_SLASH:
        SecondAttack = true;
        break;
      case SpecialAbility.M_1_5_FOUR_ARMS:
        MakeLowerArmsVisible(true);
        ApexAttack = true;
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
      }
    }

    Vector3 A = attackOriginMap[attackType].position;
    Vector3 B = Target.transform.position;

    A.y = 0;
    B.y = 0;

    // Ensure that the target enemy is not among those reviewed for secondary damage.
    List<Enemy> potentialVictims = Targeting.GetAllValidEnemiesInRange(
        enemies: ObjectPool.Instance.GetActiveEnemies().Where(e => !e.Equals(Target)).ToHashSet(),
        towerPosition: transform.position,
        towerRange: Range,
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

  // These methods are intended to be called through AnimationEvents.
  public void FreezeTarget() {
    FrozenTarget = true;
  }

  public void UnFreezeTarget() {
    FrozenTarget = false;
  }

  protected override void TowerUpdate() {
    if (!FrozenTarget) {

      Target = targeting.FindTarget(
      oldTarget: Target,
      enemies: ObjectPool.Instance.GetActiveEnemies(),
      towerPosition: transform.position,
      towerRange: Range,
      camoSight: CamoSight,
      antiAir: AntiAir);

      if (Target == null) {
        firing = false;
      } else {
        bodyMesh.LookAt(Target.AimPoint - (Vector3.up * 4));
        firing = true;
      }
    } else if (Target != null) {
      bodyMesh.LookAt(Target.AimPoint - (Vector3.up * 4));
    } else if (Target == null) {
      firing = false;
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
    if (ApexAttack) {
      animator.Play("Third Attack Layer.LR Attack");
      animator.Play("Fourth Attack Layer.LL Attack");
    }
  }

  private void MakeLowerArmsVisible(bool visible) {
    if (lowerLeftArm != null && lowerRightArm != null) {
      if (visible) {
        lowerLeftArm.localScale = new Vector3(1, 1, 1);
        lowerRightArm.localScale = new Vector3(1, 1, 1);
      } else {
        lowerLeftArm.localScale = new Vector3(0, 0, 0);
        lowerRightArm.localScale = new Vector3(0, 0, 0);
      }
    }
  }

  private IEnumerator Attack() {
    while (true) {
      while (firing && !IsDazzled()) {
        Stab();
        FreezeTarget();

        yield return new WaitForSeconds(1 / EffectiveAttackSpeed);
      }
      yield return new WaitUntil(() => firing);
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
