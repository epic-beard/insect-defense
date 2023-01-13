using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
  protected Dictionary<TowerData.Stat, float> attributes;
  protected List<TowerData.TowerAbility> towerAbilities;
  int[] upgradeLevels;

  public void Upgrade(Ability ability) {
    if (ability.Mode == TowerData.Mode.SPECIAL) {
      SpecialAbilityUpgrade(ability.SpecialAbility);
    } else {
      foreach (Ability.AttributeModifier mod in ability.AttributeModifiers) {
        attributes[mod.attribute] = attributes[mod.attribute] * mod.mult;
      }
    }

    upgradeLevels[ability.UpgradePath]++;
  }

  public virtual void SpecialAbilityUpgrade(Ability.SpecialAbilityEnum ability) {}
}
