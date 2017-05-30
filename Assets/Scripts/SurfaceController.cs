using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SurfaceController : MonoBehaviour, IInputClickHandler {
    private bool Selected_ = false;
    public Material Material;
    public Material SelectedMaterial;

    private bool ControlEnable_ = true;

    public void SetEnable(bool enable) {
        ControlEnable_ = enable;
    }


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

        UpdateInsideCube();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Select(bool selected) {
        if(Selected_ != selected) {
            // マテリアル切り替え
            GetComponent<Renderer>().material = selected ? SelectedMaterial : Material;
        }

        Selected_ = selected;
    }

	public void UpdateInsideCube() {
        if(BasePiece_ == null) {
            return;
        }
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

    /*
    void OnMouseUp() {
        RubiksCubeController_.OnDragCancel();
    }

    void OnMouseDown() {
		RubiksCubeController_.OnDragStart(BasePiece_, InsidePiece_);
    }

    void OnMouseOver() {
		RubiksCubeController_.OnDragOver(BasePiece_, InsidePiece_);
    }
    */

	public bool HasInitInside() {
		return InsidePiece_ == InitInsidePiece_;
	}

    public void InputDown() {
        if (!ControlEnable_) {
            return;
        }

        RubiksCubeController_.OnDragStart(this, BasePiece_, InsidePiece_);
    }

    public void InputOver() {
        if (!ControlEnable_) {
            return;
        }

        RubiksCubeController_.OnDragOver(this, BasePiece_, InsidePiece_);
    }

    public void OnInputClicked(InputClickedEventData eventData) {
        if(!ControlEnable_) {
            return;
        }
        Debug.LogFormat("InputClick");
    }
}
