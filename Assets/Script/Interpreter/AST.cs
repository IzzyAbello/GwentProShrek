﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


/// <summary>
/// 
/// AST
/// 
/// </summary>
public abstract class AST
{
    public abstract void Print(string height);
}

/// <summary>
/// 
/// OPERATORS
/// 
/// </summary>
public class BinOp : AST
{
    public AST left;
    public Token op;
    public AST right;
    public BinOp(AST left, Token op, AST right)
    {
        this.left = left;
        this.op = op;
        this.right = right;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Binary Operator:");
        left.Print(height + "-Left Operand: \t");
        Debug.Log(height + "-Operator: " + op.type.ToString() + " (" + op.value + ")");
        right.Print(height + "-Right Operand: \t");
    }
}

public class UnaryOp : AST
{
    public Token operation;
    public AST expression;

    public UnaryOp(Token operation, AST expression)
    {
        this.operation = operation;
        this.expression = expression;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Unary Operator: ");
        Debug.Log(height + "-Operator: " + operation.type.ToString() + " (" + operation.value + ")");
        expression.Print(height + "-Expression: \t");
    }
}

/// <summary>
/// 
/// LITERALS
/// 
/// </summary>
public class Int : AST
{
    public Token token;
    public int value;
    public Int(Token token)
    {
        this.token = token;
        value = int.Parse(token.value);
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Integer: \nToken: " + token.type.ToString() + " " + token.value);
    }
}

public class String : AST
{
    public Token token;
    public string text;

    public String(Token token)
    {
        this.token = token;
        text = token.value;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-String: \nText: " + text);
    }
}

public class Bool : AST
{
    public Token token;
    public bool value;

    public Bool (Token token)
    {
        this.token = token;
        value = bool.Parse(token.value);
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Bool: \nValue: " + value);
    }
}


/// <summary>
/// 
/// NAME PARAM
/// 
/// </summary>
public class Name : AST
{
    public string name;

    public Name (Token token)
    {
        name = token.value;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Name: " + name);
    }
}


/// <summary>
/// CARD PARAMS
/// </summary>
public class CardNode : AST
{
    public Name name;
    public Type type;
    public Faction faction;
    public Power power;
    public Range range;
    public OnActivation onActivation;

    public CardNode()
    {
        name = null;
        type = null;
        faction = null;
        power = null;
        range = null;
        onActivation = null;
    }

    public CardNode(Name name, Type type, Faction faction, Power power, Range range, OnActivation onActivation)
    {
        this.name = name;
        this.type = type;
        this.faction = faction;
        this.power = power;
        this.range = range;
        this.onActivation = onActivation;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Card: ");
        name.Print(height + "\t");
        type.Print(height + "\t");
        faction.Print(height + "\t");
        power.Print(height + "\t");
        range.Print(height + "\t");
        onActivation.Print(height + "\t");
    }
}

public class Type : AST
{
    public string type;

    public Type (Token token)
    {
        type = token.value;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Type: " + type);
    }
}

public class Faction : AST
{
    public string faction;

    public Faction(Token token)
    {
        faction = token.value;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Faction: " + faction);
    }
}

public class Power : AST
{
    public int power;

    public Power(Token token)
    {
        power = int.Parse(token.value);
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Power: " + power);
    }
}

public class Range : AST
{
    public string range;

    public Range(Token token)
    {
        range = token.value;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Range: " + range);
    }
}

public class OnActivation : AST
{
    public List<OnActivationElement> onActivation;

    public OnActivation()
    {
        onActivation = null;
    }

    public OnActivation(List<OnActivationElement> onActivation)
    {
        this.onActivation = onActivation;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-OnActivation: ");
        foreach(OnActivationElement element in onActivation)
        {
            element.Print(height + "\t");
        }
    }
}

public class OnActivationElement : AST
{
    public EffectOnActivation effectOnActivation;
    public Selector selector;
    public PostAction postAction;

    public OnActivationElement(EffectOnActivation effectOnActivation, Selector selector)
    {
        this.effectOnActivation = effectOnActivation;
        this.selector = selector;
        postAction = null;
    }

    public OnActivationElement(EffectOnActivation effectOnActivation, Selector selector, PostAction postAction)
    {
        this.effectOnActivation = effectOnActivation;
        this.selector = selector;
        this.postAction = postAction;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-OnActivationELEMENT: ");
        effectOnActivation.Print(height + "\t");
        selector.Print(height + "\t");
        if (postAction != null) postAction.Print(height + "\t");
    }
}

public class EffectOnActivation : AST
{
    public Name name;
    public Args parameters;

    public EffectOnActivation(Name name, Args parameters)
    {
        this.name = name;
        this.parameters = parameters;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-EffectOnActivation: ");
        name.Print(height + "\t");
        parameters.Print(height + "\t");
    }
}

public class PostAction : AST
{
    public Type type;
    public Selector selector;

    public PostAction(Type type)
    {
        this.type = type;
        selector = null;
    }

    public PostAction(Type type, Selector selector)
    {
        this.type = type;
        this.selector = selector;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-PostAction: ");
        type.Print(height + "\t");
        if (selector != null) selector.Print(height + "\t");
    }
}

public class Selector : AST
{
    public Source source;
    public Single single;
    public Predicate predicate;

    public Selector (Source source, Predicate predicate)
    {
        this.source = source;
        this.predicate = predicate;
        single = new Single(new Token(Token.Type.BOOL, "false"));
    }

    public Selector(Source source, Single single, Predicate predicate)
    {
        this.source = source;
        this.single = single;
        this.predicate = predicate;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Selector: ");
        source.Print(height + "\t");
        single.Print(height + "\t");
        predicate.Print(height + "\t");
    }
}

public class Single : AST
{
    public bool single;

    public Single (Token token)
    {
        if (token.type == Token.Type.BOOL)
        {
            if (token.value == "true") single = true;
            else single = false;
        }
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Single: " + single);
    }
}

public class Source : AST
{
    public string source;

    public Source(Token token)
    {
        source = token.value;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "Source: " + source);
    }
}

public class Predicate : AST
{
    public Var unit;
    public AST condition;

    public Predicate(Var unit, AST condition)
    {
        this.unit = unit;
        this.condition = condition;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Predicate: ");
        unit.Print(height + "\t");
        condition.Print(height + "\t");
    }
}


/// <summary>
/// 
/// EFFECT PARAMS
/// 
/// </summary>
public class EffectNode : AST
{
    public Name name;
    public Args parameters;
    public Action action;

    public EffectNode(Name name, Action action)
    {
        this.name = name;
        parameters = null;
        this.action = action;
    }

    public EffectNode(Name name, Args parameters, Action action)
    {
        this.name = name;
        this.parameters = parameters;
        this.action = action;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-EffectNode: ");
        name.Print(height + "\t");
        if (parameters != null) parameters.Print(height + "\t");
        action.Print(height + "\t");
    }
}

public class Action : AST
{
    public Var targets;
    public Var context;
    public Compound body;

    public Action(Var targets, Var context, Compound body)
    {
        this.targets = targets;
        this.context = context;
        this.body = body;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Action: ");
        targets.Print(height + "-Targets: \t");
        context.Print(height + "-Context: \t");
        body.Print(height + "-Body: \t");
    }
}

/// <summary>
/// 
/// COMPOUND OF INSTRUCTIONS
/// 
/// </summary>
public class Compound : AST
{
    public List<AST> children;
    public Compound()
    {
        children = new List<AST>();
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Compound:");
        Debug.Log(height + "Children: ");

        foreach (AST child in children)
            child.Print(height + '\t');
    }
}

public class IfNode : AST
{
    public AST condition;
    public Compound body;

    public IfNode(AST condition, Compound body)
    {
        this.condition = condition;
        this.body = body;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-IfNode: ");
        condition.Print(height + "-Condition: ");
        body.Print(height + "-Body: \t");
    }
}

public class ForLoop : AST
{
    public Compound body;
    public Var target;
    public Var targets;

    public ForLoop(Var target, Var targets, Compound body)
    {
        this.target = target;
        this.body = body;
        this.targets = targets;
    }

    public override void Print(string height)
    {
        target.Print(height + "-Target: ");
        targets.Print(height + "-Targets: ");
        body.Print(height + "-Body: \t");
    }
}

public class WhileLoop : AST
{
    public AST condition;
    public Compound body;

    public WhileLoop (AST condition, Compound body)
    {
        this.condition = condition;
        this.body = body;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-WhileLoop: ");
        condition.Print(height + "-Condition:");
        body.Print(height + "-Body: \t");
    }
}

public class Function : AST
{
    public string functionName;
    public Args args;
    public Var.Type type;

    public Function(string functionName, Args args)
    {
        this.functionName = functionName;
        this.args = args;
        type = Var.Type.NULL;
        TypeToReturn();
    }

    public void TypeToReturn()
    {
        if (functionName == "FieldOfPlayer")
            type = Var.Type.CONTEXT;
        if (functionName == "HandOfPlayer")
            type = Var.Type.FIELD;
        if (functionName == "GraveyardOfPlayer")
            type = Var.Type.FIELD;
        if (functionName == "DeckOfPlayer")
            type = Var.Type.FIELD;
        if (functionName == "Find")
            type = Var.Type.TARGETS;
        if (functionName == "Push")
            type = Var.Type.VOID;
        if (functionName == "SendBottom")
            type = Var.Type.VOID;
        if (functionName == "Pop")
            type = Var.Type.CARD;
        if (functionName == "Remove")
            type = Var.Type.VOID;
        if (functionName == "Shuffle")
            type = Var.Type.VOID;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Function " + functionName + ":");
        Debug.Log("Type to return: " + type.ToString());
        args.Print(height + "-Parameters: ");
    }
}

public class Assign : AST
{
    public Var left;
    public Token op;
    public AST right;
    public Assign(Var left, Token op, AST right)
    {
        this.left = left;
        this.op = op;
        this.right = right;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Assignment:");
        left.Print(height + "-Variable: \t");
        Debug.Log(height + "-Operator: " + op.type.ToString() + " (" + op.value + ")");
        right.Print(height + "-Value: \t");
    }
}

/// <summary>
/// 
/// VAR
/// 
/// </summary>
public class Var : AST
{
    public enum Type
    {
        TARGETS, CONTEXT, CARD, FIELD, INT, STRING, BOOL, VOID, NULL
    }

    public Token token;
    public string value;
    public Type type;

    public Var(Token token)
    {
        this.token = token;
        value = token.value;
        type = Type.NULL;
    }

    public void TypeInParams(Token.Type t)
    {
        if (t == Token.Type.D_BOOL) type = Type.BOOL;
        if (t == Token.Type.D_INT) type = Type.INT;
        if (t == Token.Type.D_STRING) type = Type.STRING;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Variable: ");
        Debug.Log(height + "Token: " + token.type + " " + token.value);
        Debug.Log(height + "Type: " + type.ToString());
        Debug.Log(height + "Value: " + value);
    }
}

public class VarComp : Var
{
    public List<AST> args;

    public VarComp(Token token) : base(token)
    {
        args = new List<AST>();
    }

    public override void Print(string height)
    {
        base.Print(height);
        foreach (AST ast in args)
        {
            ast.Print(height);
        }
    }
}

/// <summary>
/// 
/// ARGS
/// 
/// </summary>
public class Args : AST
{
    public List<AST> args;

    public Args()
    {
        args = new List<AST>();
    }

    public void Add(AST argument)
    {
        args.Add(argument);
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Arguments: ");

        foreach (AST child in args)
            child.Print(height + '\t');
    }
}

public class NoOp : AST
{
    public override void Print(string height)
    {
        Debug.Log(height + "-Empty");
    }
}