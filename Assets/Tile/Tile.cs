using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour {

  private bool lingeringWebs = false;
  private Dictionary<Tower, LingeringSlow> lingeringSlows;

  // This private class is used because structs are value type and c# does not deal well with changing value
  // types in a dictionary.
  private class LingeringSlow {
    public float SlowPower { get; set; }
    public float SlowDuration { get; set; }
    public int Hits { get; set; }

    public LingeringSlow(float slowPower, float slowDuration, int hits) {
      SlowPower = slowPower;
      SlowDuration = slowDuration;
      Hits = hits;
    }
  }

  // Add a lingering web to this tile. It can support multiple different towers adding a lingering web
  // of differing slow strengths.
  public void AddLingeringWeb(Tower tower, float slowPower, float slowDuration, int hits) {
    if (lingeringSlows.ContainsKey(tower)) {
      LingeringSlow slow = lingeringSlows[tower];
      slow.SlowPower = slowPower;
      slow.SlowDuration = slowDuration;
      slow.Hits += hits;
    } else {
      lingeringSlows.Add(tower, new LingeringSlow(slowPower, slowDuration, hits));
    }
    lingeringWebs = true;
  }

  private void OnTriggerEnter(Collider other) {
    Debug.Log("Trigger triggered!");

    if (lingeringWebs) {
      Enemy enemy = other.GetComponent<Enemy>();
      if (enemy.Flying) {
        return;
      }

      List<Tower> entriesToRemove = new();
      foreach (var (tower, lingeringSlow) in lingeringSlows) {
        enemy.ApplySlow(lingeringSlow.SlowPower, lingeringSlow.SlowDuration);
        lingeringSlow.Hits--;
        if (lingeringSlow.Hits == 0) {
          entriesToRemove.Add(tower);
        }
      }

      foreach (var entry in entriesToRemove) {
        lingeringSlows.Remove(entry);
      }

      if (lingeringSlows.Count == 0) {
        lingeringWebs = false;
      }
    }
  }
}
