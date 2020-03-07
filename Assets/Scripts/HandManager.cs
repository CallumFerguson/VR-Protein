using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class HandManager : MonoBehaviour
{
    public LineRenderer pointer;

    public LeapServiceProvider provider;
    public GameObject moveGameObject;
    float moveGameObjectScale = 1;

    float lastPinchDistance = -1;
    Vector3 lastMidPoint = Vector3.zero;

    public GameObject aot;
    public GameObject aoz;

    public UnityEngine.UI.Image aotUI;
    public UnityEngine.UI.Image aozUI;
    public UnityEngine.UI.Image toggleUI;

    Color grey = new Color(0.2f, 0.2f, 0.2f, 0.85f);
    Color grey2 = new Color(0.35f, 0.35f, 0.35f, 0.85f);

    public GameObject[] models;

    bool aotActive = true;

    bool ribbonView = true;
    float lastToggleTime = 0;

    private void Start()
    {
        pointer.startWidth = 0.01f;
        pointer.endWidth = 0.01f;

        aotUI.color = Color.green;
    }

    void Update()
    {
        pointer.gameObject.SetActive(false);
        if(!aotActive)
            aotUI.color = grey;

        if(aotActive)
            aozUI.color = grey;

        toggleUI.color = grey;

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
                moveGameObjectScale = Mathf.Clamp(moveGameObjectScale, 0.1f, 7.5f);
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
                //Debug.DrawRay(hand.PalmPosition.ToVector3(), hand.Direction.ToVector3());

                if (hand.GetFistStrength() >= 0.8f)
                {
                    float speed = 180f;
                    if (hand.IsLeft)
                        moveGameObject.transform.Rotate(new Vector3(0, -Time.deltaTime * speed, 0), Space.World);
                    else
                        moveGameObject.transform.Rotate(new Vector3(0, Time.deltaTime * speed, 0), Space.World);
                }
                else
                {
                    if (hand.IsRight)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(hand.PalmPosition.ToVector3(), hand.Direction.ToVector3(), out hit))
                        {
                            if (hit.collider.CompareTag("Panel"))
                            {
                                pointer.gameObject.SetActive(true);
                                pointer.gameObject.transform.position = hit.point;
                                pointer.SetPosition(0, hand.PalmPosition.ToVector3());
                                pointer.SetPosition(1, hit.point);

                                if (hit.collider.name == "5aot" && !aotActive)
                                {
                                    aotUI.color = grey2;
                                }
                                if (hit.collider.name == "5aoz" && aotActive)
                                {
                                    aozUI.color = grey2;
                                }
                                if(hit.collider.name == "toggle")
                                {
                                    toggleUI.color = grey2;
                                }

                                if (hand.IsPinching())
                                {
                                    if (hit.collider.name == "5aot")
                                    {
                                        aotActive = true;
                                        aot.SetActive(true);
                                        aoz.SetActive(false);
                                        aotUI.color = Color.green;
                                    }
                                    if (hit.collider.name == "5aoz")
                                    {
                                        aotActive = false;
                                        aot.SetActive(false);
                                        aoz.SetActive(true);
                                        aozUI.color = Color.green;
                                    }
                                    if(hit.collider.name == "toggle")
                                    {
                                        toggleUI.color = Color.green;

                                        if(Time.time - lastToggleTime >= 1f)
                                        {
                                            ribbonView = !ribbonView;
                                            if(ribbonView)
                                            {
                                                models[0].SetActive(true);
                                                models[1].SetActive(false);
                                                models[2].SetActive(true);
                                                models[3].SetActive(false);
                                            }
                                            else
                                            {
                                                models[0].SetActive(false);
                                                models[1].SetActive(true);
                                                models[2].SetActive(false);
                                                models[3].SetActive(true);
                                            }
                                            lastToggleTime = Time.time;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}