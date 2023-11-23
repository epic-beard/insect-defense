using UnityEngine;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;

public class TowerDataManager : MonoBehaviour {
  public static TowerDataManager Instance;

  [SerializeField] private string towerDataFilename = "data.towers";
  private TowerDictionary towers = new();

  private void Awake() {
    Instance = this;
  }

  void Start() {
    towers = Deserialize<TowerDictionary>(towerDataFilename);
  }

  public TowerData GetTowerData(TowerData.Type type) {
    return towers[type];
  }
}
