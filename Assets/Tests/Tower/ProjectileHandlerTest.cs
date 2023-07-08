using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.ParticleSystem;

public class ProjectileHandlerTest {

  ProjectileHandler projectileHandler;
  ParticleSystem projectileSystem;
  Particle[] particles;

  [SetUp]
  public void SetUp() {
    projectileSystem = new GameObject().AddComponent<ParticleSystem>();

    projectileHandler = new(projectileSystem, 10.0f, 0.1f);
    particles = new Particle[projectileSystem.main.maxParticles];
    Time.captureDeltaTime = 1;
  }

  #region UpdateParticlesTests

  [UnityTest]
  public IEnumerator UpdateParticlesCorrectDirectionOfTravel() {
    EmitParticleAndSetTheLocationToOrigin(particles);

    Enemy enemy = new GameObject().AddComponent<Enemy>();
    enemy.transform.position = Vector3.right * 100;

    Vector3 directionOfTravel = projectileHandler.GetSafeChildPosition(enemy.transform) - particles[0].position;
    Vector3 normalizedDirectionOfTravel = directionOfTravel.normalized;

    projectileHandler.UpdateParticles(enemy, FakeProcessParticleCollision);

    int numActiveParticles = projectileSystem.GetParticles(particles);

    Assert.That(numActiveParticles, Is.EqualTo(1));
    Assert.That(particles[0].position.normalized, Is.EqualTo(normalizedDirectionOfTravel));

    return null;
  }

  // The setup for this test is creating the particle at the origin and creating the enemy one unit to
  // the right. With the projectile speed of 10, it will overshoot, unless our logic handles that.
  [UnityTest]
  public IEnumerator UpdateParticlesDestroysWhenParticlesHitNoOvershoot() {
    EmitParticleAndSetTheLocationToOrigin(particles);

    Enemy enemy = new GameObject().AddComponent<Enemy>();
    enemy.transform.position = Vector3.right;

    projectileHandler.UpdateParticles(enemy, FakeProcessParticleCollision);

    int numActiveParticles = projectileSystem.GetParticles(particles);

    Assert.That(numActiveParticles, Is.EqualTo(0));

    return null;
  }

  #endregion

  #region GetSafeChildPositionTests

  [Test]
  public void GetSafeChildPositionWithChild() {
    projectileHandler = new(projectileSystem, 1.0f, 1.0f);
    GameObject gameObject = new();

    Enemy enemyParent = gameObject.AddComponent<Enemy>();
    Enemy enemyChild = gameObject.AddComponent<Enemy>();
    enemyParent.transform.position = Vector3.up;
    enemyChild.transform.position = Vector3.down;
    enemyChild.transform.parent = enemyParent.transform;

    Assert.That(projectileHandler.GetSafeChildPosition(enemyParent.transform), Is.EqualTo(Vector3.down));
  }

  [Test]
  public void GetSafeChildPositionWithoutChild() {
    projectileHandler = new(projectileSystem, 1.0f, 1.0f);
    GameObject gameObject = new();

    Enemy enemyParent = gameObject.AddComponent<Enemy>();
    enemyParent.transform.position = Vector3.up;

    Assert.That(projectileHandler.GetSafeChildPosition(enemyParent.transform), Is.EqualTo(Vector3.up));
  }

  #endregion

  #region TestHelperMethods

  // Ensure the created particle has a start position of (0, 0, 0).
  private void EmitParticleAndSetTheLocationToOrigin(Particle[] particles) {
    projectileSystem.Emit(1);
    int numActiveParticles = projectileSystem.GetParticles(particles);
    particles[0].position = Vector3.zero;
    projectileSystem.SetParticles(particles, numActiveParticles);
  }

  private void FakeProcessParticleCollision(Enemy target) {
    // Do nothing.
  }

  #endregion
}
