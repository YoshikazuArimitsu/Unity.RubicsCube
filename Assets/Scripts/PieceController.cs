using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {
    private Transform BaseCube_;
    private RubicsCubeController RubicsCubeController_;

	// Use this for initialization
	void Start () {
        BaseCube_ = transform.parent;
        RubicsCubeController_ = BaseCube_.parent.GetComponent<RubicsCubeController>();

        if(RubicsCubeController_ == null) {
            Debug.Log("Error?");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown() {
        Debug.LogFormat("OnMouseDown on {0}", transform.name);
    }
}
