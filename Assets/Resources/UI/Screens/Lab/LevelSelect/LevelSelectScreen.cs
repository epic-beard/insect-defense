using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace lab {
  public class LevelSelectScreen : MonoBehaviour {
    public static LevelSelectScreen Instance;
    private int level;
    readonly private string levelLabelName = "level-label";
    readonly private string backButtonName = "level-select-back-button";
    readonly private string startButtonName = "start-button";

    private UIDocument terrariumScreen;

    private Label levelLabelVE;
    private Button startButtonVE;
    private Button backButtonVE;

    void Awake() {
      Instance = this;

      SetVisualElements();
      startButtonVE.clicked += GoToLevel;
      backButtonVE.clicked += CloseScreen;
      terrariumScreen.rootVisualElement.style.display = DisplayStyle.None;
      Terrarium.TerrariumClicked += OpenScreen;
    }

    private void SetVisualElements() {
      terrariumScreen = GetComponent<UIDocument>();
      VisualElement rootElement = terrariumScreen.rootVisualElement;

      levelLabelVE = rootElement.Q<Label>(levelLabelName);
      startButtonVE = rootElement.Q<Button>(startButtonName);
      backButtonVE = rootElement.Q<Button>(backButtonName);
    }

    public void OpenScreen(Terrarium t) {
      level = t.level;
      LabCamera.Instance.MoveToTerrarium(t,
        () => {
          terrariumScreen.rootVisualElement.style.display = DisplayStyle.Flex;
          LabInputManager.Instance.EnableTerrariumActionMap();
        });
      levelLabelVE.text = t.level.ToString();
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