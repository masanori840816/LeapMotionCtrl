﻿/// <summary>
/// MotionCtrlから取得した手の情報を使って縦横のSwipeジェスチャーを検出する.
/// </summary>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MotionSwipeCtrl : MonoBehaviour, IMotionCallback
{
    public GameObject motionCtrlObject;

    private readonly int MovedLengthCount = 3;
    // 
    private readonly float TargetDirectionPerFrameMoveLength = 25.0f;
    private readonly float OppositeDirectionPerFrameMoveLength = 5.0f;
    private readonly Vector2 DefaultMoveLength = new Vector2(0.0f, 0.0f);

    private MotionCtrl motionCtrl;
    private bool isCtrlDisabled;
    // X, Y方向のSwipeのみを取るためVector2を使う.
    private List<Vector2> movedLengthList;
    private float movedTotalLengthX;
    private float movedTotalLengthY;

    private Vector3 lastHandPosition;
    private Vector2 newMovedLength;

    private float validMinMoveLength;
    private float invalidMaxOppositeLength;

    private bool isGestureExecuted;

    public void OnTrackingStarted()
    {
        lastHandPosition = motionCtrl.HandPosition;
        isCtrlDisabled = false;
        isGestureExecuted = false;
    }
    public void OnTrackingStopped()
    {
        isCtrlDisabled = true;
        Reset();
    }
    public void OnHandGrabbed()
    {
        isCtrlDisabled = true;
    }
    public void OnHandReleased()
    {
        isCtrlDisabled = false;
    }
    private void Start(){
        motionCtrl = motionCtrlObject.GetComponent<MotionCtrl>();
        movedLengthList = new List<Vector2>();
        validMinMoveLength = 0f;
        invalidMaxOppositeLength = 0f;
        // 初期化.
        for (var i = MovedLengthCount; i >= 0; i--)
        {
            movedLengthList.Add(DefaultMoveLength);

            validMinMoveLength += TargetDirectionPerFrameMoveLength;
            invalidMaxOppositeLength += OppositeDirectionPerFrameMoveLength;
        }
        isCtrlDisabled = true;
        isGestureExecuted = false;
    }
    private void Update () {
        if (isCtrlDisabled)
        {
            return;
        }
        newMovedLength.x = motionCtrl.HandPosition.x - lastHandPosition.x;
        newMovedLength.y = motionCtrl.HandPosition.y - lastHandPosition.y;

        if (! isGestureExecuted)
        {
            // 前数Frame分の移動距離を算出.
            movedTotalLengthX = Math.Abs(movedLengthList.Select(movedLength => movedLength.x).Sum() + newMovedLength.x);
            movedTotalLengthY = Math.Abs(movedLengthList.Select(movedLength => movedLength.y).Sum() + newMovedLength.y);

            if (movedTotalLengthX >= movedTotalLengthY)
            {
                if (movedTotalLengthX >= validMinMoveLength
                    && movedTotalLengthY <= invalidMaxOppositeLength)
                {
                    Debug.Log("Swipe X Dir");
                    isGestureExecuted = true;
                    StopDetectingGesture();
                }
            }
            else
            {
                if (movedTotalLengthY >= validMinMoveLength
                    && movedTotalLengthX <= invalidMaxOppositeLength)
                {
                    Debug.Log("Swipe Y Dir");
                    isGestureExecuted = true;
                    StopDetectingGesture();
                }
            }
        }
        
        // 値を更新して今Frameの座標値の差分を保存する.
        for (var i = MovedLengthCount; i >= 1; i--)
        {
            movedLengthList[i] = movedLengthList[i - 1];
        }
        movedLengthList[0] = newMovedLength;
        // 今Frameの座標値を保存する.
        lastHandPosition = motionCtrl.HandPosition;
    }
    private void Reset()
    {
        for (var i = MovedLengthCount; i >= 0; i--)
        {
            movedLengthList[i] = DefaultMoveLength;
        }
        isGestureExecuted = false;
    }
    private IEnumerator StopDetectingGesture()
    {
        // Swipe実行後は一定時間Swipe操作を無効にする.
        yield return new WaitForSeconds(0.5f);
        Reset();
    }
}
