using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class GameClearController : MonoBehaviour {
    private bool Enabled_;
    private Transform Background_;
    private Transform[] Bars_;
	// Use this for initialization
	void Start () {
        Background_ = transform.Find("BackGround");

        var bars = new List<Transform>();
        for(int i = 1; i <= 12; i++) {
            bars.Add(transform.Find("TerettereeBar " + i));
        }
        Bars_ = bars.ToArray();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetEnable(bool enable) {
        Enabled_ = enable;

        // カメラBloom
        if(Enabled_) {
            Camera.main.GetComponent<Bloom>().bloomIntensity = 10;
        } else {
            Camera.main.GetComponent<Bloom>().bloomIntensity = 0;
        }

        // テーレッテレー背景
        if (Enabled_) {
            Background_.GetComponent<MeshRenderer>().enabled = true;
        } else {
            Background_.GetComponent<MeshRenderer>().enabled = false;
        }

        // テーレッテレー
        foreach (var t in Bars_) {
            t.GetComponent<TerettereeController>().SetEnable(enable);
        }
    }
}
