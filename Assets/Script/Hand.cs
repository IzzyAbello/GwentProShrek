using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    int changeCards = 0;

    public bool isFirstRound = true;

    public bool canChangeCard = true;

    public List<GameObject> hand = new List<GameObject>();

    public GameObject cardInHand;
    public GameObject playerHand;

    public Deck deck;
    public Graveyard graveyard;


    public void RemoveFromHand(GameObject cardToRemove)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            DisplayCard getIDtoRemove = cardToRemove.GetComponent<DisplayCard>();
            DisplayCard getIDonHand = hand[i].GetComponent<DisplayCard>();

            if (getIDonHand.displayCard.cardId == getIDtoRemove.displayCard.cardId)
            {

                graveyard.cardsGraveyard.Add(hand[i]);
                hand.RemoveAt(i);
            }
        }

        foreach (Transform child in transform)
        {
            if (child.GetComponent<DisplayCard>().cardId == cardToRemove.GetComponent<DisplayCard>().cardId)
            {
                changeCards++;
                Destroy(child.gameObject);
                break;
            }
        }
        if (changeCards == 2)
        {
            GameObject button = GameObject.Find("StartGameButton");
            button.GetComponent<StartButton>().OnClickStart();
            isFirstRound = false;
        }
    }

    public void OnClickTakeFromDeck(int n = 1)
    {
        int size = hand.Count;

        for (int i = size; i < size + n; i++)
        {
            hand.Add(deck.deck[0]);
            Debug.Log($"Card added to hand: {hand[hand.Count - 1].name}");
            deck.RemoveFromDeck(1);
            GameObject playerCard = Instantiate(hand[i], new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(playerHand.transform, false);
        }

        Debug.Log("Cards in hand: ");
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].GetComponent<DisplayCard>().CardReset();
            Debug.Log($"Card {i}: {hand[i].gameObject.name}");
        }

    }

    private void Start()
    {
        OnClickTakeFromDeck(10);
    }
}
