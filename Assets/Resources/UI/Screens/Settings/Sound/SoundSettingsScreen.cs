using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SoundSettingsScreen : MonoBehaviour {
  VisualElement rootElement;

  readonly private string sfxName = "sfx";

  readonly private string soundSettingsMenuName = "sound-settings-menu-doc";

  readonly private string masterVolumeSliderName = "master-volume-slider";
  readonly private string masterVolumeLabelName = "master-volume-label";

  readonly private string sfxVolumeSliderName = "sfx-volume-slider";
  readonly private string sfxVolumeLabelName = "sfx-volume-label";

  readonly private string musicVolumeSliderName = "music-volume-slider";
  readonly private string musicVolumeLabelName = "music-volume-label";

  readonly private string playSfxButtonName = "play-sfx-button";

  private Slider masterVolumeSliderVE;
  private Label masterVolumeLabelVE;

  private Slider musicVolumeSliderVE;
  private Label musicVolumeLabelVE;

  private Slider sfxVolumeSliderVE;
  private Label sfxVolumeLabelVE;

  private Button playSfxButtonVE;

  private void OnEnable() {
    SetVisualElements();
    RegisterCallbacks();

    playSfxButtonVE.clicked += PlaySfx;
  }

  private void Start() {
    SetLabels(PlayerState.Instance.Settings);
    VolumeChanged();
  }

  private void OnDisable() {
    playSfxButtonVE.clicked -= PlaySfx;
  }

  private void SetVisualElements() {
    UIDocument settingsMenu = GetComponent<UIDocument>();
    rootElement = settingsMenu.rootVisualElement.Q(soundSettingsMenuName);

    masterVolumeSliderVE = rootElement.Q<Slider>(masterVolumeSliderName);
    masterVolumeLabelVE = rootElement.Q<Label>(masterVolumeLabelName);

    musicVolumeSliderVE = rootElement.Q<Slider>(musicVolumeSliderName);
    musicVolumeLabelVE = rootElement.Q<Label>(musicVolumeLabelName);
    
    sfxVolumeSliderVE = rootElement.Q<Slider>(sfxVolumeSliderName);
    sfxVolumeLabelVE = rootElement.Q<Label>(sfxVolumeLabelName);

    playSfxButtonVE = rootElement.Q<Button>(playSfxButtonName);
  }

  private void RegisterCallbacks() {
    masterVolumeSliderVE.RegisterValueChangedCallback(OnVolumeChanged);
    sfxVolumeSliderVE.RegisterValueChangedCallback(OnVolumeChanged);
    musicVolumeSliderVE.RegisterValueChangedCallback(OnVolumeChanged);
  }

  public void OnVolumeChanged(ChangeEvent<float> evt) {
    VolumeChanged();
  }

  public void VolumeChanged() {
    Settings settings = PlayerState.Instance.Settings;
    settings.MasterVolume = masterVolumeSliderVE.value;
    settings.SfxVolume = sfxVolumeSliderVE.value;
    settings.MusicVolume = musicVolumeSliderVE.value;

    SaveManager.Instance.Save(PlayerState.Instance);

    SetLabels(settings);

    AudioManager.Instance.OnVolumeChanged();
  }

  public void PlaySfx() {
    AudioManager.Instance.Play(sfxName);
  }

  private void SetLabels(Settings settings) {
    masterVolumeLabelVE.text = VolumeToDisplayText(settings.MasterVolume);
    sfxVolumeLabelVE.text = VolumeToDisplayText(settings.SfxVolume);
    musicVolumeLabelVE.text = VolumeToDisplayText(settings.MusicVolume);
  }

  private string VolumeToDisplayText(float volume) {
    return Mathf.RoundToInt(volume*100).ToString();
  }
}
