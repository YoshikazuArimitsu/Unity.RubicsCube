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

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateInsideCube() {
		Transform newInside = null;

		// この面から親キューブにレイを飛ばし、更に一つ奥に位置するキューブを得る
		var vec = BaseCube_.transform.position - transform.position;
		Ray ray = new Ray (BaseCube_.transform.position, vec.normalized);
		RaycastHit hit;

		// Rayが衝突したかどうか
		if (Physics.Raycast (ray, out hit)) {
			newInside = hit.collider.GetComponent<Transform> ();
		}

		/*
		if (newInside != InsideCube_) {
			Debug.LogFormat ("{0}-{1} Update InsideCube={2}",
				BaseCube_.name, name,
				newInside != null ? newInside.name : "null");
		}
		*/
		InsideCube_ = newInside;
	}

    void OnMouseUp() {
        RubicsCubeController_.OnDragCancel();
    }

    void OnMouseDown() {
		RubicsCubeController_.OnDragStart(BaseCube_, InsideCube_);
    }

    void OnMouseOver() {
		RubicsCubeController_.OnDragOver(BaseCube_, InsideCube_);
    }
}
