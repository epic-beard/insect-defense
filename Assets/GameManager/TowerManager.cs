using System.Collections.Generic;
using UnityEngine;
using static EpicBeardLib.XmlSerializationHelpers;

using TowerDictionary = EpicBeardLib.Containers.SerializableDictionary<TowerData.Type, TowerData>;

public class TowerManager : MonoBehaviour {
  public static TowerManager Instance;

  [SerializeField] private string towerDataFilename = "data.towers";
  private TowerDictionary towers = new();
  private readonly Dictionary<TowerData.Type, string> prefabMap = new() {
    { TowerData.Type.SPITTING_ANT_TOWER, "Towers/SpittingAntTower/Spitting Ant Tower" },
    { TowerData.Type.MANTIS_TOWER, "Towers/MantisTower/Mantis Tower" },
    { TowerData.Type.ASSASSIN_BUG_TOWER, "Towers/AssassinBugTower/Assassin Bug Tower" },
    { TowerData.Type.WEB_SHOOTING_SPIDER_TOWER, "Towers/WebShootingSpiderTower/Web Shooting Spider Tower" },
  };

  private void Awake() {
    Instance = this;
    towers = Deserialize<TowerDictionary>(towerDataFilename);
  }

  public TowerData GetTowerData(TowerData.Type type) {
    return towers[type];
  }

  public Tower ConstructTower(Waypoint waypoint, TowerData.Type type) {
    return ConstructTower(waypoint, GetTowerData(type));
  }

  public Tower ConstructTower(Waypoint waypoint, TowerData data) {
    string towerDataPath = TowerManager.Instance.GetTowerPrefabPath(data.type);
    GameObject prefab = Resources.Load<GameObject>(towerDataPath);
    GameObject towerObj = Instantiate(
        prefab,
        waypoint.transform.position,
        Quaternion.identity);
    Tower tower = towerObj.GetComponent<Tower>();
    tower.SetTowerData(data);
    tower.Tile = waypoint.GetComponent<Tile>();
    tower.enabled = false;

    GameStateManager.Instance.ActiveTowerMap.Add(waypoint.GetCoordinates(), tower);
    return tower;
  }

  public string GetTowerPrefabPath(TowerData.Type type) {  return prefabMap[type]; }
}
