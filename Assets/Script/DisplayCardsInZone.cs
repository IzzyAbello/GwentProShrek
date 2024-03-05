using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCardsInZone : MonoBehaviour
{
    public static List<GameObject> hand = new List<GameObject>();

    public GameObject cardInHand;
    public GameObject playerHand;

    static int size = hand.Count;

    public static void RemoveFromHand(GameObject cardToRemove)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            DisplayCard getIDtoRemove = cardToRemove.GetComponent<DisplayCard>();
            DisplayCard getIDonHand = hand[i].GetComponent<DisplayCard>();

            if (getIDonHand.displayCard.cardId == getIDtoRemove.displayCard.cardId)
            {
                
                Graveyard.cardsGraveyard.Add(hand[i]);
                hand.RemoveAt(i);
                size--;
            }
        }
            
    }


    public void OnClick()
    {
        int n = 2;
        for (int i = size; i < size + n; i++)
        {
            hand.Add(Deck.deck[0]);
            Debug.Log($"Card added to hand: {hand[hand.Count - 1].name}");
            Deck.RemoveFromDeck(1);
            GameObject playerCard = Instantiate(hand[i], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(playerHand.transform, false);
        }
        size = hand.Count;

        Debug.Log("Cards in hand: ");
        for (int i = 0; i < hand.Count; i++)
            Debug.Log($"Card {i}: {hand[i].gameObject.name}");
    }
}
