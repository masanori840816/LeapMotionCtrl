/// <summary>
/// MotionCtrlから取得した手の座標値を元にSphereを移動させる.
/// </summary>
using UnityEngine;
using System.Collections;

public class MotionPointerCtrl : MonoBehaviour, IMotionCallback{
    public GameObject motionCtrlObject;

    public GameObject targetObject;

    private MotionCtrl motionCtrl;
    private Vector3 targetPosition;
    private bool isCtrlDisabled;

    public void OnTrackingStarted()
    {
        isCtrlDisabled = false;
        targetObject.SetActive(true);
    }
    public void OnTrackingStopped()
    {
        targetObject.SetActive(false);
        isCtrlDisabled = true;
    }
    public void OnHandGrabbed()
    {
        // 何もしない.
    }
    public void OnHandReleased()
    {
        // 何もしない.
    }
    private void Start()
    {
        motionCtrl = motionCtrlObject.GetComponent<MotionCtrl>();
        isCtrlDisabled = true;
        targetObject.SetActive(false);
    }
	private void Update()
    {
        if (isCtrlDisabled)
        {
            return;
        }
        targetPosition = motionCtrl.HandPosition;
        targetPosition.z = -targetPosition.z;
        targetObject.transform.localPosition = targetPosition;
	}
}
