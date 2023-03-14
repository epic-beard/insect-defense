#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

// A class to handle projectile management instead of unity's built-in handling. This allows fine-grained
// control of particle movement and destruction.
public class ProjectileHandler {
  readonly private ParticleSystem particleSystem;
  readonly private float particleSpeed;
  readonly private float hitRange;
  private Particle[] particles;

  // This dictionary is to track each individual particle and their target.
  private Dictionary<Int64, Enemy> particleIDsToEnemies = new();
   
  // This is a tracker for id generation for keys to the particleIdsToEnemies dictionary.
  private Int64 particleIdTracker = 100;

  public delegate void ProcessParticleCollision(Enemy target);

  public ProjectileHandler(ParticleSystem particleSystem, float particleSpeed, float hitRange) {
    this.particleSystem = particleSystem;
    this.particleSpeed = particleSpeed;
    this.hitRange = hitRange;

    particles = new Particle[particleSystem.main.maxParticles];
  }

  // Get a safe position for the shots of any tower. Ideally, the actual mesh, but if that isn't present,
  // the enemy container itself.
  public Vector3 GetSafeChildPosition(Transform transform) {
    if (transform.childCount == 0) {
      return transform.position;
    }
    return transform.GetChild(0).position;
  }

  // Make sure that a just-fired particle is associated with the given enemy.
  public void AssociateOrphanParticlesWithEnemy(Enemy enemy) {
    int numActiveParticles = particleSystem.GetParticles(particles);

    // Code intending to change particle position/behavior must use particles[i] rather than a helper variable.
    for (int i = 0; i < numActiveParticles; i++) {
      particles[i].velocity = Vector3.zero;

      // Add to the particle to enemy tracker if necessary. Tracking individual particles can be difficult because
      // ParticleSystem.GetParticles returns a value rather than a reference.
      if (particles[i].startLifetime < 100) {
        particles[i].startLifetime = particleIdTracker;
        particleIDsToEnemies.Add(particleIdTracker, enemy);
        particleIdTracker++;
      }
    }

    particleSystem.SetParticles(particles, numActiveParticles);
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
          // This should be an impossible situation, a particle that was just fired but doesn't have a target.
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
}