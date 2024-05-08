using UnityEngine;
using UnityEngine.UIElements;

public class BottomBar : MonoBehaviour {
  readonly private string gameSpeedDialPointerName = "game-speed-dial-pointer";
  readonly private string gameSpeedButton0Name = "game-speed-0-button";
  readonly private string gameSpeedButton1Name = "game-speed-1-button";
  readonly private string gameSpeedButton2Name = "game-speed-2-button";
  readonly private string gameSpeedButton3Name = "game-speed-3-button";

  readonly private string settingsButtonName = "settings-button";

  private string pointerRotationClass = "game-speed-dial-pointer-0";

  private UIDocument uiDocument;

  private VisualElement gameSpeedDialPointerVE;
  private Button gameSpeedButton0VE;
  private Button gameSpeedButton1VE;
  private Button gameSpeedButton2VE;
  private Button gameSpeedButton3VE;

  private Button settingsButtonVE;

  private void Awake() {
    uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;

    gameSpeedDialPointerVE = rootElement.Q<VisualElement>(className: gameSpeedDialPointerName);
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
          (ClickEvent) => {
              UiSfx.PlaySfx(UiSfx.settings_open);
              TerrariumInputManager.Instance.ToggleSettings();
          });
  }

  private void SetGameSpeedFromDial(float speed) {
    UiSfx.PlaySfx(UiSfx.speed_dial_click);

    if (speed == 0) {
        GameSpeedManager.Instance.HandlePause();
    } else {
        if (GameSpeedManager.Instance.IsPaused()) {
            GameSpeedManager.Instance.HandlePause();
        }
        GameSpeedManager.Instance.SetGameSpeed(speed);
    }

    gameSpeedDialPointerVE.RemoveFromClassList(pointerRotationClass);
    pointerRotationClass = "game-speed-dial-pointer-" + speed.ToString();
    gameSpeedDialPointerVE.AddToClassList(pointerRotationClass);
  }
}
