using Assets;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

  [SerializeField] bool isTowerPlaceable;

  private bool isTowerPresent = false;
  private bool lingeringWebs = false;
  private Dictionary<Tower, LingeringSlow> lingeringSlows = new();
  private LineRenderer webLineRenderer;
  private Waypoint waypoint;
  private Color basic;
  private Material mat;

  // This class is used because structs are value type and c# does not deal well with changing value
  // types in a dictionary.
  public class LingeringSlow {
    public float SlowPower { get; set; }
    public float SlowDuration { get; set; }
    // This counts how many hits the web has left.
    public int WebHits { get; set; }

    public LingeringSlow(float slowPower, float slowDuration, int hits) {
      SlowPower = slowPower;
      SlowDuration = slowDuration;
      WebHits = hits;
    }
  }

  private void Start() {
    webLineRenderer = GetComponentInChildren<LineRenderer>();
    waypoint = GetComponent<Waypoint>();
    mat = this.transform.GetChild(1).GetComponent<Renderer>().material;
    basic = mat.color;
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
      slow.WebHits = hits;
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
        lingeringSlow.WebHits--;
        if (lingeringSlow.WebHits == 0) {
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

  // Handle tower placement and selection. Will automatically select a tower that has just been built.
  private void OnMouseUp() {
    if (GameStateManager.Instance.IsMouseOverUI) return;
    if (!isTowerPlaceable) { return; }
    if (!isTowerPresent) {
      isTowerPresent = TowerManager.Instance.BuildTower(waypoint);
      if (!isTowerPresent) { return; }
    }
    // This happens while a tower is in the prcocess of being sold.
    if (isTowerPresent && !TowerManager.Instance.HasTower(GetCoordinates())) { return; }

    Utilities.SetSelectedTower(TowerManager.Instance.GetTower(GetCoordinates()));
  }

  private void BuildTowerIfPossible() {
    if (!isTowerPlaceable) { return; }
    if (!isTowerPresent) {
      isTowerPresent = TowerManager.Instance.BuildTower(waypoint);
      if (!isTowerPresent) { return; }
    }
    // This happens while a tower is in the prcocess of being sold.
    if (isTowerPresent && !TowerManager.Instance.HasTower(GetCoordinates())) { return; }

    Utilities.SetSelectedTower(TowerManager.Instance.GetTower(GetCoordinates()));
  }

  public void ResetTile() {
    isTowerPresent = false;
  }

  public void SetUnselected() {
    mat.SetColor("_Color", basic);
  }

  public void SetSelected() {
    mat.SetColor("_Color", Color.yellow);
  }

  public Vector2Int GetCoordinates() {
    return waypoint.GetCoordinates();
  }
}
