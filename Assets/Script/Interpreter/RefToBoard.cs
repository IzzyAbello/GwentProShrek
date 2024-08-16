using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefToBoard : MonoBehaviour
{
    // All field areas (ref)
    public GameObject shrekHandRef; // INSPECTOR
    public GameObject badHandRef;

    public GameObject shrekSiegeRef;
    public GameObject badSiegeRef;

    public GameObject shrekRangeRef;
    public GameObject badRangeRef;

    public GameObject shrekMeleeRef;
    public GameObject badMeleeRef;

    public GameObject shrekGraveyardRef;
    public GameObject badGraveyardRef;

    public GameObject shrekDeckRef;
    public GameObject badDeckRef;

    public GameObject climateZoneRef;

    public GameObject abyss;

    // All fieldStruct areas
    public FieldStruct shrekHand;
    public FieldStruct badHand;

    public FieldStruct shrekSiege;
    public FieldStruct badSiege;

    public FieldStruct shrekRange;
    public FieldStruct badRange;

    public FieldStruct shrekMelee;
    public FieldStruct badMelee;

    public FieldStruct shrekGraveyard;
    public FieldStruct badGraveyard;

    public FieldStruct shrekDeck;
    public FieldStruct badDeck;

    public FieldStruct climateZone;

    // String for name of factions
    public string shrekFactionString = "Shrek";
    public string badFactionString = "Lord Farquaad";

    // Context of all Board
    public ContextStruct shrekFaction;
    public ContextStruct badFaction;
    public ContextStruct allBoard;

    public void ResetBoard()
    {
        allBoard = new ContextStruct();
        shrekFaction = new ContextStruct();
        badFaction = new ContextStruct();

        shrekHand = new FieldStruct(shrekHandRef);
        badHand = new FieldStruct(badHandRef);
        shrekFaction.Add(shrekHand);
        badFaction.Add(badHand);
        allBoard.Add(shrekHand);
        allBoard.Add(badHand);

        shrekSiege = new FieldStruct(shrekSiegeRef);
        badSiege = new FieldStruct(shrekSiegeRef);
        shrekFaction.Add(shrekSiege);
        badFaction.Add(badSiege);
        allBoard.Add(shrekSiege);
        allBoard.Add(badSiege);

        shrekRange = new FieldStruct(shrekRangeRef);
        badRange = new FieldStruct(badRangeRef);
        shrekFaction.Add(shrekRange);
        badFaction.Add(badRange);
        allBoard.Add(shrekRange);
        allBoard.Add(badRange);

        shrekMelee = new FieldStruct(shrekMeleeRef);
        badMelee = new FieldStruct(badMeleeRef);
        shrekFaction.Add(shrekMelee);
        badFaction.Add(badMelee);
        allBoard.Add(shrekMelee);
        allBoard.Add(badMelee);

        shrekDeck = new FieldStruct(shrekDeckRef);
        badDeck = new FieldStruct(badDeckRef);
        shrekFaction.Add(shrekDeck);
        badFaction.Add(badDeck);
        allBoard.Add(shrekDeck);
        allBoard.Add(badDeck);

        shrekGraveyard = new FieldStruct(shrekGraveyardRef);
        badGraveyard = new FieldStruct(badGraveyardRef);
        shrekFaction.Add(shrekGraveyard);
        badFaction.Add(badGraveyard);
        allBoard.Add(shrekGraveyard);
        allBoard.Add(badGraveyard);

        climateZone = new FieldStruct(climateZoneRef);
        allBoard.Add(climateZone);
    }

    private void Start()
    {
        StartCoroutine(StartDalayedCoroutine());
    }

    IEnumerator StartDalayedCoroutine()
    {
        yield return new WaitForEndOfFrame();
        ResetBoard();
    }

    public ContextStruct FillAllBoard()
    {
        ContextStruct context = new ContextStruct();
        context.Add(shrekHand); context.Add(badHand); context.Add(shrekSiege);
        context.Add(badSiege); context.Add(shrekRange); context.Add(badRange);
        context.Add(shrekMelee); context.Add(badMelee); context.Add(shrekDeck);
        context.Add(badDeck); context.Add(shrekGraveyard); context.Add(badGraveyard);
        context.Add(climateZone);

        return context;
    }

    public FieldStruct SetField(FieldStruct field, FieldStruct target, bool single = false)
    {
        ContextStruct toAdd = (target == shrekDeck || target == shrekHand || target == shrekSiege
            || target == shrekRange || target == shrekMelee || target == shrekGraveyard) ?
            shrekFaction : badFaction;

        target = FilterCards(field, target);
        if (single) target = SingleFilter(target);
        toAdd.Add(target);
        allBoard.Add(target);

        return target;
    }
    public void AfterPredicateFilter(FieldStruct field, bool single = false)
    {
        allBoard = new ContextStruct();
        shrekFaction = new ContextStruct();
        badFaction = new ContextStruct();

        shrekDeck = SetField(field, shrekDeck, single);
        badDeck = SetField(field, badDeck, single);

        shrekHand = SetField(field, shrekHand, single);
        badHand = SetField(field, badHand, single);

        shrekSiege = SetField(field, shrekSiege, single);
        badSiege = SetField(field, badSiege, single);

        shrekRange = SetField(field, shrekRange, single);
        badRange = SetField(field, badRange, single);

        shrekMelee = SetField(field, shrekMelee, single);
        badMelee = SetField(field, badMelee, single);

        shrekGraveyard = SetField(field, shrekGraveyard, single);
        badGraveyard = SetField(field, badGraveyard, single);

        climateZone = SetField(field, climateZone, single);
    }
    public FieldStruct FilterCards(FieldStruct toGet, FieldStruct target)
    {
        FieldStruct toReturn = new FieldStruct();
        foreach (CardStruct card in target.cardList)
            if (toGet.Contains(card)) toReturn.Add(card);
        return toReturn;
    }
    public FieldStruct SingleFilter(FieldStruct target)
    {
        if (target.cardList.Count > 0)
            while (target.cardList.Count != 1)
                target.cardList.RemoveAt(1);
        return target;
    }
}
