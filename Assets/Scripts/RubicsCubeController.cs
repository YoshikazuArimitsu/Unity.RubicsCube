using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubicsCubeController : MonoBehaviour {
	private Transform[,,] Pieces_ = new Transform[3, 3, 3];
	private Transform Core_;

	private int[] locatePiece(Transform o) {
		int[] r = new int[3];
		r [0] = (o.transform.localPosition.x < Core_.transform.localPosition.x) ? 0 :
			(o.transform.localPosition.x > Core_.transform.localPosition.x) ? 2 : 1;
		r [1] = (o.transform.localPosition.y < Core_.transform.localPosition.y) ? 0 :
			(o.transform.localPosition.y > Core_.transform.localPosition.y) ? 2 : 1;
		r [2] = (o.transform.localPosition.z < Core_.transform.localPosition.z) ? 0 :
			(o.transform.localPosition.z > Core_.transform.localPosition.z) ? 2 : 1;
		return r;
	}

	// Use this for initialization
	void Start () {
		// Core
		Core_ = transform.Find ("Core");
		Pieces_ [1, 1, 1] = Core_;
		Debug.LogFormat ("Core : {0} -> (1, 1, 1)", Core_.transform.localPosition);

		// Centers
		for (int i = 1; i <= 6; i++) {
			var t = transform.Find ("Center Piece " + i);
			var l = locatePiece (t);
			Pieces_ [l [0], l [1], l [2]] = t;
			Debug.LogFormat ("Center : {0} -> ({1}, {2}, {3})",
				t.transform.localPosition,
				l[0], l[1], l[2]);
		}

		// Corners
		for (int i = 1; i <= 6; i++) {
			var t = transform.Find ("Corner Piece " + i);
			var l = locatePiece (t);
			Pieces_ [l [0], l [1], l [2]] = t;
			Debug.LogFormat ("Corner : {0} -> ({1}, {2}, {3})",
				t.transform.localPosition,
				l[0], l[1], l[2]);
		}

		// Centers
		for (int i = 1; i <= 12; i++) {
			var t = transform.Find ("Edge Piece " + i);
			var l = locatePiece (t);
			Pieces_ [l [0], l [1], l [2]] = t;
			Debug.LogFormat ("Edge : {0} -> ({1}, {2}, {3})",
				t.transform.localPosition,
				l[0], l[1], l[2]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
