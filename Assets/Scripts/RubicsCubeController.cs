using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RotatePattern {
    public int FilterX;
    public int FilterY;
    public int FilterZ;
    public bool Direction;

    public RotatePattern(int filterX, int filterY, int filterZ, bool direction) {
        FilterX = filterX;
        FilterY = filterY;
        FilterZ = filterZ;
        Direction = direction;
    }
}

public class RubicsCubeController : MonoBehaviour {
    // 中心キューブ
	private Transform Core_;
    // 3x3x3 ピース配列
    private Transform[,,] Pieces_ = new Transform[3, 3, 3];

    // 回転パターン
    private RotatePattern[] RotatePatterns_ = new RotatePattern[] {
        new RotatePattern(0, -1, -1, true),
        new RotatePattern(0, -1, -1, false),
        new RotatePattern(2, -1, -1, true),
        new RotatePattern(2, -1, -1, false),
        new RotatePattern(-1, 0, -1, true),
        new RotatePattern(-1, 0, -1, false),
        new RotatePattern(-1, 2, -1, true),
        new RotatePattern(-1, 2, -1, false),
        new RotatePattern(-1, -1, 0, true),
        new RotatePattern(-1, -1, 0, false),
        new RotatePattern(-1, -1, 2, true),
        new RotatePattern(-1, -1, 2, false),
    };

    // 回転状態パラメータ
    private bool IsRotation_ = false;
    private float TotalRotate_ = 0.0f;
    private bool RotateDirection_ = false;
    private Transform[] RotateTransforms_;
    private Transform RotateCenter_;
    private Vector3 RotateAxis_;

    private int[] locatePiece(Transform o, float threashold = 0.3f) {
		int[] r = new int[3];
		r [0] = (o.transform.localPosition.x < Core_.transform.localPosition.x - threashold) ? 0 :
			(o.transform.localPosition.x > Core_.transform.localPosition.x + threashold) ? 2 : 1;
		r [1] = (o.transform.localPosition.y < Core_.transform.localPosition.y - threashold) ? 0 :
			(o.transform.localPosition.y > Core_.transform.localPosition.y + threashold) ? 2 : 1;
		r [2] = (o.transform.localPosition.z < Core_.transform.localPosition.z - threashold) ? 0 :
			(o.transform.localPosition.z > Core_.transform.localPosition.z + threashold) ? 2 : 1;
		return r;
	}

    private Transform[] filterPiece(int x = -1, int y = -1, int z = -1) {
        List<Transform> transformList = new List<Transform>();

        for(int _x = 0; _x < 3; _x++) {
            for (int _y = 0; _y < 3; _y++) {
                for (int _z = 0; _z < 3; _z++) {
                    if( (x > -1 && x == _x) ||
                        (y > -1 && y == _y) ||
                        (z > -1 && z == _z) ) {
                        transformList.Add(Pieces_[_x, _y, _z]);
                    }
                }
            }
        }
        return transformList.ToArray();
    }

    private Transform lookupCenter(Transform[] transforms) {
        foreach(var t in transforms) {
            if(t.name.StartsWith("Center Piece")) {
                return t;
            }
        }
        return null;
    }

    private void rebuildPieces() {
        var pieces = new Transform[3, 3, 3]; ;
        // Core
        Core_ = transform.Find("Core");
        pieces[1, 1, 1] = Core_;
        Debug.LogFormat("Core : {0} -> (1, 1, 1)", Core_.transform.localPosition);

        // Centers
        for (int i = 1; i <= 6; i++) {
            var t = transform.Find("Center Piece " + i);
            var l = locatePiece(t);
            pieces[l[0], l[1], l[2]] = t;
            Debug.LogFormat("Center : {0} -> ({1}, {2}, {3})",
                t.transform.localPosition,
                l[0], l[1], l[2]);
        }

        // Corners
        for (int i = 1; i <= 8; i++) {
            var t = transform.Find("Corner Piece " + i);
            var l = locatePiece(t);
            pieces[l[0], l[1], l[2]] = t;
            Debug.LogFormat("Corner : {0} -> ({1}, {2}, {3})",
                t.transform.localPosition,
                l[0], l[1], l[2]);
        }

        // Centers
        for (int i = 1; i <= 12; i++) {
            var t = transform.Find("Edge Piece " + i);
            var l = locatePiece(t);
            pieces[l[0], l[1], l[2]] = t;
            Debug.LogFormat("Edge : {0} -> ({1}, {2}, {3})",
                t.transform.localPosition,
                l[0], l[1], l[2]);
        }

        Pieces_ = pieces;
    }

	// Use this for initialization
	void Start () {
        rebuildPieces();
	}
	
	// Update is called once per frame
	void Update () {
        float rotateSpeed = 3.0f;

        // 回転トリガー
        if (!IsRotation_) {
            KeyCode[] keyCodes = new KeyCode[] {
                KeyCode.Alpha0,
                KeyCode.Alpha1,
                KeyCode.Alpha2,
                KeyCode.Alpha3,
                KeyCode.Alpha4,
                KeyCode.Alpha5,
                KeyCode.Alpha6,
                KeyCode.Alpha7,
                KeyCode.Alpha8,
                KeyCode.Alpha9,
                KeyCode.A,
                KeyCode.B
            };

            for(int i = 0; i < keyCodes.Length; ++i) {
                if(Input.GetKey(keyCodes[i])) {
                    var r = RotatePatterns_[i];
                    RotateTransforms_ = filterPiece(r.FilterX, r.FilterY, r.FilterZ);
                    RotateDirection_ = r.Direction;
                    RotateCenter_ = lookupCenter(RotateTransforms_);
                    RotateAxis_ = RotateCenter_.transform.position - Core_.transform.position;
                    TotalRotate_ = 0.0f;
                    IsRotation_ = true;

                    Debug.LogFormat("Trigger Rotation : (x/y/z)=({0}, {1}, {2}), dir={3}",
                        r.FilterX, r.FilterY, r.FilterZ, r.Direction);
                    break;
                }
            }
        }

        // 回転
        if (IsRotation_) {
            float rotate = rotateSpeed * (RotateDirection_ ? 90.0f : -90.0f);
            float deltaAngle = rotate * Time.deltaTime;

            if (Mathf.Abs(deltaAngle) + TotalRotate_ >= 90.0f) {
                float a = 90.0f - TotalRotate_;
                deltaAngle = (deltaAngle > 0.0f) ? a : -a;
            }
            Debug.LogFormat("Do Rotation : delta={0}, total={1}", deltaAngle, TotalRotate_);

            foreach (var t in RotateTransforms_) {
                t.RotateAround(RotateCenter_.transform.position, RotateAxis_, deltaAngle);
            }

            TotalRotate_ += Mathf.Abs(deltaAngle);
            if (TotalRotate_ >= 90.0f) {
                Debug.Log("Do Rotation : 90deg rotate complete.");
                IsRotation_ = false;

                rebuildPieces();
            }
        }
    }

    public Vector3 GetPieceLocation(Transform t) {
        for (int _x = 0; _x < 3; _x++) {
            for (int _y = 0; _y < 3; _y++) {
                for (int _z = 0; _z < 3; _z++) {
                    if(Pieces_[_x, _y, _z] == t) {
                        return new Vector3(_x, _y, _z);
                    }
                }
            }
        }
        return Vector3.zero;
    }

    public bool IsEnablePieceDrag() {
        // 回転中なら拒否
        if(IsRotation_) {
            return false;
        }
        return true;
    }

    private List<Transform> DragCubes_ = new List<Transform>();

    public void OnDragCancel() {
        DragCubes_.Clear();
    }

    public void OnDragStart(Transform t) {
        DragCubes_.Clear();
        DragCubes_.Add(t);
    }

    public void OnDragOver(Transform t) {
        // 無効
        if(DragCubes_.Count == 0 || DragCubes_.Contains(t) || !IsEnablePieceDrag()) { 
            return;
        }

        DragCubes_.Add(t);
        // ドラッグで通過したキューブを含む平面が一つに特定できるか？
        var surface = detectSurface(DragCubes_);
        if(surface == null) {
            // 一つに特定できない
            return;
        }

        Debug.Log("Fire Rotate!");
        DragCubes_.Clear();
        /*
        var from = DragFrom_;
        var to = t;
        DragFrom_ = null;
        Debug.LogFormat("Raise drag {0} -> {1}", from.name, to.name);
        */
    }

    private bool ContainsAll(Transform[] arrays, Transform[] targets) {
        foreach(var t in targets) {
            bool contain = false;
            foreach(var a in arrays) {
                if(a == t) {
                    contain = true;
                    break;
                }
            }

            if(!contain) {
                return false;
            }
        }
        return true;
    }

    private Transform[] detectSurface(List<Transform> transforms) {

        string names = "";
        foreach(var t in transforms) {
            if(string.IsNullOrEmpty(names)) {
                names = t.name;
            } else {
                names += ", " + t.name;
            }
        }
        Debug.LogFormat("DetectSurface : Cubes=[{0}]", names);

        int[,] filterParams = {
            {0, -1, -1 },
            {2, -1, -1 },
            {-1, 0, -1 },
            {-1, 2, -1 },
            {-1, -1, 0 },
            {-1, -1, 2 },
        };

        Transform[] surface = null;

        for(int i = 0; i < 6 ; i++) {
            var s = filterPiece(filterParams[i, 0], filterParams[i, 1], filterParams[i, 2]);

            if(ContainsAll(s, transforms.ToArray())) {
                if(surface != null) {
                    Debug.Log("Matched multiple surface");
                    return null;
                }

                Debug.LogFormat("DetectSurface : Found ({0}, {1}, {2})",
                    filterParams[i, 0], filterParams[i, 1], filterParams[i, 2]);

                surface = s;
            }
        }
        return surface;
    }

}
