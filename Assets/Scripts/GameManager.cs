using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour, IInputClickHandler {
    public Transform RubiksCube;
    public Transform HandDraggableMarker;
    public Transform Teretteree;
    public Transform ShuffleButton;
    public Transform ReverseButton;
    private bool firstUpdated_ = false;

    public enum GameState {
        Play,       // ゲーム中状態
        Terettere   // テーレッテレー状態
    };

    private GameState GameState_;

	// Use this for initialization
	void Start () {
        GameState_ = GameState.Play;

        RubiksCube.GetComponent<RubiksCubeController>().Cleared += GameManager_Cleared;
    }

    private void GameManager_Cleared() {
        GameState_ = GameState.Terettere;
        UpdateControlls();

        InputManager.Instance.AddGlobalListener(gameObject);
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
                // 各コントローラを操作可能に
                RubiksCube.GetComponent<RubiksCubeController>().ControlEnabled = true;
                HandDraggableMarker.GetComponent<HandDraggableMarker>().IsDraggingEnabled = true;
                Teretteree.GetComponent<GameClearController>().SetEnable(false);
                ShuffleButton.GetComponent<CustomTestButton>().EnableActivation = true;
                ReverseButton.GetComponent<CustomTestButton>().EnableActivation = true;
                break;

            case GameState.Terettere:
                // 各コントローラを操作不可能に
                RubiksCube.GetComponent<RubiksCubeController>().ControlEnabled = false;
                HandDraggableMarker.GetComponent<HandDraggableMarker>().IsDraggingEnabled = false;
                Teretteree.GetComponent<GameClearController>().SetEnable(true);
                ShuffleButton.GetComponent<CustomTestButton>().EnableActivation = false;
                ReverseButton.GetComponent<CustomTestButton>().EnableActivation = false;
                break;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData) {
        if(GameState_ != GameState.Terettere) {
            return;
        }

        GameState_ = GameState.Play;
        UpdateControlls();

        InputManager.Instance.RemoveGlobalListener(gameObject);
    }
}
