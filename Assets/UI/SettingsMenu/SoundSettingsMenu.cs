using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SoundSettingsMenu : MonoBehaviour {
  VisualElement rootElement;

  readonly private string sfxName = "sfx";

  readonly private string soundSettingsMenuName = "sound-settings-menu--doc";

  readonly private string masterVolumeSliderName = "master-volume--slider";
  readonly private string masterVolumeLabelName = "master-volume--label";

  readonly private string sfxVolumeSliderName = "sfx-volume--slider";
  readonly private string sfxVolumeLabelName = "sfx-volume--label";

  readonly private string musicVolumeSliderName = "music-volume--slider";
  readonly private string musicVolumeLabelName = "music-volume--label";

  readonly private string playSfxButtonName = "play-sfx--button";

  private Slider masterVolumeSlider;
  private Label masterVolumeLabel;

  private Slider musicVolumeSlider;
  private Label musicVolumeLabel;

  private Slider sfxVolumeSlider;
  private Label sfxVolumeLabel;

  private Button playSfxButton;

  private void OnEnable() {
    SetVisualElements();
    RegisterCallbacks();

    if (PlayerState.Instance == null) Debug.Log("Help");
    SetLabels(PlayerState.Instance.Settings);

    playSfxButton.clicked += PlaySfx;
  }

  private void OnDisable() {
    playSfxButton.clicked -= PlaySfx;
  }

  private void SetVisualElements() {
    UIDocument settingsMenu = GetComponent<UIDocument>();
    rootElement = settingsMenu.rootVisualElement.Q(soundSettingsMenuName);

    masterVolumeSlider = rootElement.Q<Slider>(masterVolumeSliderName);
    masterVolumeLabel = rootElement.Q<Label>(masterVolumeLabelName);

    sfxVolumeSlider = rootElement.Q<Slider>(sfxVolumeSliderName);
    sfxVolumeLabel = rootElement.Q<Label>(sfxVolumeLabelName);
    
    musicVolumeSlider = rootElement.Q<Slider>(musicVolumeSliderName);
    musicVolumeLabel = rootElement.Q<Label>(musicVolumeLabelName);

    playSfxButton = rootElement.Q<Button>(playSfxButtonName);
  }

  private void RegisterCallbacks() {
    masterVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
    sfxVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
    musicVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
  }

  public void OnVolumeChanged(ChangeEvent<float> evt) {
    Settings settings = PlayerState.Instance.Settings;
    settings.MasterVolume = masterVolumeSlider.value;
    settings.SfxVolume = sfxVolumeSlider.value;
    settings.MusicVolume = musicVolumeSlider.value;

    SaveManager.Instance.Save(PlayerState.Instance);

    SetLabels(settings);

    AudioManager.Instance.OnVolumeChanged();
  }

  public void PlaySfx() {
    AudioManager.Instance.Play(sfxName);
  }

  private void SetLabels(Settings settings) {
    masterVolumeLabel.text = VolumeToDisplayText(settings.MasterVolume);
    sfxVolumeLabel.text = VolumeToDisplayText(settings.SfxVolume);
    musicVolumeLabel.text = VolumeToDisplayText(settings.MusicVolume);
  }

  private string VolumeToDisplayText(float volume) {
    return Mathf.RoundToInt(volume*100).ToString();
  }
}
