using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceRotateMarker : Singleton<PieceRotateMarker> {
    public bool Enable_ = true;
    private List<MeshRenderer> ChildRenderers_ = new List<MeshRenderer>();

	// Use this for initialization
	void Start () {
        ChildRenderers_.Add(transform.Find("Back").GetComponent<MeshRenderer>());
        ChildRenderers_.Add(transform.Find("Arrow/Cylinder").GetComponent<MeshRenderer>());
        ChildRenderers_.Add(transform.Find("Arrow/Cone").GetComponent<MeshRenderer>());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetEnable(bool enable) {
        foreach(var r in ChildRenderers_) {
            r.enabled = enable;
        }
        Enable_ = enable;
    }

    public void UpdateTransform(SurfaceController select) {
        if(select != null) {
            SetEnable(true);

            // 位置更新
            Debug.LogFormat("SurfaceLoc : {0}", select.transform.position);
            transform.eulerAngles = select.transform.eulerAngles;
            transform.position = select.transform.position;
        } else {
            SetEnable(false);
        }
    }
}
