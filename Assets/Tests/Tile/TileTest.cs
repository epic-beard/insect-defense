using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class TileTest {

  Tile tile;
  LineRenderer lineRenderer;
  GameObject enemyGameObject;

  [SetUp]
  public void Setup() {
    GameObject gameObject = new();
    tile = gameObject.AddComponent<Tile>();
    lineRenderer = gameObject.AddComponent<LineRenderer>();
    lineRenderer.enabled = false;
    tile.SetWebLineRenderer(lineRenderer);
  }

  // Test that adding a new web will have the desired effect.
  [Test]
  public void AddLingeringWebNewAddition() {
    Dictionary<Tower, Tile.LingeringSlow> lingeringWebs = tile.GetLingeringSlows();
    Assert.That(lingeringWebs.Count, Is.EqualTo(0));
    Assert.That(lineRenderer.enabled, Is.False);

    WebShootingSpiderTower wssTower = new GameObject().AddComponent<WebShootingSpiderTower>();
    float slowPower = 0.5f;
    float slowDuration = 2.0f;
    int hits = 3;
    tile.AddLingeringWeb(wssTower, slowPower, slowDuration, hits);

    lingeringWebs = tile.GetLingeringSlows();
    Assert.That(lingeringWebs.Count, Is.EqualTo(1));
    Assert.That(lineRenderer.enabled, Is.True);
  }

  // Make sure that adding an existing web doesn't do anything crazy.
  [Test]
  public void AddLingeringWebAddToExistingWeb() {
    WebShootingSpiderTower wssTower = new GameObject().AddComponent<WebShootingSpiderTower>();
    float slowPower = 0.5f;
    float slowDuration = 2.0f;
    int hits = 3;
    tile.AddLingeringWeb(wssTower, slowPower, slowDuration, hits);

    Dictionary<Tower, Tile.LingeringSlow> lingeringWebs = tile.GetLingeringSlows();
    Assert.That(lingeringWebs.Count, Is.EqualTo(1));
    Assert.That(lineRenderer.enabled, Is.True);

    tile.AddLingeringWeb(wssTower, slowPower, slowDuration, hits);

    lingeringWebs = tile.GetLingeringSlows();
    Assert.That(lingeringWebs.Count, Is.EqualTo(1));
    Assert.That(lineRenderer.enabled, Is.True);
  }

  [Test]
  public void OnTriggerEnterBasic() {
    WebShootingSpiderTower wssTower = new GameObject().AddComponent<WebShootingSpiderTower>();
    float slowPower = 0.5f;
    float slowDuration = 2.0f;
    int hits = 3;
    tile.AddLingeringWeb(wssTower, slowPower, slowDuration, hits);

    Dictionary<Tower, Tile.LingeringSlow> lingeringWebs = tile.GetLingeringSlows();
    Collider collider = CreateColliderWithEnemy(Vector3.zero);
    Enemy enemy = collider.GetComponent<Enemy>();

    // Establish the basis for comparison.
    Assert.That(lingeringWebs.Count, Is.EqualTo(1));
    Assert.That(lineRenderer.enabled, Is.True);
    Assert.That(lingeringWebs[wssTower].WebHits, Is.EqualTo(hits));
    Assert.That(enemy.SlowDuration, Is.EqualTo(0.0f));

    tile.InvokeOnTriggerEnter(collider);

    // Ensure that all the appropriate changes have been made.
    lingeringWebs = tile.GetLingeringSlows();
    Assert.That(lingeringWebs.Count, Is.EqualTo(1));
    Assert.That(lineRenderer.enabled, Is.True);
    Assert.That(lingeringWebs[wssTower].WebHits, Is.EqualTo(hits - 1));
    Assert.That(enemy.SlowDuration, Is.EqualTo(slowDuration));
  }

  [Test]
  public void OnTriggerEnterFlying() {
    WebShootingSpiderTower wssTower = new GameObject().AddComponent<WebShootingSpiderTower>();
    float slowPower = 0.5f;
    float slowDuration = 2.0f;
    int hits = 3;
    tile.AddLingeringWeb(wssTower, slowPower, slowDuration, hits);

    Dictionary<Tower, Tile.LingeringSlow> lingeringWebs = tile.GetLingeringSlows();
    Collider collider = CreateColliderWithEnemy(Vector3.zero, properties: EnemyData.Properties.FLYING);
    Enemy enemy = collider.GetComponent<Enemy>();

    // Establish the basis for comparison.
    Assert.That(lingeringWebs.Count, Is.EqualTo(1));
    Assert.That(lineRenderer.enabled, Is.True);
    Assert.That(lingeringWebs[wssTower].WebHits, Is.EqualTo(hits));
    Assert.That(enemy.SlowDuration, Is.EqualTo(0.0f));

    tile.InvokeOnTriggerEnter(collider);

    // Ensure the enemy's speed hasn't changed and the web is still intact.
    lingeringWebs = tile.GetLingeringSlows();
    Assert.That(lingeringWebs.Count, Is.EqualTo(1));
    Assert.That(lineRenderer.enabled, Is.True);
    Assert.That(lingeringWebs[wssTower].WebHits, Is.EqualTo(hits));
    Assert.That(enemy.SlowDuration, Is.EqualTo(0.0f));
  }

  #region TestHelperMethods

  private Collider CreateColliderWithEnemy(
      Vector3 position,
      EnemyData.Properties properties = EnemyData.Properties.NONE) {
    GameObject gameObject = new();
    gameObject.transform.position = position;

    EnemyData data = new() {
      properties = properties,
      size = EnemyData.Size.NORMAL,
    };

    Enemy enemy = gameObject.AddComponent<Enemy>();
    enemy.data = data;
    Collider collider = gameObject.AddComponent<BoxCollider>();

    return collider;
  }

  #endregion

}

#region TileUtils

public static class TileUtils {
  public static void SetWebLineRenderer(this Tile tile, LineRenderer lineRenderer) {
    typeof(Tile)
      .GetField("webLineRenderer", BindingFlags.Instance | BindingFlags.NonPublic)
      .SetValue(tile, lineRenderer);
  }

  public static Dictionary<Tower, Tile.LingeringSlow> GetLingeringSlows(this Tile tile) {
    return (Dictionary<Tower, Tile.LingeringSlow>) typeof(Tile)
      .GetField("lingeringSlows", BindingFlags.Instance | BindingFlags.NonPublic)
      .GetValue(tile);
  }

  public static void InvokeOnTriggerEnter(this Tile tile, Collider collider) {
    object[] args = { collider };
    Type[] argTypes = { typeof(Collider) };
    MethodInfo onTriggerEnter = typeof(Tile).GetMethod(
        "OnTriggerEnter",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null, CallingConventions.Standard, argTypes, null);
    onTriggerEnter.Invoke(tile, args);
  }
}

#endregion
