using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverScreen : MonoBehaviour {
  readonly private string restartButtonName = "restart__button";

  UIDocument gameOverScreen;

  Button restartButton;

  private void OnEnable() {
    SetVisualElements();
    RegisterCallbacks();

    GameStateManager.GameOver += OnGameOver;
  }

  private void OnDisable() {
    GameStateManager.GameOver -= OnGameOver;
  }

  private void SetVisualElements() {
    gameOverScreen = GetComponent<UIDocument>();
    VisualElement rootElement = gameOverScreen.rootVisualElement;

    restartButton = rootElement.Q<Button>(restartButtonName);
  }

  private void RegisterCallbacks() {
    restartButton.RegisterCallback<ClickEvent>(Restart);
  }

  private void Restart(ClickEvent evt) {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    Time.timeScale = 1;
  }

  private void OnGameOver() {
    Time.timeScale = 0;
    gameOverScreen.enabled = true;
  }
}
