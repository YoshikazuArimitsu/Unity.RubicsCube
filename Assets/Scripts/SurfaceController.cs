using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceController : MonoBehaviour {
    /// <summary>
    /// この面が属するピース
    /// </summary>
    private Transform BasePiece_;

    /// <summary>
    /// 親Controller
    /// </summary>
    private RubiksCubeController RubiksCubeController_;

    /// <summary>
    /// この面に対して垂直ベクトルを引き、内側一個奥に位置するピース
    /// </summary>
	private Transform InsidePiece_;

    /// <summary>
    /// 初期状態(つまり揃った状態)での InsidePiece_
    /// ６面揃った状態になると全ての InsidePiece が初期状態と同じになる
    /// </summary>
    private Transform InitInsidePiece_;

    // Use this for initialization
    void Start () {
        BasePiece_ = transform.parent;
		RubiksCubeController_ = BasePiece_.parent.GetComponent<RubiksCubeController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateInsideCube() {
		Transform newInside = null;

		// この面から親ピースにレイを飛ばし、更に一つ奥に位置するピースを得る
		var vec = BasePiece_.transform.position - transform.position;
		Ray ray = new Ray (BasePiece_.transform.position, vec.normalized);
		RaycastHit hit;

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
		InsidePiece_ = newInside;

		// 初回
		InitInsidePiece_ = (InitInsidePiece_ == null) ? InsidePiece_ : InitInsidePiece_;
	}

    void OnMouseUp() {
        RubiksCubeController_.OnDragCancel();
    }

    void OnMouseDown() {
		RubiksCubeController_.OnDragStart(BasePiece_, InsidePiece_);
    }

    void OnMouseOver() {
		RubiksCubeController_.OnDragOver(BasePiece_, InsidePiece_);
    }

	public bool HasInitInside() {
		return InsidePiece_ == InitInsidePiece_;
	}
}
