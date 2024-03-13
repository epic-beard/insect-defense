using UnityEngine;
using UnityEngine.UIElements;

public class TopBar : MonoBehaviour {
  public static TopBar Instance;

  readonly private string hpLabelName = "hp-label";
  readonly private string waveLabelName = "wave-label";

  private UIDocument uiDocument;
  private Label hpLabelVE;
  private Label waveLabelVE;

  private void Awake() {
    Instance = this;

    SetVisualElements();
    GameStateManager.HealthChanged += OnHealthChanged;
    Spawner.WaveComplete += OnWaveComplete;
    Spawner.WavesStarted += OnWavesStarted;
  }

  private void SetVisualElements() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;
    hpLabelVE = rootElement.Q<Label>(hpLabelName);
    waveLabelVE = rootElement.Q<Label>(waveLabelName);
  }

  private void OnHealthChanged(int hp) {
    hpLabelVE.text = hp.ToString();
  }

  private void OnWavesStarted(int numWaves) {
    UpdateWaveLabel(1, numWaves);
  }

  private void OnWaveComplete(int currWave, int numWaves) {
    UpdateWaveLabel(currWave, numWaves);
  }

  public void UpdateWaveLabel(int currWave, int numWaves) {
    waveLabelVE.text = "Wave: " + currWave + "/" + numWaves;
  }
}
