using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {
    private Transform BaseCube_;
    private RubicsCubeController RubicsCubeController_;

    private bool RaisedFlick_ = false;
    private Vector3 MouseDownPosition_;
    public int FlickThreshold = 20;

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
        Debug.LogFormat("OnMouseDown on {0} : {1}", BaseCube_.name, Input.mousePosition);
        RaisedFlick_ = false;
        MouseDownPosition_ = Input.mousePosition;
    }

    void OnMouseDrag() {
        if(!RubicsCubeController_.IsEnablePieceDrag()) {
            return;
        }

        //Debug.LogFormat("OnMouseDrag on {0}", BaseCube_.name);
        if(!RaisedFlick_) {
            if (Input.mousePosition.x > MouseDownPosition_.x + FlickThreshold) {
                Debug.Log("Flick Right!");
                RaisedFlick_ = true;
            }

            if (Input.mousePosition.x < MouseDownPosition_.x - FlickThreshold) {
                Debug.Log("Flick Left!");
                RaisedFlick_ = true;
            }

            if (Input.mousePosition.y > MouseDownPosition_.y + FlickThreshold) {
                Debug.Log("Flick Up!");
                RaisedFlick_ = true;
            }

            if (Input.mousePosition.y < MouseDownPosition_.y - FlickThreshold) {
                Debug.Log("Flick Down!");
                RaisedFlick_ = true;
            }
        }
    }
}
