using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {
    private Transform BaseCube_;
    private RubicsCubeController RubicsCubeController_;
	private Transform InsideCube_;

    /*
    private bool RaisedFlick_ = false;
    private Vector3 MouseDownPosition_;
    public int FlickThreshold = 20;
    */

	// Use this for initialization
	void Start () {
        BaseCube_ = transform.parent;
        RubicsCubeController_ = BaseCube_.parent.GetComponent<RubicsCubeController>();

		// この面から親キューブにレイを飛ばし、更に一つ奥に位置するキューブを得る
		var vec = BaseCube_.transform.position - transform.position;
		Ray ray = new Ray (BaseCube_.transform.position, vec.normalized);
		RaycastHit hit;

		// Rayが衝突したかどうか
		if (Physics.Raycast (ray, out hit)) {
			InsideCube_ = hit.collider.GetComponent<Transform> ();
		} else {
		}

		Debug.LogFormat("Start : {0}/{1} Inside={2}",
			BaseCube_.name, name,
			InsideCube_ != null ? InsideCube_.name : "null");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseUp() {
        RubicsCubeController_.OnDragCancel();
    }

    void OnMouseDown() {
        RubicsCubeController_.OnDragStart(BaseCube_);
        /*
        RaisedFlick_ = false;
        MouseDownPosition_ = Input.mousePosition;

        {
            Vector3 loc = RubicsCubeController_.GetPieceLocation(BaseCube_);
            Debug.LogFormat("OnMouseDown on {0} : {1}", BaseCube_.name, loc);
        }
        */
    }

    void OnMouseOver() {
        RubicsCubeController_.OnDragOver(BaseCube_);
    }

    void OnMouseDrag() {
        /*
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
        */
    }
}
