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

    void Update()
    {
        Frame frame = provider.CurrentFrame;
        var hands = frame.Hands;

        if (hands.Count == 2 && hands[0].IsPinching() && hands[1].IsPinching())
        {
            float pinchDistance = Vector3.Distance(hands[0].GetPinchPosition(), hands[1].GetPinchPosition());
            Vector3 midPoint = (hands[0].GetPinchPosition() + hands[1].GetPinchPosition()) / 2f;

            if (lastPinchDistance >= 0)
            {
                float pintchDistanceDifference = pinchDistance - lastPinchDistance;
                moveGameObjectScale += pintchDistanceDifference * 3.25f;
                moveGameObjectScale = Mathf.Clamp(moveGameObjectScale, 0.1f, 5f);
                moveGameObject.transform.localScale = new Vector3(moveGameObjectScale, moveGameObjectScale, moveGameObjectScale);

                Vector3 midPointDifference = midPoint - lastMidPoint;
                moveGameObject.transform.position += midPointDifference;
            }

            lastPinchDistance = pinchDistance;
            lastMidPoint = midPoint;

            //Debug.DrawLine(hands[0].GetPinchPosition(), hands[1].GetPinchPosition(), Color.red);
        }
        else
        {
            lastPinchDistance = -1;
            foreach (Hand hand in hands)
            {
                if (hand.GetFistStrength() >= 0.8f)
                {
                    float speed = 180f;
                    if (hand.IsLeft)
                        moveGameObject.transform.Rotate(new Vector3(0, -Time.deltaTime * speed, 0), Space.World);
                    else
                        moveGameObject.transform.Rotate(new Vector3(0, Time.deltaTime * speed, 0), Space.World);
                }
            }
        }
    }
}