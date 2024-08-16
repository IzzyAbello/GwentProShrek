using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope<T>
{
    public Dictionary<string, T> LOCAL_SCOPE;
    public Scope<T> GLOBAL_SCOPE;

    public Scope()
    {
        GLOBAL_SCOPE = null;
        LOCAL_SCOPE = new Dictionary<string, T>();
    }
    public Scope(Scope<T> GLOBAL_SCOPE)
    {
        this.GLOBAL_SCOPE = GLOBAL_SCOPE;
        LOCAL_SCOPE = new Dictionary<string, T>();
    }


    public bool IsInScope(string name)
    {
        if (GLOBAL_SCOPE == null) return LOCAL_SCOPE.ContainsKey(name);
        else if (LOCAL_SCOPE.ContainsKey(name)) return true;
        else return GLOBAL_SCOPE.IsInScope(name);
    }
    public bool IsInScope(Name name)
    {
        return IsInScope(name.name);
    }
    public bool IsInScope(Var variable)
    {
        return IsInScope(variable.value);
    }
    public bool IsInScope(EffectNode effect)
    {
        return IsInScope(effect.name.name);
    }
    public bool IsInScope(CardNode card)
    {
        return IsInScope(card.name.name);
    }


    public T Get(string name)
    {
        if (GLOBAL_SCOPE == null)
        {
            if (LOCAL_SCOPE.ContainsKey(name)) return LOCAL_SCOPE[name];
            else
            {
                Debug.Log($"'{name}' not found");
                return default;
            }
        }
        else if (LOCAL_SCOPE.ContainsKey(name)) return LOCAL_SCOPE[name];
        else return GLOBAL_SCOPE.Get(name);
    }
    public T Get(Name name)
    {
        return Get(name.name);
    }
    public T Get(Var variable)
    {
        return Get(variable.value);
    }
    public T Get(EffectNode effect)
    {
        return Get(effect.name.name);
    }
    public T Get(CardNode card)
    {
        return Get(card.name.name);
    }


    public void Set(string name, T value)
    {
        if (!IsInScope(name) || LOCAL_SCOPE.ContainsKey(name)) LOCAL_SCOPE[name] = value;
        else GLOBAL_SCOPE.Set(name, value);
    }
    public void Set(Name name, T value)
    {
        Set(name.name, value);
    }
    public void Set(Var variable, T value)
    {
        Set(variable.value, value);
    }
    public void Set(EffectNode effect, T value)
    {
        Set(effect.name.name, value);
    }
    public void Set(CardNode card, T value)
    {
        Set(card.name.name, value);
    }
}

public abstract class InterpreterStruct
{
    public RefToBoard refToBoard;
    public abstract object Acces(object key);

    public abstract object SetAcces(object key, object value, bool isLast = false);
}

public class CardStruct : InterpreterStruct
{
    public GameObject card;

    public override object Acces(object key)
    {
        string k = key as string;
        DisplayCard dp = card.GetComponent<DisplayCard>();

        if (k == "Power") return dp.cardPower;
        if (k == "Name") return dp.cardName;
        if (k == "Type") return (dp.cardKind == 'g') ? "Oro" : "Plata";
        if (k == "Range")
        {
            char zone = dp.cardZone;
            if (zone == 'M') return "Melee";
            if (zone == 'R') return "Ranged";
            if (zone == 'S') return "Siege";
            if (zone == 'P') return "PowerUp";
            return 'C';
        }
        if (k == "Faction") return (dp.cardFaction == 0) ? "Shrek" : "Lord Farquaad";
        if (k == "Owner") return (dp.cardFaction == 0) ? refToBoard.shrekFaction : refToBoard.badFaction;
        return default;
    }

    public override object SetAcces(object key, object value, bool isLast = false)
    {
        string k = key as string;
        DisplayCard dp = card.GetComponent<DisplayCard>();

        if (isLast)
        {
            if (k == "Power")
            {
                dp.SetPower((int)value);
            }
            if (k == "Name")
            {
                dp.displayCard.cardName = value as string;
                dp.GetStated();
            }
            if (k == "Type")
            {
                if (value as string == "Oro") dp.cardKind = 'g';
                if (value as string == "Plata") dp.cardKind = 's';
                dp.SetImages();
            }
            if (k == "Range")
            {
                if (value as string == "Melee") dp.cardZone = 'M';
                if (value as string == "Ranged") dp.cardZone = 'R';
                if (value as string == "Siege") dp.cardZone = 'S';
                if (value as string == "Climate") dp.cardZone = 'C';
                if (value as string == "PowerUp") dp.cardZone = 'P';
                dp.SetImages();
            }
            if (k == "Faction")
            {
                if (value as string == "Shrek") dp.cardFaction = 0;
                if (value as string == "Lord Farquaad") dp.cardFaction = 1;
                dp.SetImages();
            }
        }
        if (k == "Owner")
            return (dp.cardFaction == 0) ? refToBoard.shrekFaction : refToBoard.badFaction;

        return default;
    }

    public CardStruct(GameObject card)
    {
        this.card = card;
        refToBoard = GameObject.Find("ReferenceToTheBoard").GetComponent<RefToBoard>();
    }
}

public class FieldStruct : InterpreterStruct
{
    public List<CardStruct> cardList;

    public override object Acces(object key)
    {
        int index = (int)key;
        return cardList[index];
    }

    public override object SetAcces(object key, object value, bool isLast = false)
    {
        int index = (int)key;

        if (isLast)
        {
            cardList[index] = value as CardStruct;
        }

        return cardList[index];
    }

    public bool Contains(CardStruct card)
    {
        return cardList.Contains(card);
    }

    public FieldStruct()
    {
        refToBoard = GameObject.Find("ReferenceToTheBoard").GetComponent<RefToBoard>();
        cardList = new List<CardStruct>();
    }
    public FieldStruct(GameObject cardZone)
    {
        cardList = new List<CardStruct>();
        refToBoard = GameObject.Find("ReferenceToTheBoard").GetComponent<RefToBoard>();
        string name = cardZone.name;

        if (name == "GraveYardShrek" || name == "GraveYardBad")
        {
            GetDisplay(cardZone.GetComponent<Graveyard>().cardsGraveyard);
        }
        else if (name == "PlayerDeck" || name == "PlayerDeckBad")
        {
            GetDisplay(cardZone.GetComponent<Deck>().deck);
        }
        else 
            foreach (Transform card in cardZone.transform)
            {
                cardList.Add(new CardStruct(card.gameObject));
            }
    }

    void GetDisplay(List<GameObject> list)
    {
        foreach (GameObject card in list)
        {
            card.GetComponent<DisplayCard>().GetStated();

            cardList.Add(new CardStruct(card));
        }
    }

    public FieldStruct(List<CardStruct> cardList)
    {
        refToBoard = GameObject.Find("ReferenceToTheBoard").GetComponent<RefToBoard>();
        this.cardList = cardList;
    }

    public void Add(CardStruct card)
    {
        cardList.Add(card);
    }

    public void Shuffle() // See this crap...
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            CardStruct temp = cardList[i];
            int randomIndex = Random.Range(0, cardList.Count);
            cardList[i] = cardList[randomIndex];
            cardList[randomIndex] = temp;
        }
    }

    public void Remove(CardStruct card)
    {
        int index = -1;
        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i] == card)
            {
                index = i;
                break;
            }
        }

        if (index != -1) cardList.RemoveAt(index);
    }

    public CardStruct Pop()
    {
        CardStruct toReturn = cardList[0];
        cardList.RemoveAt(0);
        return toReturn;
    }

    public void SendBottom(CardStruct card)
    {
        Remove(card);
        Add(card);
    }

    public void Push(CardStruct card)
    {
        cardList.Insert(0, card);
    }

    public void Join(FieldStruct fieldToJoin)
    {
        foreach (CardStruct card in fieldToJoin.cardList)
        {
            cardList.Add(card);
        }
    }
}

public class ContextStruct : InterpreterStruct
{
    public List<FieldStruct> fieldList;
    public FieldStruct allCards;

    public ContextStruct()
    {
        refToBoard = GameObject.Find("ReferenceToTheBoard").GetComponent<RefToBoard>();
        fieldList = new List<FieldStruct>();
        allCards = new FieldStruct();
    }

    public void Add(FieldStruct field)
    {
        fieldList.Add(field);
        foreach (CardStruct card in field.cardList)
        {
            allCards.Add(card);
        }
    }

    public override object Acces(object key)
    {
        string k = key as string;
        string faction = (allCards.cardList.Count > 0) ?
            allCards.cardList[0].Acces("Faction") as string : MyTools.GetFaction();

        if (faction == "Shrek")
        {
            if (k == "Hand") return refToBoard.shrekHand;
            if (k == "Graveyard") return refToBoard.shrekGraveyard;
            if (k == "Deck") return refToBoard.shrekDeck;
            if (k == "Melee") return refToBoard.shrekMelee;
            if (k == "Range") return refToBoard.shrekRange;
            if (k == "Siege") return refToBoard.shrekSiege;
            if (k == "TriggerPlayer") return refToBoard.shrekFaction;
        }
        else
        {
            if (k == "Hand") return refToBoard.badHand;
            if (k == "Graveyard") return refToBoard.badGraveyard;
            if (k == "Deck") return refToBoard.badDeck;
            if (k == "Melee") return refToBoard.badMelee;
            if (k == "Range") return refToBoard.badRange;
            if (k == "Siege") return refToBoard.badSiege;
            if (k == "TriggerPlayer") return refToBoard.badFaction;
        }

        return default;
    }

    public override object SetAcces(object key, object value, bool isLast = false)
    {
        string k = key as string;
        string faction = (allCards.cardList.Count > 0) ?
            allCards.cardList[0].Acces("Faction") as string : MyTools.GetFaction();

        if (isLast)
        {
            if (faction == "Shrek")
            {
                if (k == "Hand") refToBoard.shrekHand = value as FieldStruct;
                if (k == "Graveyard") refToBoard.shrekGraveyard = value as FieldStruct;
                if (k == "Deck") refToBoard.shrekDeck = value as FieldStruct;
                if (k == "Melee") refToBoard.shrekMelee = value as FieldStruct;
                if (k == "Range") refToBoard.shrekRange = value as FieldStruct;
                if (k == "Siege") refToBoard.shrekSiege = value as FieldStruct;
            }
            else
            {
                if (k == "Hand") refToBoard.badHand = value as FieldStruct;
                if (k == "Graveyard") refToBoard.badGraveyard = value as FieldStruct;
                if (k == "Deck") refToBoard.badDeck = value as FieldStruct;
                if (k == "Melee") refToBoard.badMelee = value as FieldStruct;
                if (k == "Range") refToBoard.badRange = value as FieldStruct;
                if (k == "Siege") refToBoard.badSiege = value as FieldStruct;
            }
        }

        return Acces(k);
    }

    public FieldStruct DeckOfPlayer(ContextStruct player)
    {
        if (player == refToBoard.shrekFaction)
        {
            return refToBoard.shrekDeck;
        }
        else
        {
            return refToBoard.badDeck;
        }
    }

    public FieldStruct HandOfPlayer(ContextStruct player)
    {
        if (player == refToBoard.shrekFaction)
        {
            return refToBoard.shrekHand;
        }
        else
        {
            return refToBoard.badHand;
        }
    }

    public FieldStruct GraveyardOfPlayer(ContextStruct player)
    {
        if (player == refToBoard.shrekFaction)
        {
            return refToBoard.shrekGraveyard;
        }
        else
        {
            return refToBoard.badGraveyard;
        }
    }

    public FieldStruct FieldOfPlayer(ContextStruct player)
    {
        if (player == refToBoard.shrekFaction)
        {
            return refToBoard.shrekFaction.allCards;
        }
        else
        {
            return refToBoard.badFaction.allCards;
        }
    }
}

public class MultiScope
{
    Scope<object> scope;

    public MultiScope()
    {
        scope = new Scope<object>();
    }

    public MultiScope(MultiScope globalScope)
    {
        scope = new Scope<object>(globalScope.scope);
    }

    public bool IsInScope(string key)
    {
        return scope.IsInScope(key);
    }

    public object Get(string key)
    {
        return scope.Get(key);
    }

    public void Set(string key, object value)
    {
        scope.Set(key, value);
    }
}