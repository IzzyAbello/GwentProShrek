using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCards : MonoBehaviour
{
    public GameObject hand;
    public Hand handCards;

    private void Start()
    {
        hand = gameObject.transform.parent.gameObject;
        handCards = hand.GetComponent<Hand>();
    }

    public void OnClickReturnToHand()
    {
        if (handCards.isFirstRound)
        {
            handCards.RemoveFromHand(gameObject);
            handCards.OnClickTakeFromDeck();
        }
    }
}
