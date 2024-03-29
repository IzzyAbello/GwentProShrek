using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    public bool isSelected = false;
    public GameObject dropZone;
    
    
    public void OnClickReturnToHand ()
    {
        dropZone = gameObject.transform.parent.gameObject;
        DropZoneCards dropZoneCards = dropZone.GetComponent<DropZoneCards>();
        DisplayCard card = GetComponent<DisplayCard>();

        if (dropZoneCards != null)
        {
            if (dropZoneCards.isDecoy && card.cardKind != 'g')
            {
                isSelected = true;

                GameObject hand;
                GameObject cardToRemove = dropZoneCards.GetSpecifiedCardDecoy();

                int cardIndex = dropZoneCards.GetSpecifiedCard(cardToRemove.GetComponent<DisplayCard>().cardId);


                if (GetComponent<DisplayCard>().cardFaction == 0)
                    hand = GameObject.Find("HandShrek");
                else
                    hand = GameObject.Find("HandBad");

                cardToRemove.GetComponent<DisplayCard>().CardReset();


                Debug.Log($"Card removed from Drop Zone: {cardToRemove.name}");
                Debug.Log($"Card added to Hand: {cardToRemove.name}");

                dropZoneCards.cardsDropZone.RemoveAt(cardIndex);
                cardToRemove.transform.SetParent(hand.transform, true);

                dropZoneCards.isDecoy = false;

                GameObject turnSwitch = GameObject.Find("PlayTurnButton");

                if (!hand.GetComponent<Hand>().isPass)
                    turnSwitch.GetComponent<SwitchTurn>().SwitchTurnPlayer(hand);
            }
        }
    }
}
