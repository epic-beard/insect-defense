using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour {
  readonly private string endScreenLabelName = "end-screen__label";
  readonly private string restartButtonName = "restart__button";
  readonly private string labButtonName = "lab__button";
  
  readonly private string labScene = "Lab";
  UIDocument endScreen;

  Label endScreenLabel;
  Button restartButton;
  Button labButton;

  private void OnEnable() {
    SetVisualElements();
    RegisterCallbacks();

    GameStateManager.GameOver += OnGameOver;
    Spawner.OnLevelComplete += OnLevelComplete;

    endScreen.rootVisualElement.style.display = DisplayStyle.None;
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
    labButton.RegisterCallback<ClickEvent>(GoToLab);
  }

  private void Restart(ClickEvent evt) {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //[TODO nnewsom] is this necessary
    //PauseManager.Instance.Unpause();
  }

  private void GoToLab(ClickEvent evt) {
    SceneManager.LoadScene(labScene);
  }

  private void OnGameOver() {
    PauseManager.Instance.HandlePause(PauseToken.END);
    TerrariumUI.Instance.HideUI();
    endScreenLabel.text = "You Died";
    labButton.text = "Give Up";
    endScreen.rootVisualElement.style.display = DisplayStyle.Flex;
  }

  // TODO(nnewsom): Conduct necessary checks, metadata tracking, and save the player state.
  private void OnLevelComplete() {
    PauseManager.Instance.HandlePause(PauseToken.END);
    TerrariumUI.Instance.HideUI();
    endScreenLabel.text = "You Won!";
    labButton.text = "Return";
    endScreen.rootVisualElement.style.display = DisplayStyle.Flex;
  }
}
