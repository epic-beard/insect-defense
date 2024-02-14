using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumBottomUI : MonoBehaviour {
  readonly private string nuLabelName = "nu_amount__label";
  readonly private string playPauseButtonWithTooltipName = "play_pause__button_with_tooltip";
  readonly private string settingsButtonWithTooltipName = "settings__button_with_tooltip";

  readonly private string playString = "Play";
  readonly private string playTooltip = "Once more unto the breach, dear friends!";
  readonly private string pauseString = "Pause";
  readonly private string pauseTooltip = "Take a moment to think";
  readonly private string settingsString = "Settings";
  readonly private string settingsTooltip = "Open the settings menu";

  private UIDocument terrariumScreen;
  private ButtonWithTooltipVE playPauseButton;
  private ButtonWithTooltipVE settingsButton;
  private Label nuLabel;

  private void Awake() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    playPauseButton = rootElement.Q<ButtonWithTooltipVE>(playPauseButtonWithTooltipName);
    settingsButton = rootElement.Q<ButtonWithTooltipVE>(settingsButtonWithTooltipName);
    settingsButton.ButtonWithTooltip.text = settingsString;
    settingsButton.TooltipTextString = settingsTooltip;
    nuLabel = rootElement.Q<Label>(nuLabelName);
    GameStateManager.OnNuChanged += UpdateNu;
    PauseManager.OnPauseChanged += KeepPlayPauseButtonNameCorrect;
  }

  private void Start() {
    RegisterCallbacks();
  }

  private void RegisterCallbacks() {
    playPauseButton.RegisterCallback<ClickEvent>(
        (ClickEvent) => { PauseManager.Instance.HandlePause(); });
    settingsButton.RegisterCallback<ClickEvent>(
        (ClickEvent) => { TerrariumInputManager.Instance.ToggleSettings(); });
  }

  public void KeepPlayPauseButtonNameCorrect(bool paused) {
    if (paused) {
      playPauseButton.SetButtonText(playString);
      playPauseButton.TooltipTextString = playTooltip;
    } else {
      playPauseButton.SetButtonText(pauseString);
      playPauseButton.TooltipTextString = pauseTooltip;
    }
  }

  public void UpdateNu(int nu) {
    nuLabel.text = nu.ToString();
  }
}
