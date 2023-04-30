using System;

[Serializable]
public class PlayerState {
  public static PlayerState Instance;

  public int HighestLevelBeat = 0;
  public Settings Settings = new();
}
