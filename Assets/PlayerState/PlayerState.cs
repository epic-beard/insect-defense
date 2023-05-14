using System;

[Serializable]
public class PlayerState {
  public static PlayerState Instance;

  public string SaveName;
  public int HighestLevelBeat = 0;
  public Settings Settings = new();
}
