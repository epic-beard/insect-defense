using System;
using UnityEngine;

[Serializable]
public class Sound {
  public enum AudioType { SFX, MUSIC };

  public string Name;
  public AudioType Type;
  public AudioClip Clip;
  public bool Loop;
  public bool PlayOnAwake;

  [Range(0,1)]
  public float Volume;
}
