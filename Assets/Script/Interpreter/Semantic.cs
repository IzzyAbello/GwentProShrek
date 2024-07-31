using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope <T>
{
    public Dictionary<string, T> LOCAL_SCOPE;
    public Scope <T> GLOBAL_SCOPE;

    public Scope()
    {
        GLOBAL_SCOPE = null;
        LOCAL_SCOPE = new Dictionary<string, T>();
    }
    public Scope(Scope <T> GLOBAL_SCOPE)
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


    public void Set(string name, T type)
    {
        if (!IsInScope(name)) LOCAL_SCOPE[name] = type;
        else if (!Equals(Get(name), type))
        {
            Debug.Log($"ERROR IN ASSIGMENT CANOT CONVERT '{Get(name)}' to '{type}'");
        }
    } // SEE THIS
    public void Set(Name name, T type)
    {
        Set(name.name, type);
    }
    public void Set(Var variable, T type)
    {
        Set(variable.value, type);
    }
    public void Set(EffectNode effect, T type)
    {
        Set(effect.name.name, type);
    }
    public void Set(CardNode card, T type)
    {
        Set(card.name.name, type);
    }
}

public class FieldStruct // Create all methods for this shit...
{
    List<GameObject> cardList;

    public FieldStruct()
    {
        cardList = new List<GameObject>();
    }

    public void Add(GameObject card)
    {
        cardList.Add(card);
    }
}

public class ContextStruct // And this...
{
    List<FieldStruct> fieldList;

    public ContextStruct()
    {
        fieldList = new List<FieldStruct>();
    }
}

public class EffectStruct // Work on this...
{
    public string name;
    public Dictionary<string, int> INT_PARAMS;
    public Dictionary<string, string> STRING_PARAMS;
    public Dictionary<string, bool> BOOL_PARAMS;
    public ContextStruct context; // Maybe isn't mandatory
    public FieldStruct targets;
    

    public EffectStruct(EffectNode effect)
    {
        name = effect.name.name;


        ///
    }

}

public class MultiScope
{
    public Dictionary<string, int> INT_SCOPE;
    public Dictionary<string, string> STRING_SCOPE;
    public Dictionary<string, bool> BOOL_SCOPE;
    public Dictionary<string, GameObject> CARD_SCOPE;
    public Dictionary<string, FieldStruct> FIELD_SCOPE;
    public Dictionary<string, ContextStruct> CONTEXT_SCOPE;

    public MultiScope()
    {
        INT_SCOPE = new Dictionary<string, int>();
        STRING_SCOPE = new Dictionary<string, string>();
        BOOL_SCOPE = new Dictionary<string, bool>();
        CARD_SCOPE = new Dictionary<string, GameObject>();
        FIELD_SCOPE = new Dictionary<string, FieldStruct>();
        CONTEXT_SCOPE = new Dictionary<string, ContextStruct>();
    }

    public bool Equals(MultiScope other) // Let's see if it fits in anyone...
    {
        if (INT_SCOPE != other.INT_SCOPE) return false;
        if (STRING_SCOPE != other.STRING_SCOPE) return false;
        if (BOOL_SCOPE != other.BOOL_SCOPE) return false;
        if (CARD_SCOPE != other.CARD_SCOPE) return false;
        if (FIELD_SCOPE != other.FIELD_SCOPE) return false;
        if (CONTEXT_SCOPE != other.CONTEXT_SCOPE) return false;
        return true;
    }
}