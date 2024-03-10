using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanPlayCard : MonoBehaviour
{
    public bool CanPlaySpecificCard (GameObject dropZone)
    {
        DisplayCard card = gameObject.GetComponent<DisplayCard>();
        DropZoneConditions conditions = dropZone.GetComponent<DropZoneConditions>();
        if (conditions.faction == card.cardFaction && conditions.zone == card.cardZone)
            return true;
        else
            return false;
    }
}
