using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumGameView : MonoBehaviour {
  readonly private string waveCompleteLabelName = "wave_complete__label";

  private UIDocument terrariumGameView;
  private Label waveCompleteLabel;

  private float fadeSpeed = 1.5f;
  private Color labelColor = Color.white;

  void Awake() {
    SetVisualElements();
    Spawner.WaveComplete += OnWaveComplete;
  }

  private void SetVisualElements() {
    terrariumGameView = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumGameView.rootVisualElement;
    waveCompleteLabel = rootElement.Q<Label>(waveCompleteLabelName);
  }
  private void OnWaveComplete(int currWave, int numWaves) {
    StartCoroutine(ShowWaveComplete());
  }

  private IEnumerator ShowWaveComplete() {
    Debug.Log("Coming into focus.");
    float opacity = 0.0f;
    while (opacity < 1.0f) {
      opacity += Time.deltaTime * fadeSpeed;
      labelColor.a = opacity;
      waveCompleteLabel.style.color = labelColor;
      yield return null;
    }
    yield return new WaitForSeconds(1);
    yield return StartCoroutine(HideWaveComplete());
  }

  private IEnumerator HideWaveComplete() {
    Debug.Log("Fading away.");
    float opacity = 1.0f;
    while (opacity > 0.0f) {
      opacity -= Time.deltaTime * fadeSpeed;
      labelColor.a = opacity;
      waveCompleteLabel.style.color = labelColor;
      yield return null;
    }
  }
}
