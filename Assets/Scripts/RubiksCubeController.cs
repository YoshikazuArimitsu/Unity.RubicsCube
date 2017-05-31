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

public class RubiksCubeController : MonoBehaviour {
	// Unity Properties
	public AudioClip RotateAudio;
	public float RotateSpeed = 3.0f;

    private bool ControlEnable_ = true;

    public void SetEnable(bool enable) {
        ControlEnable_ = enable;

        foreach (var s in AllSurfaceControllers()) {
            s.SetEnable(enable);
        }
    }

    // 中心ピース
	private Transform Core_;
    // 3x3x3 ピース配列
    private Transform[,,] Pieces_ = new Transform[3, 3, 3];

    // 回転パターン
    private RotatePattern[] RotatePatterns_ = new RotatePattern[] {
        // 左面
        new RotatePattern(0, -1, -1, true ),
        new RotatePattern(0, -1, -1, false),
        // 中(LR)面
		new RotatePattern(1, -1, -1, true ),
		new RotatePattern(1, -1, -1, false),
        // 右面
        new RotatePattern(2, -1, -1, true ),
        new RotatePattern(2, -1, -1, false),
        // 下面
        new RotatePattern(-1, 0, -1, true ),
        new RotatePattern(-1, 0, -1, false),
        // 中(UD)面
		new RotatePattern(-1, 1, -1, true ),
		new RotatePattern(-1, 1, -1, false),
        // 上面
        new RotatePattern(-1, 2, -1, true ),
        new RotatePattern(-1, 2, -1, false),
        // 前面
        new RotatePattern(-1, -1, 0, true ),
        new RotatePattern(-1, -1, 0, false),
        // 中(前後)
		new RotatePattern(-1, -1, 1, true ),
		new RotatePattern(-1, -1, 1, false),
        // 後面
        new RotatePattern(-1, -1, 2, true ),
        new RotatePattern(-1, -1, 2, false),
    };

    public enum ManipulateMode {
        None,
        PieceDrag,
        PieceClick
    };

    private ManipulateMode ManipulateMode_ = ManipulateMode.None;

	// ドラッグ起点&内側ピース
	private Transform DragOrigin_;
	private Transform DragOriginInside_;

    // 選択中ピース面
    private PieceRotateMarker PiiceRotateMarker_;
    private Transform ClickOrigin_;

    // 回転状態パラメータ
    private bool IsRotation_ = false;       // 回転フラグ
    private float TotalRotate_ = 0.0f;      // トータル回転角
    private Vector3 RotateAxis_;            // 回転軸
    private bool RotateDirection_ = false;  // 回転方向
    private Transform[] RotateTransforms_;  // 回転するピースの集合(9)
    private Transform RotateCenter_;        // RotationTransforms中心ピース

	// シャッフルモード(Count>0)
	private int ShuffleCount_ = 0;
	private float TotalRotateReverse_ = 0.0f;
    public delegate void ShuffleCompleteDelegate();
    public event ShuffleCompleteDelegate ShuffleCompleted;

    // 反転
    private bool IsReversed_ = false;
    private bool IsReversing_ = false;
    public delegate void ReverseCompleteDelegate();
    public event ReverseCompleteDelegate ReverseCompleted;

    // クリア
    public delegate void ClearDelegate();
    public event ClearDelegate Cleared;


    // Use this for initialization
    void Start () {
        PiiceRotateMarker_ = transform.Find("PieceRotateMarker").GetComponent<PieceRotateMarker>();
        UpdatePieceRotateMarker();

        RebuildPieces();
	}

    /// <summary>
    /// point を pivot 中心に angle 回転する。
    /// </summary>
    /// <param name="point"></param>
    /// <param name="pivot"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle) {
        var finalPos = point - pivot;
        finalPos = angle * finalPos;
        finalPos += pivot;
        return finalPos;
    }

    /// <summary>
    /// Arraysの配列がtargetsの配列の内容を全て含むか
    /// </summary>
    /// <param name="arrays">Arrays.</param>
    /// <param name="targets">Targets.</param>
    private static bool ContainsAll(Transform[] arrays, Transform[] targets) {
        foreach (var t in targets) {
            bool contain = false;
            foreach (var a in arrays) {
                if (a == t) {
                    contain = true;
                    break;
                }
            }

            if (!contain) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ピースのローカル座標から Piece_ 配列内 x,y,z に変換
    /// </summary>
    /// <param name="o"></param>
    /// <param name="threashold"></param>
    /// <returns></returns>
    private int[] LocatePiece(Transform o, float threashold = 0.3f) {
		int[] r = new int[3];
		r [0] = (o.transform.localPosition.x < Core_.transform.localPosition.x - threashold) ? 0 :
			(o.transform.localPosition.x > Core_.transform.localPosition.x + threashold) ? 2 : 1;
		r [1] = (o.transform.localPosition.y < Core_.transform.localPosition.y - threashold) ? 0 :
			(o.transform.localPosition.y > Core_.transform.localPosition.y + threashold) ? 2 : 1;
		r [2] = (o.transform.localPosition.z < Core_.transform.localPosition.z - threashold) ? 0 :
			(o.transform.localPosition.z > Core_.transform.localPosition.z + threashold) ? 2 : 1;
		return r;
	}

    /// <summary>
    /// 座標 x, y, z (-1は無視) にマッチするピース配列を得る
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private Transform[] FilterPieces(int x = -1, int y = -1, int z = -1) {
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

    /// <summary>
    /// Piece集合からCenter Pieceを得る(複数あった場合は先頭)
    /// </summary>
    /// <param name="transforms"></param>
    /// <returns></returns>
    private Transform LookupCenter(Transform[] transforms) {
        foreach(var t in transforms) {
            if(t.name.StartsWith("Center Piece")) {
                return t;
            }
        }
        return null;
    }

    /// <summary>
    /// 回転の中心ピースを得る
    /// 1. Core Piece を含む場合はそいつ
    /// 2. 含まない場合は Center Piece が一つ含まれる
    /// </summary>
    /// <param name="transforms"></param>
    /// <returns></returns>
	private Transform LookupRotateCenter(Transform[] transforms) {
		foreach (var t in transforms) {
			if (t == Core_) {
				return t;
			}
		}
		return LookupCenter(transforms);
	}

    /// <summary>
    /// transforms平面の回転軸ベクトル取得
    /// </summary>
    /// <param name="transforms"></param>
    /// <returns></returns>
	private Vector3 CalcRotateAxis(Transform[] transforms) {
		var center = LookupRotateCenter(transforms);

		// 中心を含まない場合、キューブ中心から回転面中央キューブへのベクトルが回転軸となる
		if (center != Core_) {
			return center.transform.position - Core_.transform.position;
		}

		// 中心を含む場合、transforms に含まれない 回転面中央キューブ(2) のうち小さいものからのベクトルを取る
		Transform[] targets = new Transform[] {
			Pieces_ [0, 1, 1],
			Pieces_ [1, 0, 1],
			Pieces_ [1, 1, 0]
		};

		foreach (var target in targets) {
			var c = ContainsAll (transforms, new Transform[] { target });

			if (c == false) {
                // 
				return target.transform.position - Core_.transform.position;
			}
		}

		Debug.Log ("Can't find rotateAxis!!");
		return Vector3.zero;
	}

    /// <summary>
    /// Pieces_配列再構築
    /// </summary>
    private void RebuildPieces() {
        var pieces = new Transform[3, 3, 3];
        // Core
        Core_ = transform.Find("Core");
        pieces[1, 1, 1] = Core_;
        //Debug.LogFormat("Core : {0} -> (1, 1, 1)", Core_.transform.localPosition);

        // Centers
        for (int i = 1; i <= 6; i++) {
            var t = transform.Find("Center Piece " + i);
            var l = LocatePiece(t);
            pieces[l[0], l[1], l[2]] = t;

			/*
            Debug.LogFormat("Center : {0} -> ({1}, {2}, {3})",
                t.transform.localPosition,
                l[0], l[1], l[2]);
                */
        }

        // Corners
        for (int i = 1; i <= 8; i++) {
            var t = transform.Find("Corner Piece " + i);
            var l = LocatePiece(t);
            pieces[l[0], l[1], l[2]] = t;

			/*
            Debug.LogFormat("Corner : {0} -> ({1}, {2}, {3})",
                t.transform.localPosition,
                l[0], l[1], l[2]);
                */
        }

        // Centers
        for (int i = 1; i <= 12; i++) {
            var t = transform.Find("Edge Piece " + i);
            var l = LocatePiece(t);
            pieces[l[0], l[1], l[2]] = t;

			/*
            Debug.LogFormat("Edge : {0} -> ({1}, {2}, {3})",
                t.transform.localPosition,
                l[0], l[1], l[2]);
            */
        }

        Pieces_ = pieces;

		UpdateInsidePieces ();
    }

    public static List<SurfaceController> AllSurfaceControllers() {
        var list = new List<SurfaceController>();
        var surfaces = GameObject.FindGameObjectsWithTag("Surface Cube");

        foreach (var s in surfaces) {
            var pc = s.GetComponent<SurfaceController>();
            if (pc != null) {
                list.Add(pc);
            }
        }
        return list;
    }

    /// <summary>
    /// 全SurfaceのInsidePiece更新
    /// </summary>
	private void UpdateInsidePieces() {
        foreach (var s in AllSurfaceControllers()) {
            s.UpdateInsideCube();
        }
    }

    /// <summary>
    /// クリアしたかチェック
    /// </summary>
	private void CheckClear() {
        // 全表面の奥キューブが初期状態のものと一致すればクリア
        foreach (var s in AllSurfaceControllers()) {
            if (!s.HasInitInside()) {
                return;
            }
        }

		//Debug.Log ("Cleared!!!!!");
        if(Cleared != null) {
            Cleared();
        }
	}

    /// <summary>
    /// 回転実行
    /// </summary>
    /// <param name="filterX"></param>
    /// <param name="filterY"></param>
    /// <param name="filterZ"></param>
    /// <param name="direction"></param>
	private void FireRotate(int filterX, int filterY, int filterZ, bool direction) {
        if(ControlEnable_ == false) {
            // 操作不可状態
            return;
        }

		RotateTransforms_ = FilterPieces(filterX, filterY, filterZ);
		RotateCenter_ = LookupRotateCenter (RotateTransforms_);
		RotateAxis_ = CalcRotateAxis (RotateTransforms_);
		RotateDirection_ = direction;
		TotalRotate_ = 0.0f;
		IsRotation_ = true;

		/*
		Debug.LogFormat("Fire Rotation : (x/y/z)=({0}, {1}, {2}), dir={3}",
			filterX, filterY, filterZ, direction);
		*/

		if (RotateAudio != null) {
			var audioSource = gameObject.GetComponent<AudioSource> ();
			audioSource.clip = RotateAudio;
			audioSource.Play ();
		}
	}

    /// <summary>
    /// 回転実行。
    /// 平面と、ユーザ操作によりどのピースからどのピースへフリック操作を行ったかにより回転方向を判定する。
    /// </summary>
    /// <param name="filterX"></param>
    /// <param name="filterY"></param>
    /// <param name="filterZ"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
	private void FireRotateBy2Cube(int filterX, int filterY, int filterZ, Transform from, Transform to) {
		var transforms = FilterPieces(filterX, filterY, filterZ);
		var center = LookupRotateCenter (transforms);
		var axis = CalcRotateAxis (transforms);

		// from を RotateAxis中心に±360/9 回転してみる。
		// to との距離が近くなった方を実際の回転とする。
		var q1 = Quaternion.AngleAxis(360.0f / 9, axis);
		var q2 = Quaternion.AngleAxis(-360.0f / 9, axis);

		var p1 = RotateAroundPoint (from.transform.position, center.position, q1);
		var p2 = RotateAroundPoint (from.transform.position, center.position, q2);

		var d1 = Vector3.Distance (p1, to.transform.position);
		var d2 = Vector3.Distance (p2, to.transform.position);
		/*
		Debug.LogFormat ("from:{0}, from+q1:{1}/{3}, from+q2:{2}/{4}",
			from.transform.position,
			p1,
			p2,
			d1,
			d2);
		*/

		FireRotate (filterX, filterY, filterZ, d1 < d2);
	}

    /// <summary>
    /// シャッフル開始
    /// </summary>
    public void FireShuffle() {
        if (ControlEnable_ == false) {
            // 操作不可状態
            return;
        }

        ShuffleCount_ = 200;
    }

    /// <summary>
    /// 反転開始
    /// </summary>
    public void FireReverse() {
        if (ControlEnable_ == false) {
            // 操作不可状態
            return;
        }

        IsReversing_ = true;
        TotalRotateReverse_ = 0.0f;
    }

    private void FireRotateByKeyboard() {
        // 回転面の特定
        int x = -1, y = -1, z = -1;

        if(!IsReversed_) {
            // 通常時  x / y / z
            // L/M/R/D/E/U/F/S/B面
            x = Input.GetKey(KeyCode.L) ? 0 : x;
            x = Input.GetKey(KeyCode.M) ? 1 : x;
            x = Input.GetKey(KeyCode.R) ? 2 : x;
            if (x == -1) {
                y = Input.GetKey(KeyCode.D) ? 0 : y;
                y = Input.GetKey(KeyCode.E) ? 1 : y;
                y = Input.GetKey(KeyCode.U) ? 2 : y;
            }
            if (x == -1 && y == -1) {
                z = Input.GetKey(KeyCode.F) ? 0 : z;
                z = Input.GetKey(KeyCode.S) ? 1 : z;
                z = Input.GetKey(KeyCode.B) ? 2 : z;
            }
        } else {
            // 反転時 -x / z / y  の形になっている
            // L/M/R/D/E/U/F/S/B面
            x = Input.GetKey(KeyCode.L) ? 2 : x;
            x = Input.GetKey(KeyCode.M) ? 1 : x;
            x = Input.GetKey(KeyCode.R) ? 0 : x;
            if (x == -1) {
                z = Input.GetKey(KeyCode.D) ? 0 : z;
                z = Input.GetKey(KeyCode.E) ? 1 : z;
                z = Input.GetKey(KeyCode.U) ? 2 : z;
            }
            if (x == -1 && z == -1) {
                y = Input.GetKey(KeyCode.F) ? 0 : y;
                y = Input.GetKey(KeyCode.S) ? 1 : y;
                y = Input.GetKey(KeyCode.B) ? 2 : y;
            }
        }

        if (x == -1 && y == -1 && z == -1) {
            return;
        }

        bool direction = true;

        // 反転時のM平面は逆回転
        if(IsReversed_ && x == 1) {
            direction = !direction;
        }

        // Shiftが押されていると更に逆回転
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            direction = !direction;
        }

        FireRotate(x, y, z, direction);
    }

    // Update is called once per frame
    void Update () {
        // 回転トリガー
        if (!IsRotation_) {
            // 新・キーボード回転
            FireRotateByKeyboard();

			// 反転
			if (Input.GetKey (KeyCode.Z)) {
                FireReverse();
                return;
			}

			// シャッフル
			if (Input.GetKey (KeyCode.X)) {
                FireShuffle();
                return;
			}
        }

        // 回転
        if (IsRotation_) {
			// 基本回転速度
			float rotateSpeed = RotateSpeed;

			// シャッフル中なら回転速度8倍
			rotateSpeed = (ShuffleCount_ > 0) ? rotateSpeed * 8 : rotateSpeed;

            float rotate = rotateSpeed * (RotateDirection_ ? 90.0f : -90.0f);
            float deltaAngle = rotate * Time.deltaTime;

            if (Mathf.Abs(deltaAngle) + TotalRotate_ >= 90.0f) {
                float a = 90.0f - TotalRotate_;
                deltaAngle = (deltaAngle > 0.0f) ? a : -a;
            }
            //Debug.LogFormat("Do Rotation : delta={0}, total={1}", deltaAngle, TotalRotate_);

            foreach (var t in RotateTransforms_) {
                t.RotateAround(RotateCenter_.transform.position, RotateAxis_, deltaAngle);
            }

            TotalRotate_ += Mathf.Abs(deltaAngle);
            if (TotalRotate_ >= 90.0f) {
				// 回転完了
                //Debug.Log("rotate complete.");
                IsRotation_ = false;

                // 操作/選択解除
                UpdateManipulateMode(ManipulateMode.None);

                // キューブ情報組み直し
                RebuildPieces();

				if (ShuffleCount_ > 0) {
					// シャッフル中だったら回数デクリメント
					ShuffleCount_ = ShuffleCount_ > 0 ? ShuffleCount_ - 1 : 0;

                    if(ShuffleCount_ == 0) {
                        // シャッフル完了
                        if(ShuffleCompleted != null) {
                            ShuffleCompleted();
                        }
                    }
				} else {
					// クリアかチェック
					CheckClear ();
				}
            }
        }

		// シャッフル
		if (!IsRotation_ && ShuffleCount_ > 0) {
			var r = RotatePatterns_ [Random.Range ((int)0, RotatePatterns_.Length)];
			FireRotate (r.FilterX, r.FilterY, r.FilterZ, r.Direction);
		}

        // 反転
        // 回転中なら回転が完了するまで起動しない
        if (IsReversing_ && !IsRotation_) {
            // 基本回転速度
            float rotateSpeed = RotateSpeed;

            float rotate = rotateSpeed * 180.0f;
            float deltaAngle = rotate * Time.deltaTime;

            if (Mathf.Abs(deltaAngle) + TotalRotateReverse_ >= 180.0f) {
                float a = 180.0f - TotalRotateReverse_;
                deltaAngle = (deltaAngle > 0.0f) ? a : -a;
            }
            transform.Rotate(0.0f, deltaAngle, 0.0f, Space.World);

            TotalRotateReverse_ += Mathf.Abs(deltaAngle);
            if (TotalRotateReverse_ >= 180.0f) {
                // 反転完了
                //Debug.Log ("reverse complete.");
                IsReversing_ = false;
                IsReversed_ = !IsReversed_;

                if (ReverseCompleted != null) {
                    ReverseCompleted();
                }
            }
        }
    }

    private void UpdateManipulateMode(ManipulateMode m) {
        Debug.LogFormat("UpdateManipulateMode : {0}", m.ToString());
        switch(m) {
            case ManipulateMode.None:
                DragOrigin_ = null;
                DragOriginInside_ = null;
                foreach (var s in AllSurfaceControllers()) {
                    s.Select(false);
                }
                break;
            case ManipulateMode.PieceDrag:
            case ManipulateMode.PieceClick:
            default:
                break;
        }

        ManipulateMode_ = m;
        UpdatePieceRotateMarker();

    }

    /// <summary>
    /// ドラッグキャンセル
    /// </summary>
    public void OnDragCancel() {
        if(ManipulateMode_ != ManipulateMode.PieceDrag) {
            return;
        }

        // 回転中は無視(回転完了後に自動で戻る)
        if(IsRotation_) {
            return;
        }

        UpdateManipulateMode(ManipulateMode.None);
    }

    /// <summary>
    /// ドラッグ開始
    /// </summary>
    /// <param name="drag"></param>
    /// <param name="inside"></param>
	public void OnDragStart(SurfaceController surface, Transform drag, Transform inside) {
        // 回転中は無視
        if(IsRotation_) {
            return;
        }

        // 既にドラッグ・選択中なら無視
        if(ManipulateMode_ != ManipulateMode.None) {
            return;
        }

		DragOrigin_ = drag;
		DragOriginInside_ = inside;
        surface.Select(true);

        UpdateManipulateMode(ManipulateMode.PieceDrag);
        //Debug.LogFormat("OnDragStart : Cubes=[{0}, {1}]",
        //	DragOrigin_.name, DragOriginInside_.name);
    }

    /// <summary>
    /// ドラッグ移動
    /// </summary>
    /// <param name="drag"></param>
    /// <param name="inside"></param>
	public void OnDragOver(SurfaceController surface, Transform drag, Transform inside) {
        // ドラッグモード中でなければ無視
		//if(DragOrigin_ != null || IsRotation_) { 
        if(ManipulateMode_ != ManipulateMode.PieceDrag) { 
            return;
        }

        // 回転中は無視
        if (IsRotation_) {
            return;
        }

        // 無変更
        if (DragOrigin_ == drag) {
			return;
		}

        surface.Select(true);

        // ドラッグで通過したピースを含む平面が一つに特定できるか？
        List<Transform> cubes = new List<Transform> () {
			DragOrigin_, DragOriginInside_
		};
		if (!cubes.Contains (drag)) {
			cubes.Add (drag);
		}
		if (!cubes.Contains (inside)) {
			cubes.Add (inside);
		}

		var plane = DetectRotatePlane(cubes.ToArray());
        if(plane == null) {
            // 一つに特定できない
            return;
        }

		FireRotateBy2Cube (plane[0], plane[1], plane[2],
            DragOrigin_, drag);

        // 回転中は選択面を暗くしておきたいのでここではキャンセルしない。
        // this.OnDragCancel();
    }

    public void OnClickSurface(SurfaceController surface, Transform drag, Transform inside) {
        // 回転中は無視
        if (IsRotation_) {
            return;
        }

        // 既にドラッグ・選択中なら無視
        //if (ManipulateMode_ != ManipulateMode.None) {
        // ドラッグ中なら無視(とりあえず)
        if(ManipulateMode_ == ManipulateMode.PieceDrag) { 
            return;
        }

        // クリック面設定
        ClickOrigin_ = surface.GetComponent<Transform>();
        surface.Select(true);
        UpdateManipulateMode(ManipulateMode.PieceClick);
    }

    private void UpdatePieceRotateMarker() {
        // クリック選択モード
        if(ManipulateMode_ == ManipulateMode.PieceClick) {
            PiiceRotateMarker_.UpdateTransform(ClickOrigin_.GetComponent<SurfaceController>());
        } else { 
            PiiceRotateMarker_.UpdateTransform(null);
        }
    }

    /// <summary>
    /// transforms を全て含む回転可能面が1つだけあれば x,y,z を返す。
    /// </summary>
    /// <param name="transforms"></param>
    /// <returns></returns>
	private int[] DetectRotatePlane(Transform[] transforms) {
        /*
		{
			string names = "";
			foreach (var t in transforms) {
				if (string.IsNullOrEmpty (names)) {
					names = t.name;
				} else {
					names += ", " + t.name;
				}
			}
			Debug.LogFormat ("DetectSurface : Cubes=[{0}]", names);
		}
        */

        // 回転可能平面パラメータリスト
		int[,] filterParams = {
            { 0, -1, -1 },
			{ 1, -1, -1 },
            { 2, -1, -1 },
            {-1,  0, -1 },
			{-1,  1, -1 },
            {-1,  2, -1 },
            {-1, -1,  0 },
			{-1, -1,  1 },
            {-1, -1,  2 },
        };

        int[] planeParam = null;

        for(int i = 0; i < 9 ; i++) {
            var s = FilterPieces(filterParams[i, 0], filterParams[i, 1], filterParams[i, 2]);

            // 回転面が transforms を全て含んでいる？
            if(ContainsAll(s, transforms)) {
                if(planeParam != null) {
                    // 2面目が見つかってしまった。
                    return null;
                }

                //Debug.LogFormat("DetectSurface : Found ({0}, {1}, {2})",
                //    filterParams[i, 0], filterParams[i, 1], filterParams[i, 2]);
				planeParam = new int[] {
					filterParams [i, 0], filterParams [i, 1], filterParams [i, 2]
				};
            }
        }
        return planeParam;
    }

}
