using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCards : MonoBehaviour
{
    public GameObject hand;
    public Hand handCards;
    private AudioManager audioM;

    private void Start()
    {
        hand = gameObject.transform.parent.gameObject;
        handCards = hand.GetComponent<Hand>();

        audioM = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void OnClickReturnToHand()
    {
        if (handCards.isFirstRound)
        {
            handCards.RemoveFromHand(gameObject);
            handCards.OnClickTakeFromDeck();

            audioM.PlaySound(audioM.drawCardAudio);
        }
    }
}
