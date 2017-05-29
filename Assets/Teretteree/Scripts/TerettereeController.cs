using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerettereeController : MonoBehaviour {
	public Material Material1_;
	public Material Material2_;
	public Material Material3_;

	public int Duration1 = 200;
	public int Duration2 = 100;
	public int Duration3 = 200;

    public bool Enabled = false;

	private float BaseTime_;
	private Transform[] Spheres_;

	// Use this for initialization
	void Start () {
		BaseTime_ = 0.0f;

		List<Transform> spheres = new List<Transform> ();
		for (int i = 1; i <= 12 ; i++) {
			var s = transform.Find ("Sphere " + i);
			spheres.Add (s);
		}
		Spheres_ = spheres.ToArray ();
	}
	
	// Update is called once per frame
	void Update () {
        if(Enabled == false) {
            return;
        }

		BaseTime_ += Time.deltaTime;

		int totalDuration = Duration1 + Duration2 * 2 + Duration3;
		int currentT = (int)(BaseTime_ * 1000) % totalDuration;

		int spheresDiff = totalDuration / Spheres_.Length;
		for (int i = 0; i < Spheres_.Length; i++) {
			var s = Spheres_ [i];
			var st = (currentT + (spheresDiff * i)) % totalDuration;

			Material m;
			if (st < Duration1) {
				m = Material1_;
			} else if (st > (Duration1 + Duration2) && st < (Duration1 + Duration2 + Duration3)) {
				m = Material3_;
			} else {
				m = Material2_;
			}

			{
				var r = s.GetComponent<MeshRenderer> ();
				if (m == Material1_) {
					r.enabled = false;
				} else {
					r.enabled = true;
				}
			}

			s.GetComponent<Renderer> ().material = m;
		}
	}

    public void SetEnable(bool enable) {
        Enabled = enable;
        if(!Enabled) {
            foreach(var s in Spheres_) {
                s.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
