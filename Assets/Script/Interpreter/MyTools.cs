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
}
