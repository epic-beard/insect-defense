using UnityEngine;
using UnityEngine.UIElements;

public class BottomBar : MonoBehaviour {
  readonly private string gameSpeedDialName = "game-speed-dial";
  readonly private string gameSpeedButton0Name = "game-speed-0-button";
  readonly private string gameSpeedButton1Name = "game-speed-1-button";
  readonly private string gameSpeedButton2Name = "game-speed-2-button";
  readonly private string gameSpeedButton3Name = "game-speed-3-button";

  readonly private string settingsButtonName = "settings-button";

  private UIDocument uiDocument;

  private VisualElement gameSpeedDialVE;
  private Button gameSpeedButton0VE;
  private Button gameSpeedButton1VE;
  private Button gameSpeedButton2VE;
  private Button gameSpeedButton3VE;

  private Button settingsButtonVE;

  private void Awake() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    gameSpeedDialVE = rootElement.Q<VisualElement>(className: gameSpeedDialName);
    gameSpeedButton0VE = rootElement.Q<Button>(gameSpeedButton0Name);
    gameSpeedButton1VE = rootElement.Q<Button>(gameSpeedButton1Name);
    gameSpeedButton2VE = rootElement.Q<Button>(gameSpeedButton2Name);
    gameSpeedButton3VE = rootElement.Q<Button>(gameSpeedButton3Name);
    settingsButtonVE = rootElement.Q<Button>(settingsButtonName);
  }

  private void Start() {
    RegisterCallbacks();
  }

  private void RegisterCallbacks() {
     gameSpeedButton0VE.RegisterCallback<ClickEvent>(
          (ClickEvent) => { SetGameSpeedFromDial(0); });
      gameSpeedButton1VE.RegisterCallback<ClickEvent>(
          (ClickEvent) => { SetGameSpeedFromDial(1); });
      gameSpeedButton2VE.RegisterCallback<ClickEvent>(
          (ClickEvent) => { SetGameSpeedFromDial(2); });
      gameSpeedButton3VE.RegisterCallback<ClickEvent>(
          (ClickEvent) => { SetGameSpeedFromDial(4); });
      settingsButtonVE.RegisterCallback<ClickEvent>(
          (ClickEvent) => { TerrariumInputManager.Instance.ToggleSettings(); });
  }

  private void SetGameSpeedFromDial(float speed) {
    if (speed == 0) {
        GameSpeedManager.Instance.HandlePause();
    } else {
        if (GameSpeedManager.Instance.IsPaused()) {
            GameSpeedManager.Instance.HandlePause();
        }
        GameSpeedManager.Instance.SetGameSpeed(speed);
    }

    gameSpeedDialVE.style.backgroundImage = Resources.Load<Texture2D>("UI/images/bgs/speed_dial_texture_" + speed.ToString());
  }
}
