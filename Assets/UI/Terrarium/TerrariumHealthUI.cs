using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumHealthUI : MonoBehaviour {
  public static TerrariumHealthUI Instance;

  readonly private string hpLabelName = "hp__label";

  private UIDocument terrariumScreen;
  private Label hpLabel;

  private void Awake() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;
    hpLabel = rootElement.Q<Label>(hpLabelName);
    Instance = this;

    GameStateManager.HealthChanged += SetHpLabelText;
  }

  public void SetHpLabelText(int hp) {
    hpLabel.text = hp.ToString();
  }
}
