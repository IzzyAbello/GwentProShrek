using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTurn : MonoBehaviour
{
    bool shrekStarts = true;

    public GameObject handShrek;
    public GameObject handBad;
    public GameObject leaderShrek;
    public GameObject leaderBad;
    public GameObject pointsShrek;
    public GameObject pointsBad;

    Vector2 playPositionHand;
    Vector2 outSidePositionHand;

    Vector2 playPositionLeader;
    Vector2 outSidePositionLeader;

    public int turn = 1;

    private void Start()
    {
        float xx = handShrek.transform.position.x;
        float yy1 = handShrek.transform.position.y;
        float yy2 = handBad.transform.position.y;

        float xxx1 = leaderShrek.transform.position.x;
        float xxx2 = leaderBad.transform.position.x;
        float yyy = leaderShrek.transform.position.y;

        playPositionHand = new Vector2(xx, yy1);
        outSidePositionHand = new Vector2 (xx, yy2);
        playPositionLeader = new Vector2 (xxx1, yyy);
        outSidePositionLeader = new Vector2 (xxx2, yyy);
    }




    public void OnClickSwitchTurn ()
    {
        if (turn % 2 == 0)
        {
            GameObject liveToRemove;
            GameObject clear = GameObject.Find("Clear");

            ClearAllField clearAll = clear.GetComponent<ClearAllField>();

            int turnPointsShrek = pointsShrek.GetComponent<GetPoints>().points;
            int turnPointsBad = pointsBad.GetComponent<GetPoints>().points;


            if (turnPointsShrek > turnPointsBad)
            {
                Debug.Log("Shrek wins the round...");

                liveToRemove = GameObject.Find("LiveBorderBad");

                foreach (Transform child in liveToRemove.transform)
                {
                    Destroy(child.gameObject);
                    break;
                }

                shrekStarts = true;
            }
            else if (turnPointsShrek < turnPointsBad)
            {
                Debug.Log("Farquaad wins the round...");

                liveToRemove = GameObject.Find("LiveBorderShrek");

                foreach (Transform child in liveToRemove.transform)
                {
                    Destroy(child.gameObject);
                    break;
                }

                shrekStarts = false;
            }
            else
            {
                Debug.Log("Draw...");

                liveToRemove = GameObject.Find("LiveBorderBad");

                foreach (Transform child in liveToRemove.transform)
                {
                    Destroy(child.gameObject);
                    break;
                }

                liveToRemove = GameObject.Find("LiveBorderShrek");

                foreach (Transform child in liveToRemove.transform)
                {
                    Destroy(child.gameObject);
                    break;
                }

                turn += 2;
            }


            clearAll.Clear();
        }

        if (turn <= 5)
        {
            if (shrekStarts && turn != 1)
            {
                handShrek.transform.position = playPositionHand;
                leaderShrek.transform.position = playPositionLeader;

                handBad.transform.position = outSidePositionHand;
                leaderBad.transform.position = outSidePositionLeader;
                
                shrekStarts = false;
            }
            else
            {
                handBad.transform.position = playPositionHand;
                leaderBad.transform.position = playPositionLeader;

                handShrek.transform.position = outSidePositionHand;
                leaderShrek.transform.position = outSidePositionLeader;

                shrekStarts = true;
            }


            turn++;
        }
    }
}
