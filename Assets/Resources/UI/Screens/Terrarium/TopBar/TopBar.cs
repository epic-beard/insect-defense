using UnityEngine;
using UnityEngine.UIElements;

public class TopBar : MonoBehaviour {
  public static TopBar Instance;

  readonly private string hpLabelName = "hp__label";
  readonly private string waveLabelName = "wave__label";

  private UIDocument terrariumScreen;
  private Label hpLabel;
  private Label waveLabel;

  private void Awake() {
    Instance = this;

    SetVisualElements();
    GameStateManager.HealthChanged += OnHealthChanged;
    Spawner.WaveComplete += OnWaveComplete;
    Spawner.WavesStarted += OnWavesStarted;
  }

  private void SetVisualElements() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;
    hpLabel = rootElement.Q<Label>(hpLabelName);
    waveLabel = rootElement.Q<Label>(waveLabelName);
  }

  private void OnHealthChanged(int hp) {
    hpLabel.text = hp.ToString();
  }

  private void OnWavesStarted(int numWaves) {
    UpdateWaveLabel(1, numWaves);
  }

  private void OnWaveComplete(int currWave, int numWaves) {
    UpdateWaveLabel(currWave, numWaves);
  }

  public void UpdateWaveLabel(int currWave, int numWaves) {
    waveLabel.text = "Wave: " + currWave + "/" + numWaves;
  }
}
