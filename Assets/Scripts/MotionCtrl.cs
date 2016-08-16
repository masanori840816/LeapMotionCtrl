using Leap;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MotionCtrl : MonoBehaviour {
    public List<GameObject> motionCtrlObjectList;

    private List<IMotionCallback> motionCtrlList;
    private int motionCtrlCount;

    private Controller leapCtrl;
    private Frame leapFrame;
    private List<Hand> handList;

    private bool isTracking;
    private bool isGrabbed;
    private bool lastTrackingStatus;
    private bool lastGrabbedStatus;
    private Vector3 convertedValue = new Vector3(0, 0, 0);

    private Vector3 handPosition;
    public Vector3 HandPosition
    {
        get { return handPosition; }
    }
	private void Start (){
        // Trackingと手の状態変化を返す対象のClass.
        motionCtrlList = motionCtrlObjectList.Select(ctrlObject => ctrlObject.GetComponent<IMotionCallback>()).ToList();
        motionCtrlCount = motionCtrlList.Count - 1;

        // App起動時にLeapMotionに接続.
        leapCtrl = new Controller();
        leapCtrl.StartConnection();

        isTracking = false;
        isGrabbed = false;

        lastTrackingStatus = false;
        lastGrabbedStatus = false;
    }
	
	private void Update () {
        leapFrame = leapCtrl.Frame();
        handList = leapFrame.Hands;

        isTracking = (handList.Count > 0);

        if(isTracking != lastTrackingStatus)
        {
            // Trackingの開始・停止をCallbackで通知.
            if (isTracking)
            {
                for (var i = motionCtrlCount; i >= 0; i--)
                {
                    motionCtrlList[i].OnTrackingStarted();
                }
            }
            else {
                for (var i = motionCtrlCount; i >= 0; i--)
                {
                    motionCtrlList[i].OnTrackingStopped();
                }
                // 手の位置をリセット.
                handPosition.x = 0f;
                handPosition.y = 0f;
                handPosition.z = 0f;
            }
            lastTrackingStatus = isTracking;
        }

        if (! isTracking)
        {
            // Tracking中でなければ手の位置などは取得しない.
            return;
        }
        // Tracking中なら手(Hand0)の平の座標値を取得する.
        handPosition = ConvertToUnityVector3(handList[0].PalmPosition);

        // 手を握っているか(誤検知を考慮して開いている指の本数が1以下ならTrue).
        isGrabbed = (handList[0].Fingers.Where(finger => finger.IsExtended).Count() <= 1);
        if(isGrabbed != lastGrabbedStatus)
        {
            if (isGrabbed)
            {
                for (var i = motionCtrlCount; i >= 0; i--)
                {
                    motionCtrlList[i].OnHandGrabbed();
                }
            }
            else
            {
                for (var i = motionCtrlCount; i >= 0; i--)
                {
                    motionCtrlList[i].OnHandReleased();
                }
            }
            lastGrabbedStatus = isGrabbed;
        }
    }
    private void OnApplicationQuit()
    {
        // Appの終了時は切断する.
        leapCtrl.StopConnection();
        leapCtrl.Dispose();
    }
    private Vector3 ConvertToUnityVector3(Vector originalValue)
    {
        convertedValue.x = originalValue.x;
        convertedValue.y = originalValue.y;
        convertedValue.z = originalValue.z;
        return convertedValue;
    }
}
