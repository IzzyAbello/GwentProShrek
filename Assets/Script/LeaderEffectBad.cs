using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderEffectBad : MonoBehaviour
{
    public bool wasPlayed = false;

    public void OnClickPlayLeaderEffect ()
    {
        if (!wasPlayed)
        {
            wasPlayed = true;
            GameObject hand = GameObject.Find("HandBad");
            Hand inHand = hand.GetComponent<Hand>();
            inHand.OnClickTakeFromDeck();

            if (!inHand.isPass)
            {
                GameObject turnSwitch = GameObject.Find("PlayTurnButton");
                turnSwitch.GetComponent<SwitchTurn>().SwitchTurnPlayer(hand);
            }
        }
    }
}
