using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Terrarium : MonoBehaviour {

  [SerializeField] private int level;

  // Start is called before the first frame update
  void Start() {
    if (PlayerState.Instance.CurrentLevel < level) {
      var mat = GetComponent<Renderer>().material;
      mat.SetColor("Color", Color.gray);
      enabled = false;
    }
  }

  private void OnMouseDown() {
    string levelName = "Level" + level;
    SceneManager.LoadScene(levelName);
  }
}
