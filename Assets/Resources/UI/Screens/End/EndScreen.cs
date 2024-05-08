using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour {
  readonly private string endScreenLabelName = "end-screen-label";
  readonly private string restartButtonName = "restart-button";
  readonly private string labButtonName = "lab-button";

  readonly private string fanfare = "fanfare";
  
  UIDocument endScreen;

  Label endScreenLabel;
  Button restartButtonVE;
  Button labButtonVE;

  private void OnEnable() {
    SetVisualElements();
    RegisterCallbacks();

    GameStateManager.GameOver += OnGameOver;
    Spawner.LevelComplete += OnLevelComplete;

    endScreen.rootVisualElement.style.display = DisplayStyle.None;
  }

  private void OnDisable() {
    GameStateManager.GameOver -= OnGameOver;
    Spawner.LevelComplete -= OnLevelComplete;
  }

  private void SetVisualElements() {
    endScreen = GetComponent<UIDocument>();
    VisualElement rootElement = endScreen.rootVisualElement;

    endScreenLabel = rootElement.Q<Label>(endScreenLabelName);
    restartButtonVE = rootElement.Q<Button>(restartButtonName);
    labButtonVE = rootElement.Q<Button>(labButtonName);
  }

  private void RegisterCallbacks() {
    restartButtonVE.RegisterCallback<ClickEvent>(Restart);
    labButtonVE.RegisterCallback<ClickEvent>(GoToLab);
  }

  private void Restart(ClickEvent evt) {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  private void GoToLab(ClickEvent evt) {
    SceneManager.LoadScene(Constants.labSceneName);
  }

  private void OnGameOver() {
    UiSfx.PlaySfx(UiSfx.died);
    GameSpeedManager.Instance.HandlePause(PauseToken.END);
    TerrariumScreen.Instance.HideUI();
    endScreenLabel.text = "You Died";
    labButtonVE.text = "Give Up";
    endScreen.rootVisualElement.style.display = DisplayStyle.Flex;
  }

  // TODO(nnewsom): Conduct necessary checks, metadata tracking, and save the player state.
  private void OnLevelComplete() {
    AudioManager.Instance.Play(fanfare);
    GameSpeedManager.Instance.HandlePause(PauseToken.END);
    TerrariumScreen.Instance.HideUI();
    endScreenLabel.text = "You Won!";
    labButtonVE.text = "Return";
    endScreen.rootVisualElement.style.display = DisplayStyle.Flex;
  }
}
