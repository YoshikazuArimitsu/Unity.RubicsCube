using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleController : MonoBehaviour {
    // UnityResource
    public GameObject RubiksCubeController;

    private RubiksCubeController RubiksCubeController_;
    private CustomTestButton CustomTestButton_;

	// Use this for initialization
	void Start () {
        CustomTestButton_ = GetComponent<CustomTestButton>();
        CustomTestButton_.Activated += CustomTestButton__Activated;

        RubiksCubeController_ = RubiksCubeController.GetComponent<RubiksCubeController>();
        RubiksCubeController_.ShuffleCompleted += RubiksCubeController__ShuffleCompleted;
    }

    private void RubiksCubeController__ShuffleCompleted() {
        CustomTestButton_.EnableActivation = true;
        CustomTestButton_.Selected = false;
    }

    private void CustomTestButton__Activated(CustomTestButton source) {
        // ボタンは押せなくする
        CustomTestButton_.EnableActivation = false;

        // シャッフル開始
        RubiksCubeController_.FireShuffle();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
