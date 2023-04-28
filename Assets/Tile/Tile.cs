using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

  private bool lingeringWebs = false;
  private Dictionary<Tower, LingeringSlow> lingeringSlows = new();
  private LineRenderer webLineRenderer;

  // This class is used because structs are value type and c# does not deal well with changing value
  // types in a dictionary.
  public class LingeringSlow {
    public float SlowPower { get; set; }
    public float SlowDuration { get; set; }
    // This counts how many hits the web has left.
    public int Hits { get; set; }

    public LingeringSlow(float slowPower, float slowDuration, int hits) {
      SlowPower = slowPower;
      SlowDuration = slowDuration;
      Hits = hits;
    }
  }

  private void Start() {
    webLineRenderer = GetComponentInChildren<LineRenderer>();
  }

  // Add a lingering web to this tile. It can support multiple different towers adding a lingering web
  // of differing slow strengths.
  public void AddLingeringWeb(Tower tower, float slowPower, float slowDuration, int hits) {
    if (lingeringSlows.ContainsKey(tower)) {
      // Since the same tower is laying the web, the slow power and duration could only improve.
      LingeringSlow slow = lingeringSlows[tower];
      slow.SlowPower = slowPower;
      slow.SlowDuration = slowDuration;
      // To avoid infinitely stacking lingering webs, the max charges of a lingering web is static.
      slow.Hits = hits;
    } else {
      lingeringSlows.Add(tower, new LingeringSlow(slowPower, slowDuration, hits));
    }
    lingeringWebs = true;
    if (!webLineRenderer.enabled) {
      webLineRenderer.enabled = true;
    }
  }

  private void OnTriggerEnter(Collider other) {
    if (lingeringWebs) {
      Enemy enemy = other.GetComponent<Enemy>();
      // Lingering webs don't apply to flying enemies.
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
        webLineRenderer.enabled = false;
      }
    }
  }
}