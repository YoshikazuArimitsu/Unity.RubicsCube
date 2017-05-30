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
    public Transform AdjustButton;
    private bool firstUpdated_ = false;

    public enum GameState {
        Play,       // ゲーム中状態
        Adjusting,  // 位置調整中
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
                RubiksCube.GetComponent<RubiksCubeController>().SetEnable(true);
                HandDraggableMarker.GetComponent<HandDraggableMarker>().IsDraggingEnabled = true;
                ShuffleButton.GetComponent<CustomTestButton>().EnableActivation = true;
                ReverseButton.GetComponent<CustomTestButton>().EnableActivation = true;
                AdjustButton.GetComponent<CustomTestButton>().EnableActivation = true;

                // シャッフル・回転ボタンを見せる
                ShuffleButton.GetComponent<CustomTestButton>().Show(true);
                ReverseButton.GetComponent<CustomTestButton>().Show(true);

                // テーレッテレー OFF
                Teretteree.GetComponent<GameClearController>().SetEnable(false);

                // ゲーム領域移動不可
                RubiksCube.GetComponent<HandDraggable>().IsDraggingEnabled = false;
                break;

            case GameState.Adjusting:
                // 各コントローラを操作不可能に
                RubiksCube.GetComponent<RubiksCubeController>().SetEnable(false);
                HandDraggableMarker.GetComponent<HandDraggableMarker>().IsDraggingEnabled = false;

                // シャッフル・回転ボタンを隠す
                ShuffleButton.GetComponent<CustomTestButton>().Show(false);
                ReverseButton.GetComponent<CustomTestButton>().Show(false);

                // テーレッテレー OFF
                Teretteree.GetComponent<GameClearController>().SetEnable(false);

                // ゲーム領域移動可
                RubiksCube.GetComponent<HandDraggable>().IsDraggingEnabled = true;
                break;

            case GameState.Terettere:
                // 各コントローラを操作不可能に
                RubiksCube.GetComponent<RubiksCubeController>().SetEnable(false);
                HandDraggableMarker.GetComponent<HandDraggableMarker>().IsDraggingEnabled = false;
                ShuffleButton.GetComponent<CustomTestButton>().EnableActivation = false;
                ReverseButton.GetComponent<CustomTestButton>().EnableActivation = false;
                AdjustButton.GetComponent<CustomTestButton>().EnableActivation = false;

                // テーレッテレー ON
                Teretteree.GetComponent<GameClearController>().SetEnable(true);
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

    public void SetAdjusting(bool adjusting) {
        Debug.LogFormat("Adjusting : {0}", adjusting);

        if(adjusting) {
            GameState_ = GameState.Adjusting;
        } else {
            GameState_ = GameState.Play;
        }
        UpdateControlls();
    }
}
