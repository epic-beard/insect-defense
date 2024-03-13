using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class WavesCompleted : MonoBehaviour {
  readonly private string waveCompleteLabelName = "wave-completed-label";

  private UIDocument uiDocument;
  private Label waveCompleteLabelVE;

  private float fadeSpeed = 1.5f;
  private Color labelColor = Color.white;

  void Awake() {
    SetVisualElements();
    Spawner.WaveComplete += OnWaveComplete;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;
    waveCompleteLabelVE = rootElement.Q<Label>(waveCompleteLabelName);
  }
  private void OnWaveComplete(int currWave, int numWaves) {
    StartCoroutine(ShowWaveComplete());
  }

  private IEnumerator ShowWaveComplete() {
    float opacity = 0.0f;
    while (opacity < 1.0f) {
      opacity += Time.deltaTime * fadeSpeed;
      labelColor.a = opacity;
      waveCompleteLabelVE.style.color = labelColor;
      yield return null;
    }
    yield return new WaitForSeconds(1);
    yield return StartCoroutine(HideWaveComplete());
  }

  private IEnumerator HideWaveComplete() {
    float opacity = 1.0f;
    while (opacity > 0.0f) {
      opacity -= Time.deltaTime * fadeSpeed;
      labelColor.a = opacity;
      waveCompleteLabelVE.style.color = labelColor;
      yield return null;
    }
  }
}
