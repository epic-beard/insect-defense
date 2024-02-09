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

  [Range(0, 1)]
  public float CameraSensitivity = 0.3f;

  [Range(0, 1)]
  public float ZoomSensitivity = 0.3f;

  [Range(0, 1)]
  public float RotationSensitivity = 0.3f;
}
