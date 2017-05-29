using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Transform RubiksCube;
    public Transform HandDraggableMarker;
    public Transform Teretteree;
    private bool firstUpdated_ = false;

    public enum GameState {
        Play,       // ゲーム中状態
        Terettere   // テーレッテレー状態
    };

    private GameState GameState_;

	// Use this for initialization
	void Start () {
        GameState_ = GameState.Play;
    }

    // Update is called once per frame
    void Update () {
        if(!firstUpdated_) {
            UpdateControlls();
            firstUpdated_ = true;
        }
    }

    private void UpdateControlls() {
        switch(GameState_) {
            case GameState.Play:
                HandDraggableMarker.GetComponent<HandDraggableMarker>().IsDraggingEnabled = true;
                Teretteree.GetComponent<GameClearController>().SetEnable(false);
                break;
            case GameState.Terettere:
                HandDraggableMarker.GetComponent<HandDraggableMarker>().IsDraggingEnabled = true;
                Teretteree.GetComponent<GameClearController>().SetEnable(false);
                break;
        }
    }
}
