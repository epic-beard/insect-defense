using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour {
  readonly private string endScreenLabelName = "end-screen__label";
  readonly private string restartButtonName = "restart__button";
  readonly private string labButtonName = "lab__button";

  UIDocument endScreen;

  Label endScreenLabel;
  Button restartButton;
  Button labButton;

  private void OnEnable() {
    SetVisualElements();
    RegisterCallbacks();

    GameStateManager.GameOver += OnGameOver;
    Spawner.OnLevelComplete += OnLevelComplete;
  }

  private void OnDisable() {
    GameStateManager.GameOver -= OnGameOver;
    Spawner.OnLevelComplete -= OnLevelComplete;
  }

  private void SetVisualElements() {
    endScreen = GetComponent<UIDocument>();
    VisualElement rootElement = endScreen.rootVisualElement;

    endScreenLabel = rootElement.Q<Label>(endScreenLabelName);
    restartButton = rootElement.Q<Button>(restartButtonName);
    labButton = rootElement.Q<Button>(labButtonName);
  }

  private void RegisterCallbacks() {
    restartButton.RegisterCallback<ClickEvent>(Restart);
  }

  private void Restart(ClickEvent evt) {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    PauseManager.Instance.UnpauseAndUnlock();
  }

  private void OnGameOver() {
    PauseManager.Instance.PauseAndLock();
    endScreenLabel.text = "You Died";
    labButton.text = "Give Up";
    endScreen.enabled = true;
  }

  private void OnLevelComplete() {
    PauseManager.Instance.PauseAndLock();
    endScreenLabel.text = "You Won!";
    labButton.text = "Return";
    endScreen.enabled = true;
  }
}
