using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZCC;

public class App : MonoBehaviour {

    public Transform ObjectPoolRoot { get; private set; }

    public static App Instance { get; private set; } = null;

    private void Awake() {
        Instance = this;
        ObjectPoolRoot = transform.Find("ObjectPoolRoot");
    }

    private void Start() {
        GameEntrance();
    }

    private void Update() {
    }

    private void OnDestroy() {
    }

    private void OnApplicationQuit() {
    }

    private void OnApplicationPause(bool pause) {
    }

    /// <summary>游戏入口</summary>
    private void GameEntrance() {
        Utility.Init();
        WordsResource.Init();
        
    }

    public void Exit() {
        Application.Quit();
    }

    public void EnterGame() {
        SceneManager.LoadScene(1);
    }

}
