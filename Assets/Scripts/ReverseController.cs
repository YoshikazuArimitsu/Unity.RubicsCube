using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseController : MonoBehaviour {
    // UnityResource
    public GameObject RubiksCubeController;

    private RubiksCubeController RubiksCubeController_;
    private CustomTestButton CustomTestButton_;

    // Use this for initialization
    void Start () {
        CustomTestButton_ = GetComponent<CustomTestButton>();
        CustomTestButton_.Activated += CustomTestButton__Activated; ;

        RubiksCubeController_ = RubiksCubeController.GetComponent<RubiksCubeController>();
        RubiksCubeController_.ReverseCompleted += RubiksCubeController__ReverseCompleted;
    }

    private void RubiksCubeController__ReverseCompleted() {
        CustomTestButton_.EnableActivation = true;
        CustomTestButton_.Selected = false;
    }

    private void CustomTestButton__Activated(CustomTestButton source) {
        // ボタンは押せなくする
        CustomTestButton_.EnableActivation = false;

        // 反転開始
        RubiksCubeController_.FireReverse();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
