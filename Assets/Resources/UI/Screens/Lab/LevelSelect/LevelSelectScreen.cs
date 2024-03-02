using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace lab {
  public class LevelSelectScreen : MonoBehaviour {
    public static LevelSelectScreen Instance;
    private int level;
    readonly private string levelLabelName = "level__label";
    readonly private string backButtonName = "back__button";
    readonly private string startButtonName = "start__button";

    private UIDocument terrariumScreen;

    private Label levelLabel;
    private Button startButton;
    private Button backButton;

    void Awake() {
      Instance = this;

      SetVisualElements();
      startButton.clicked += GoToLevel;
      backButton.clicked += CloseScreen;
      terrariumScreen.rootVisualElement.style.display = DisplayStyle.None;
      Terrarium.TerrariumClicked += OpenScreen;
    }

    private void SetVisualElements() {
      terrariumScreen = GetComponent<UIDocument>();
      VisualElement rootElement = terrariumScreen.rootVisualElement;

      levelLabel = rootElement.Q<Label>(levelLabelName);
      startButton = rootElement.Q<Button>(startButtonName);
      backButton = rootElement.Q<Button>(backButtonName);
    }

    public void OpenScreen(Terrarium t) {
      level = t.level;
      LabCamera.Instance.MoveToTerrarium(t,
        () => {
          terrariumScreen.rootVisualElement.style.display = DisplayStyle.Flex;
          LabInputManager.Instance.EnableTerrariumActionMap();
        });
      levelLabel.text = t.level.ToString();
    }

    public void CloseScreen() {
      LabCamera.Instance.ReturnCamera();
      terrariumScreen.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void GoToLevel() {
      if (PlayerState.Instance.CurrentLevel >= level) {
        string levelName = "Level" + (level + 1);
        SceneManager.LoadScene(levelName);
      }
    }
  }
}