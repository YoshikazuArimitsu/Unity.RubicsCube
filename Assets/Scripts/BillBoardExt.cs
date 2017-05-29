using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PivotAxis {
    // Rotate about all axes.
    Free,
    // Rotate about an individual axis.
    Y
}

/// <summary>
/// The Billboard class implements the behaviors needed to keep a GameObject oriented towards the user.
/// </summary>
public class BillBoardExt : MonoBehaviour {
    /// <summary>
    /// The axis about which the object will rotate.
    /// </summary>
    [Tooltip("Specifies the axis about which the object will rotate.")]
    public PivotAxis PivotAxis = PivotAxis.Free;

    public GameObject PivotObject;
    private float PivotObjectAngle;

    /// <summary>
    /// ピボット対象から自オブジェクトとカメラに対するXZ平面の角度を得る
    /// </summary>
    /// <returns></returns>
    private float calcPivotObjectAngle() {
        Vector3 cameraVec = Camera.main.transform.position - PivotObject.transform.position;
        Vector3 meVec = transform.position - PivotObject.transform.position;

        Vector3 cameraVecXZ = new Vector3(cameraVec.x, 0, cameraVec.z);
        Vector3 meVecXZ = new Vector3(meVec.x, 0, meVec.z);

        // http://answers.unity3d.com/questions/181867/is-there-way-to-find-a-negative-angle.html
        float angle = Vector3.Angle(meVecXZ, cameraVecXZ);
        Vector3 cross = Vector3.Cross(meVecXZ, cameraVecXZ);
        if (cross.y < 0) angle = -angle;
        return angle;

//      return Vector3.Angle(meVecXZ, cameraVecXZ);
    }

    private void OnEnable() {
        PivotObjectAngle = calcPivotObjectAngle();

        Update();
    }

    /// <summary>
    /// Keeps the object facing the camera.
    /// </summary>
    private void Update() {
        if (!Camera.main) {
            return;
        }

        // ピボット対象から自オブジェクトとカメラに対する角度を計算し、
        // 初期角度を保つようにピボット対象の周りを回転する。
        {
            var angle = calcPivotObjectAngle();
            //Debug.LogFormat("{0} : {1}", transform.name, PivotObjectAngle - angle);

            transform.RotateAround(
                PivotObject.transform.position,
                Vector3.up,
                angle - PivotObjectAngle);
        }

        // 通常ビルボードの実装部分
        // Get a Vector that points from the target to the main camera.
        Vector3 directionToTarget = Camera.main.transform.position - transform.position;

        // Adjust for the pivot axis.
        switch (PivotAxis) {
            case PivotAxis.Y:
                directionToTarget.y = 0.0f;
                break;

            case PivotAxis.Free:
            default:
                // No changes needed.
                break;
        }

        // If we are right next to the camera the rotation is undefined. 
        if (directionToTarget.sqrMagnitude < 0.001f) {
            return;
        }

        // Calculate and apply the rotation required to reorient the object
        transform.rotation = Quaternion.LookRotation(-directionToTarget);
    }
}
