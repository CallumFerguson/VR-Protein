using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class HandManager : MonoBehaviour
{
    public LeapServiceProvider provider;
    public GameObject moveGameObject;
    float moveGameObjectScale = 1;

    float lastPinchDistance = -1;
    Vector3 lastMidPoint = Vector3.zero;
    Vector3 lastAngle = Vector3.zero;

    void Start()
    {

    }

    void Update()
    {
        Frame frame = provider.CurrentFrame;
        var hands = frame.Hands;

        if (hands.Count == 2)
        {
            if (hands[0].PinchStrength == 1 && hands[1].PinchStrength == 1)
            {
                float pinchDistance = Vector3.Distance(hands[0].GetPinchPosition(), hands[1].GetPinchPosition());
                Vector3 midPoint = (hands[0].GetPinchPosition() + hands[1].GetPinchPosition()) / 2f;

                Vector3 leftHandPos;
                Vector3 rightHandPos;
                if(hands[0].IsLeft)
                {
                    leftHandPos = hands[0].GetPinchPosition();
                    rightHandPos = hands[1].GetPinchPosition();
                }
                else
                {
                    leftHandPos = hands[1].GetPinchPosition();
                    rightHandPos = hands[0].GetPinchPosition();
                }
                rightHandPos -= leftHandPos;

                Vector3 angle = Vector3.zero;
                //angle.x = -Mathf.Atan2(rightHandPos.y, rightHandPos.z) * Mathf.Rad2Deg;
                angle.y = -Mathf.Atan2(rightHandPos.z, rightHandPos.x) * Mathf.Rad2Deg;
                angle.z = Mathf.Atan2(rightHandPos.y, rightHandPos.x) * Mathf.Rad2Deg;

                if (lastPinchDistance >= 0)
                {
                    float pintchDistanceDifference = pinchDistance - lastPinchDistance;
                    moveGameObjectScale += pintchDistanceDifference * 3.25f;
                    moveGameObjectScale = Mathf.Clamp(moveGameObjectScale, 0.1f, 5f);
                    moveGameObject.transform.localScale = new Vector3(moveGameObjectScale, moveGameObjectScale, moveGameObjectScale);

                    Vector3 midPointDifference = midPoint - lastMidPoint;
                    moveGameObject.transform.position += midPointDifference;

                    Vector3 angleDifference = angle - lastAngle;
                    //moveGameObject.transform.eulerAngles += angleDifference;
                    moveGameObject.transform.Rotate(angleDifference, Space.World);
                    //moveGameObject.transform.rotation = hands[0].Rotation.ToQuaternion();
                }

                lastPinchDistance = pinchDistance;
                lastMidPoint = midPoint;
                lastAngle = angle;

                Debug.DrawLine(hands[0].GetPinchPosition(), hands[1].GetPinchPosition(), Color.red);
            }
            else
            {
                lastPinchDistance = -1;
            }
            
        }
    }
}