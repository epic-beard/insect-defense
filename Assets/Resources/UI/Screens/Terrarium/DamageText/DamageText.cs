using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public delegate T TweeningFunction<T>(float x);

public class DamageText : MonoBehaviour {
  public List<Color> colors = new();
  public float bounceTime = 2.0f;
  public float bounceHeight = 1.0f;
  public float bounceWidth = 1.0f;
  public float fallback = 1.0f;
  public enum DamageType {
    PHYSICAL,
    BLEED,
    ACID,
    POISON,
  }

  private TextMeshPro tmpro;
  private Dictionary<DamageType, Color> damageColors = new();

  private void Awake() {
    for (int i = 0; i < colors.Count; i++) {
      damageColors.Add((DamageType)i, colors[i]);
    }

    tmpro = GetComponent<TextMeshPro>();
    FaceCamera();
  }

  private void Update() {
    FaceCamera();
  }

  public void DisplayDamage(int damage, DamageType type) {
    tmpro.text = "-" + damage.ToString();
    tmpro.color = damageColors[type];

    StartCoroutine(BounceUp(transform.position));
  }

  private void FaceCamera() {
    if (Camera.main != null) {
      Vector3 newForward = Camera.main.transform.forward;
      newForward.y = 0;
      transform.forward = newForward;
    }
  }

  private IEnumerator BounceUp(Vector3 start) {
    Vector3 end = start + new Vector3(bounceWidth/2, bounceHeight, fallback/2);
    float bounceUpTime = bounceTime / 2;
    float t = 0;

    while (t < 1) {
      t += Time.deltaTime / bounceUpTime;
      transform.position = Tween(start, end, EaseOutCubicYLinearXZTweening, t);
      yield return null;
    }

    transform.position = end;
    StartCoroutine(BounceDown(transform.position));
  }

  private IEnumerator  BounceDown(Vector3 start) {
    Vector3 end = start + new Vector3(bounceWidth / 2, -bounceHeight, fallback / 2);
    float bounceDownTime = bounceTime / 2;
    float t = 0;

    while (t < 1) {
      t += Time.deltaTime / bounceDownTime;
      transform.position = Tween(start, end, EaseInCubicYLinearXZTweening, t);
      yield return null;
    }

    transform.position = end;
    Destroy(transform.gameObject);
  }

  private Vector3 EaseOutCubicYLinearXZTweening(float x) {
    float y = 1.0f - Mathf.Pow(1 - x, 3);
    return new Vector3(x, y , x);
  }

  private Vector3 EaseInCubicYLinearXZTweening(float x) {
    float y = x * x * x;
    return new Vector3(x, y, x);
  }

  private Vector3 Tween(Vector3 start, Vector3 end, TweeningFunction<Vector3> tweening, float t) {
    return start + Vector3.Scale(end - start, tweening(t));
  }
}
