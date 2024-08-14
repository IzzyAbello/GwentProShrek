using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyTools
{
    public static bool IsDigit(char c)
    {
        return (c >= '0' && c <= '9');
    }

    public static bool IsAlpha(char c)
    {
        return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_');
    }

    public static bool IsAlnum(char c)
    {
        return (IsDigit(c) || IsAlpha(c));
    }

    public static int BoolToInt(bool b)
    {
        if (b) return 1;
        return 0;
    }

    public static bool IntToBool (int n)
    {
        if (n == 1) return true;
        return false;
    }
    
    public static string GetFaction()
    {
        SwitchTurn logic = GameObject.Find("PlayTurnButton").GetComponent<SwitchTurn>(); ;
        Vector3 position = new Vector3(logic.playPositionHand.x, logic.playPositionHand.y);

        return (position == logic.handShrek.transform.position) ? "Shrek" : "Lord Farquaad";
    }

    public static GameObject SetPointer(RefToBoard refToBoard, Pointer pointer)
    {
        string k = pointer.pointer;
        string faction = GetFaction();

        if (faction == "Shrek")
        {
            if (k == "Hand") return refToBoard.shrekHandRef;
            if (k == "Graveyard") return refToBoard.shrekGraveyardRef;
            if (k == "Deck") return refToBoard.shrekDeckRef;
            if (k == "Melee") return refToBoard.shrekMeleeRef;
            if (k == "Range") return refToBoard.shrekRangeRef;
            if (k == "Siege") return refToBoard.shrekSiegeRef;
        }
        else
        {
            if (k == "Hand") return refToBoard.badHandRef;
            if (k == "Graveyard") return refToBoard.badGraveyardRef;
            if (k == "Deck") return refToBoard.badDeckRef;
            if (k == "Melee") return refToBoard.badMeleeRef;
            if (k == "Range") return refToBoard.badRangeRef;
            if (k == "Siege") return refToBoard.badSiegeRef;
        }

        return default;
    }
}
