using Codice.CM.SEIDInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour {
  public List<GameObject> DamageTexts = new();
  // This must be in the order of the Damage Types.
  public List<Color> colors = new();
  private Dictionary<DamageType,float> durations = new();
  private Dictionary<DamageType, int> damages = new();
  private readonly float DamageTextBoxDelay = 3.0f;
  public enum DamageType {
    PHYSICAL,
    BLEED,
    ACID,
    POISON,
  }

  // Start is called before the first frame update
  void Start() {
    foreach (DamageType type in (DamageType[])Enum.GetValues(typeof(DamageType))) {
      durations[type] = 0.0f;
      damages[type] = 0;
    }
  }

  // Update is called once per frame
  void Update() {
    foreach (var obj in DamageTexts) {
      obj.transform.LookAt(-Camera.main.transform.position);
    }
  }

  //void ClearText() {
  //  foreach (var obj in DamageTexts) { obj.GetComponent<TextMeshPro>}
  //}

  void ArangeText() {
    List<DamageType> presentTypes = durations
      .Where((kvp) => kvp.Value > 0.0f)
      .Select(kvp => kvp.Key)
      .ToList();
    presentTypes.Sort();
  }

  void AddDamage(DamageType type, int damage) {
    durations[type] = DamageTextBoxDelay;
    damages[type] = damage;
  }
}
