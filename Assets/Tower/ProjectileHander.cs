#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class ProjectileHandler {
  readonly private ParticleSystem particleSystem;
  readonly private float particleSpeed;
  readonly private float hitRange;
  private Particle[] particles;
  private Dictionary<Int64, Enemy> particleIDsToEnemies = new();
  private Int64 particleIdTracker = 100;

  public delegate void ProcessParticleCollision(Enemy target);

  public ProjectileHandler(ParticleSystem particleSystem, float particleSpeed, float hitRange) {
    this.particleSystem = particleSystem;
    this.particleSpeed = particleSpeed;
    this.hitRange = hitRange;

    particles = new Particle[particleSystem.main.maxParticles];
  }

  public void UpdateParticles(Enemy? target, ProcessParticleCollision collisionProcessor) {
    int numActiveParticles = particleSystem.GetParticles(particles);

    // Code intending to change particle position/behavior must use particles[i] rather than a helper variable.
    for (int i = 0; i < numActiveParticles; i++) {
      particles[i].velocity = Vector3.zero;

      // Add to the particle to enemy tracker if necessary. Tracking individual particles can be difficult because
      // ParticleSystem.GetParticles returns a value rather than a reference.
      if (particles[i].startLifetime < 100) {
        // Visual studio recommended '==' rather than 'is'. This could be a bug.
        if (target == null) {
          particles[i].remainingLifetime = 0.0f;
        } else {
          particles[i].startLifetime = particleIdTracker;
          particleIDsToEnemies.Add(particleIdTracker, target);
          particleIdTracker++;
        }
      }

      // Destroy any particles targeting an enemy that is no longer alive.
      Enemy enemy = particleIDsToEnemies[(int)particles[i].startLifetime];
      if (!enemy.enabled) {
        particles[i].remainingLifetime = 0.0f;
        continue;
      }
      Vector3 targetPosition = GetSafeChildPosition(enemy.transform);

      // Obtain the direction of travel
      Vector3 vec = targetPosition - particles[i].position;
      float dist = vec.magnitude;
      Vector3 deltaTravel = vec.normalized;

      // Make distance traveled frame rate independent and ensure we cannot 'overshoot' a target.
      deltaTravel *= Mathf.Min(Time.deltaTime * particleSpeed, dist);
      particles[i].position += deltaTravel;

      // Initiate particle 'collision'. Destroy the particle and call the tower's particle collision handler.
      if (Vector3.Distance(targetPosition, particles[i].position) < hitRange) {
        collisionProcessor(enemy);
        particles[i].remainingLifetime = 0;
      }
    }

    // Update all particle positions.
    particleSystem.SetParticles(particles, numActiveParticles);
  }

  public Vector3 GetSafeChildPosition(Transform transform) {
    if (transform.childCount == 0) {
      return transform.position;
    }
    return transform.GetChild(0).position;
  }
}
