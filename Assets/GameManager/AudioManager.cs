using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

  public static AudioManager Instance;

  [SerializeReference] private AudioMixerGroup musicGroup;
  [SerializeReference] private AudioMixerGroup sfxGroup;
  [SerializeReference] private List<Sound> sounds;
  readonly private List<AudioSource> sources = new();

  private void Awake() {
    Instance = this;
    foreach (var sound in sounds) {
      sources.Add(GetAudioSource(sound));
    }
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
    AudioSource source = gameObject.AddComponent<AudioSource>();
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
    AudioMixer mixer = musicGroup.audioMixer;
    Settings settings = PlayerState.Instance.Settings;
    mixer.SetFloat("Music Volume", Mathf.Log10(settings.MusicVolume) * 20);
    mixer.SetFloat("Sfx Volume", Mathf.Log10(settings.SfxVolume) * 20);
    mixer.SetFloat("Master Volume", Mathf.Log10(settings.MasterVolume) * 20);
  }
}
