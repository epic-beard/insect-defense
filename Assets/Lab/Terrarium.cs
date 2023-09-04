using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Terrarium : MonoBehaviour {

  [SerializeField] private int level;

  void OnEnable() {
    Debug.Log(PlayerState.Instance.CurrentLevel);
    if (PlayerState.Instance.CurrentLevel < level) {
      var mat = GetComponent<Renderer>().material;
      mat.SetColor("_Color", Color.gray);
    }
  }

  private void OnMouseDown() {
    if (PlayerState.Instance.CurrentLevel >= level) {
      string levelName = "Level" + level;
      SceneManager.LoadScene(levelName);
    }
  }
}