using UnityEngine;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;
using AbilityDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerAbility[][]>;

public class TowerDataManager {
  public static TowerDataManager Instance;

  [SerializeField] private string towerDataFilename;
  [SerializeField] private string towerAbilitiesFilename;
  private TowerDictionary towers = new();
  private AbilityDictionary abilities = new();

  private void Awake() {
    Instance = this;
  }

  void Start() {
    towers = Deserialize<TowerDictionary>(towerDataFilename);
    abilities = Deserialize<AbilityDictionary>(towerAbilitiesFilename);
  }

  public TowerData GetTowerData(TowerData.Type type) {
    return towers[type];
  }

  public TowerAbility[][] GetTowerAbility(TowerData.Type type) {
    return abilities[type];
  }
}
