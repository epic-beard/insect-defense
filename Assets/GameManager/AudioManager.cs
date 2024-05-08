using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

  public static AudioManager Instance;

  [SerializeReference] private AudioMixerGroup musicGroup;
  [SerializeReference] private AudioMixerGroup sfxGroup;
  [NonReorderable][SerializeField] private List<Sound> sounds = new();
  readonly private List<AudioSource> sources = new();

  private void Awake() {
    Instance = this;
    foreach (var sound in sounds) {
      sources.Add(GetAudioSource(sound));
    }

    LoadSfxForUi();
  }

  private void Start() {
    OnVolumeChanged();
  }

  public void Play(string name) {
    AudioSource source = sources.Find(source => source.name == name);
    if (source == null) {
      Debug.LogError("Source not found: " + name);
      return;
    }
    source.Play();
  }

  public void Stop(string name) {
    AudioSource source = sources.Find(source => source.name == name);
    if (source == null) {
      Debug.LogError("Source not found: " + name);
      return;
    }
    source.Stop();
  }

  private AudioSource GetAudioSource(Sound sound) {
    AudioSource source = new GameObject().AddComponent<AudioSource>();
    source.name = sound.Name;
    source.clip = sound.Clip;
    source.loop = sound.Loop;
    source.volume = sound.Volume;

    switch (sound.Type) {
      case Sound.AudioType.SFX:
        source.outputAudioMixerGroup = sfxGroup;
        break;
      case Sound.AudioType.MUSIC:
        source.outputAudioMixerGroup = musicGroup;
        break;
    }

    if (sound.PlayOnAwake) source.Play();

    return source;
  }

  public void OnVolumeChanged() {
    Settings settings = PlayerState.Instance.Settings;
    UpdateVolume("Master Volume", settings.MasterVolume);
    UpdateVolume("Music Volume", settings.MusicVolume);
    UpdateVolume("Sfx Volume", settings.SfxVolume);
  }

  private void UpdateVolume(string volumeSetting, float volume) {
    AudioMixer mixer = musicGroup.audioMixer;
    if (volume > 0) {
      mixer.SetFloat(volumeSetting, Mathf.Log10(volume) * 20);
    } else {
      mixer.SetFloat(volumeSetting, -80);
    }
  }

    private void LoadSfxForUi() {
        sources.Add(CreateSfxAudioSource(UiSfx.tower_select_list_collapse));
        sources.Add(CreateSfxAudioSource(UiSfx.context_panel_collapse));
        sources.Add(CreateSfxAudioSource(UiSfx.rocker_switch));
        sources.Add(CreateSfxAudioSource(UiSfx.rocker_switch_fail));
        sources.Add(CreateSfxAudioSource(UiSfx.dropdown_active));
        sources.Add(CreateSfxAudioSource(UiSfx.dropdown_event_changed));
        sources.Add(CreateSfxAudioSource(UiSfx.settings_open));
        sources.Add(CreateSfxAudioSource(UiSfx.settings_tab_active));
        sources.Add(CreateSfxAudioSource(UiSfx.speed_dial_click));
        sources.Add(CreateSfxAudioSource(UiSfx.game_start));
        sources.Add(CreateSfxAudioSource(UiSfx.died));
    }

    private AudioSource CreateSfxAudioSource(string filename) {
        AudioSource source = new GameObject().AddComponent<AudioSource>();

        source.name = filename;
        source.clip = Resources.Load<AudioClip>("UI/sounds/" + filename);
        source.loop = false;
        source.playOnAwake = false;
        source.volume = 1f;
        source.outputAudioMixerGroup = sfxGroup;

        return source;
    }
}
