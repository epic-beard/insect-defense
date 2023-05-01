using System;
using UnityEngine;

[Serializable]
public class Settings {
  [Range(0, 1)]
  public float MasterVolume;
  [Range(0, 1)]
  public float MusicVolume;
  [Range(0, 1)]
  public float SfxVolume;
}
