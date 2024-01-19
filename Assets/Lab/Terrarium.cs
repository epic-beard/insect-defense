#nullable enable
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Terrarium : MonoBehaviour {
  public static Terrarium? Selected;
  [SerializeField] private float cameraOffsetZ;
  [SerializeField] private int level;

  readonly private string levelLabelName = "level__label";
  readonly private string startButtonName = "start__button";
  readonly private string backButtonName = "back__button";

  private UIDocument terrariumScreen;

  private Label levelLabel;
  private Button startButton;
  private Button backButton;

  void OnEnable() {
    if (PlayerState.Instance.CurrentLevel < level) {
      var mat = GetComponent<Renderer>().material;
      mat.SetColor("_Color", Color.gray);
    }

    SetVisualElements();

    startButton.clicked += GoToLevel;
    backButton.clicked += CloseScreen;
    levelLabel.text = level.ToString();
    terrariumScreen.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void SetVisualElements() {
    terrariumScreen = GetComponentInChildren<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    levelLabel = rootElement.Q<Label>(levelLabelName);
    startButton = rootElement.Q<Button>(startButtonName);
    backButton = rootElement.Q<Button>(backButtonName);
  }

  private void OnMouseUp() {
    if (PlayerState.Instance.CurrentLevel >= level) {
      Selected = this;
      LabCamera.Instance.MoveTo(transform.position + Vector3.forward * cameraOffsetZ);
      terrariumScreen.rootVisualElement.style.display = DisplayStyle.Flex;
      LabInputManager.Instance.EnableTerrariumActionMap();
    }
  }

  public void CloseScreen() {
    Selected = null;
    LabCamera.Instance.ReturnCamera();
    terrariumScreen.rootVisualElement.style.display = DisplayStyle.None;
    LabInputManager.Instance.DisableTerrariumActionMap();
  }

  private void GoToLevel() {
    if (PlayerState.Instance.CurrentLevel >= level) {
      string levelName = "Level" + level;
      SceneManager.LoadScene(levelName);
    }
  }
}
