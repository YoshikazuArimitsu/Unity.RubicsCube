using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustController : MonoBehaviour {
    public Transform GameManager;
    private CustomTestButton CustomTestButton_;

    // Use this for initialization
    void Start () {
        CustomTestButton_ = GetComponent<CustomTestButton>();
        CustomTestButton_.Activated += CustomTestButton__Activated;

    }

    private void CustomTestButton__Activated(CustomTestButton source) {
        GameManager.GetComponent<GameManager>().SetAdjusting(source.Selected);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
