using UnityEngine;
using UnityEngine.UIElements;

public class TerrariumBottomUI : MonoBehaviour {
  readonly private string playPauseButtonName = "play_pause__button";
  readonly private string settingsButtonName = "settings__button";
  readonly private string nuLabelName = "nu_amount__label";

  readonly private string buttonWithTooltipTooltipVEName = "button-tooltip-test--TooltipVE";
  private ButtonWithTooltipVE buttonWithTooltip_VE;

  private UIDocument terrariumScreen;
  private Button playPauseButton;
  private Button settingsButton;
  private Label nuLabel;

  private void Awake() {
    terrariumScreen = GetComponent<UIDocument>();
    VisualElement rootElement = terrariumScreen.rootVisualElement;

    buttonWithTooltip_VE = rootElement.Q<ButtonWithTooltipVE>(buttonWithTooltipTooltipVEName);

    playPauseButton = rootElement.Q<Button>(playPauseButtonName);
    settingsButton = rootElement.Q<Button>(settingsButtonName);
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

    //buttonWithTooltip_VE.SetupClickHandling();
    buttonWithTooltip_VE.RegisterCallback<ClickEvent>(
        (ClickEvent) => { PauseManager.Instance.HandlePause(); });
  }

  public void KeepPlayPauseButtonNameCorrect(bool paused) {
    if (paused) {
      playPauseButton.text = "Play";
      buttonWithTooltip_VE.ButtonWithTooltip.text = "Play";
    } else {
      playPauseButton.text = "Pause";
      buttonWithTooltip_VE.ButtonWithTooltip.text = "Pause";
    }
  }

  public void UpdateNu(int nu) {
    nuLabel.text = nu.ToString();
  }
}
