#nullable enable
using Ionic;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static EpicBeardLib.XmlSerializationHelpers;

public class WaveDesigner : MonoBehaviour {
  public static WaveDesigner Instance;

  [SerializeField] private string file;
  [SerializeField] private StyleSheet style;

  private Spawner.Waves waves = new();
  private TreeView treeView;
  
  void Awake() {
    Instance = this;
  }

  private void Start() {
    LoadWave(file);
    SetVisualElements();
    RegisterCallbacks();
  }

  private void SetVisualElements() {
    UIDocument uiDocument = GetComponent<UIDocument>();
    VisualElement rootElement = uiDocument.rootVisualElement;
    treeView = rootElement.Q<TreeView>();
    treeView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
    treeView.SetRootItems(GetRoots());
    treeView.makeItem = () => new VisualElement ();
    treeView.bindItem = (VisualElement ve, int index) => {
      treeView.GetItemDataForIndex<Spawner.Wave>(index).BindData(ve);
    };
    treeView.styleSheets.Add(style);
    treeView.reorderable = true;
    treeView.selectionType = SelectionType.Single;

    rootElement.Add(new NullableField<int>());
  }

  private void RegisterCallbacks() {
    // If we have problems with the current system for rejecting a drag and drop we may need to implement this.
    //treeView.RegisterCallback<DragUpdatedEvent>(evt => {
    //  // Check if we are over an invalid target.
    //  DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
    //});
    treeView.itemIndexChanged += (itemIdBeingMoved, newParentId) => {
      Spawner.Wave wave = treeView.GetItemDataForId<Spawner.Wave>(itemIdBeingMoved);
      Spawner.Wave parent = treeView.GetItemDataForId<Spawner.Wave>(newParentId);
      treeView.viewController.RebuildTree();
      int index = treeView.viewController.GetChildIndexForId(itemIdBeingMoved);
      if (!parent.AddWave(index, wave)) {
        // reordering didnt work so rebuild the tree.
        treeView.SetRootItems(GetRoots());
      }
    };

    Spawner.Wave.WaveChanged += () => SaveWave(file);
  }

  private void LoadWave(string filename) {
    waves = Deserialize<Spawner.Waves>(filename) ?? new Spawner.Waves();
  }

  private void SaveWave(string filename) {
    Serialize<Spawner.Waves>(waves, filename);
  }

  private IList<TreeViewItemData<Spawner.Wave>> GetRoots() {
    var roots = new List<TreeViewItemData<Spawner.Wave>>(waves.NumWaves);
    var subwaves = waves.GetChildren();
    int id = 0;
    foreach ( var subwave in subwaves ) {
      roots.Add(GetTreeData(subwave, ref id));
    }

    return roots;
  }

  private TreeViewItemData<Spawner.Wave> GetTreeData(Spawner.Wave wave, ref int id) {
    var subwaves = wave.GetChildren();
    if (subwaves.Count == 0) return new TreeViewItemData<Spawner.Wave>(id++, wave);
    var children = new List<TreeViewItemData<Spawner.Wave>>(subwaves.Count);
    foreach (var subwave in subwaves) {
      children.Add(GetTreeData(subwave, ref id));
    }
    return new TreeViewItemData<Spawner.Wave>(id++, wave, children);
  }
}
