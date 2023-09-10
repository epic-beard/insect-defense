using System;

[Serializable]
public class PlayerState {
  public static PlayerState Instance;

  public PlayerState(string id) {
    Id = id;
  }

  public PlayerState() {
    Id = Guid.NewGuid().ToString();
  }

  public string Id;
  public string SaveName;
  public int CurrentLevel = 0;
  public Settings Settings = new();
  // The last time this playerstate was saved in milliseconds.
  public long lastSavedTime;
}
