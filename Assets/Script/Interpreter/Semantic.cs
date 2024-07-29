using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope
{
    public Dictionary<string, ASTType.Type> LOCAL_SCOPE;
    public Scope GLOBAL_SCOPE;

    public Scope()
    {
        GLOBAL_SCOPE = null;
        LOCAL_SCOPE = new Dictionary<string, ASTType.Type>();
    }
    public Scope(Scope GLOBAL_SCOPE)
    {
        this.GLOBAL_SCOPE = GLOBAL_SCOPE;
        LOCAL_SCOPE = new Dictionary<string, ASTType.Type>();
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


    public ASTType.Type Get(string name)
    {
        if (GLOBAL_SCOPE == null)
        {
            if (LOCAL_SCOPE.ContainsKey(name)) return LOCAL_SCOPE[name];
            else return ASTType.Type.NULL;
        }
        else if (LOCAL_SCOPE.ContainsKey(name)) return LOCAL_SCOPE[name];
        else return GLOBAL_SCOPE.Get(name);
    }
    public ASTType.Type Get(Name name)
    {
        return Get(name.name);
    }
    public ASTType.Type Get(Var variable)
    {
        return Get(variable.value);
    }
    public ASTType.Type Get(EffectNode effect)
    {
        return Get(effect.name.name);
    }
    public ASTType.Type Get(CardNode card)
    {
        return Get(card.name.name);
    }


    public void Set(string name, ASTType.Type type) // SEE THIS
    {
        if (!IsInScope(name)) LOCAL_SCOPE[name] = type;
        else if (Get(name) != type)
        {
            Debug.Log($"ERROR IN ASSIGMENT CANOT CONVERT '{Get(name)}' to '{type}'");
        }
    }
    public void Set(Name name, ASTType.Type type)
    {
        Set(name.name, type);
    }
    public void Set(Var variable, ASTType.Type type)
    {
        Set(variable.value, type);
    }
    public void Set(EffectNode effect, ASTType.Type type)
    {
        Set(effect.name.name, type);
    }
    public void Set(CardNode card, ASTType.Type type)
    {
        Set(card.name.name, type);
    }
}