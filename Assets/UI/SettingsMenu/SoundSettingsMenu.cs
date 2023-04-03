using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SoundSettingsMenu : MonoBehaviour {
  UIDocument soundSettingsMenu;

  readonly private string masterVolumeSliderName = "master-volume-slider";
  readonly private string sfxVolumeSliderName = "sfx-volume-slider";
  readonly private string musicVolumeSliderName = "music-volume-slider";

  private Slider masterVolumeSlider;
  private Slider musicVolumeSlider;
  private Slider sfxVolumeSlider;

  private void OnEnable() {
    SetVisualElements();
    RegisterCallbacks();
  }

  private void SetVisualElements() {
    soundSettingsMenu = GetComponent<UIDocument>();
    VisualElement rootElement = soundSettingsMenu.rootVisualElement;

    masterVolumeSlider = rootElement.Q<Slider>(masterVolumeSliderName);
    sfxVolumeSlider = rootElement.Q<Slider>(sfxVolumeSliderName);
    musicVolumeSlider = rootElement.Q<Slider>(musicVolumeSliderName);
  }

  private void RegisterCallbacks() {
    AudioManager audioManager = AudioManager.Instance;
    masterVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
    sfxVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
    musicVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);

  }

  public void OnVolumeChanged(ChangeEvent<float> evt) {
    AudioManager.Instance.OnVolumeChanged();
  }
}
