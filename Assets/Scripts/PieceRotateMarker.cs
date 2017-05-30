﻿using HoloToolkit.Unity;
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
}
