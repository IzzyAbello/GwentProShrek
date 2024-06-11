using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public abstract class AST
{
    public abstract void Print(string height);
}

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

    public void Error()
    {
        Debug.Log("Non recognized binary operator");
        Debug.Break();
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Binary Operator:");
        Debug.Log(height + "Left Operand:");
        left.Print(height + '\t');
        Debug.Log(height + "Operator: " + op.type.ToString() + " " + op.value);
        Debug.Log(height + "Right Operand:");
        right.Print(height + '\t');
    }
}

public class UnaryOp : AST
{
    public Token op;
    public AST expr;

    public UnaryOp(Token op, AST expr)
    {
        this.op = op;
        this.expr = expr;
    }

    public void Error()
    {
        Debug.Log("Non recognized unary operator");
        Debug.Break();
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Unary Operator:");
        Debug.Log(height + "Operator: " + op.type.ToString() + " " + op.value);
        Debug.Log(height + "Operand:");
        expr.Print(height + '\t');
    }
}

public class Num : AST
{
    public Token token;
    public int value;
    public Num(Token token)
    {
        this.token = token;
        value = int.Parse(token.value);
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Number:");
        Debug.Log(height + "Token: " + token.type.ToString() + " " + token.value);
        Debug.Log(height + "Value: " + value.ToString());
    }
}

public class String : AST
{
    public string text;
    public String(string text)
    {
        this.text = text;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-String:");
        Debug.Log(height + "Text: " + text);
    }
}

public class Bool : AST
{
    public bool value;

    public Bool (bool value)
    {
        this.value = value;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Bool:");
        Debug.Log(height + "Text: " + value);
    }
}

public class Name : AST
{
    public string name;

    public Name (string name)
    {
        this.name = name;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "Name: " + name);
    }
}

public class Type : AST
{
    public string type;

    public Type (string type)
    {
        this.type = type;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "Type: " + type);
    }
}

public class Faction : AST
{
    public string faction;

    public Faction(string faction)
    {
        this.faction = faction;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "Faction: " + faction);
    }
}

public class Power : AST
{
    public int power;

    public Power(int power)
    {
        this.power = power;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "Power: " + power);
    }
}

public class Range : AST
{
    public string range;

    public Range(string range)
    {
        this.range = range;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "Range: " + range);
    }
}

public class Compound : AST
{
    public List<AST> children;
    public Compound()
    {
        children = new List<AST>();
    }

    public bool IsValidCard()
    {
        Dictionary<Token.Type, int> check = new Dictionary<Token.Type, int>();
        foreach(AST node in children)
        {
            if (node.GetType() == typeof(Name)) check[Token.Type.NAME]++;
            if (node.GetType() == typeof(Type)) check[Token.Type.TYPE]++;
            if (node.GetType() == typeof(Faction)) check[Token.Type.FACTION]++;
            if (node.GetType() == typeof(Power)) check[Token.Type.POWER]++;
            if (node.GetType() == typeof(Range)) check[Token.Type.RANGE]++;
            if (node.GetType() == typeof(Compound)) check[Token.Type.ONACTIVATION]++;
        }

        if(children.Count != 6) return false;
        if (check.ContainsKey(Token.Type.NAME) && check[Token.Type.NAME] != 1) return false;
        if (check.ContainsKey(Token.Type.TYPE) && check[Token.Type.TYPE] != 1) return false;
        if (check.ContainsKey(Token.Type.FACTION) && check[Token.Type.FACTION] != 1) return false;
        if (check.ContainsKey(Token.Type.POWER) && check[Token.Type.POWER] != 1) return false;
        if (check.ContainsKey(Token.Type.RANGE) && check[Token.Type.RANGE] != 1) return false;
        if (check.ContainsKey(Token.Type.ONACTIVATION) && check[Token.Type.ONACTIVATION] != 1) return false;

        return true;
    }

    public bool IsValidEffect()
    {
        Dictionary<Token.Type, int> check = new Dictionary<Token.Type, int>();
        foreach (AST node in children)
        {
            if (node.GetType() == typeof(Name)) check[Token.Type.NAME]++;
            if (node.GetType() == typeof(Compound) && !check.ContainsKey(Token.Type.PARAMS)) check[Token.Type.PARAMS]++;
            if (node.GetType() == typeof(Compound) && !check.ContainsKey(Token.Type.ACTION)) check[Token.Type.ACTION]++;
        }

        if (children.Count > 3) return false;
        if (check.ContainsKey(Token.Type.NAME) && check[Token.Type.NAME] != 1) return false;
        if (check.ContainsKey(Token.Type.PARAMS) && check[Token.Type.PARAMS] > 1) return false;
        if (check.ContainsKey(Token.Type.ACTION) && check[Token.Type.FACTION] != 1) return false;

        return true;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Compound:");
        Debug.Log(height + "Children:");

        foreach (AST child in children)
            child.Print(height + '\t');
    }
}

public class Conditional : AST
{
    public Token.Type type;
    public Compound body;
    public AST condition;
    public Conditional(Token.Type type, AST condition, Compound body)
    {
        this.type = type;
        this.body = body;
        this.condition = condition;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-" + type.ToString());
        Debug.Log(height + "Condition:");
        condition.Print(height + '\t');
        Debug.Log(height + "Body:");

        foreach (AST child in body.children)
            child.Print(height + '\t');
    }
}

public class ExpectedParameter : AST
{
    (string, Token.Type) param;

    public ExpectedParameter ((string, Token.Type) param)
    {
        this.param = param;
    }

    public bool IsValidAssign(ExpectedParameter a, ExpectedParameter b)
    {
        if (a.param.Item1 != b.param.Item1)
        {
            Debug.Log("Not Valid Name...");
            return false;
        }
        if (a.param.Item2 != b.param.Item2)
        {
            Debug.Log("Not Valid Type...");
            return false;
        }
        return true;
    }

    public override void Print(string height)
    {
        Debug.Log($"ExpectedParameter\nName: {param.Item1}  Type: {param.Item2}");
    }
}

public class Args : AST
{
    public List<AST> args;

    public Args(List<AST> args)
    {
        this.args = args;
    }

    public void Add(AST argument)
    {
        args.Add(argument);
    }

    public override void Print(string height)
    {
        Debug.Log(height + "Arguments:");

        foreach (AST child in args)
            child.Print(height + '\t');
    }
}

public class Function : AST
{
    public string functionName;
    public Args args;
    public Function(string functionName, Args args)
    {
        this.functionName = functionName;
        this.args = args;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Method '" + functionName + "':");
        args.Print(height);
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
        Debug.Log(height + "Variable:");
        left.Print(height + '\t');
        Debug.Log(height + "Operator: " + op.type.ToString() + " " + op.value);
        Debug.Log(height + "Value:");
        right.Print(height + '\t');
    }
}

public class Var : AST
{
    public Token token;
    public string value;
    public Var(Token token)
    {
        this.token = token;
        value = token.value;
    }

    public override void Print(string height)
    {
        Debug.Log(height + "-Variable:");
        Debug.Log(height + "Token: " + token.type + " " + token.value);
        Debug.Log(height + "Value: " + value);
    }
}

public class NoOp : AST
{
    public override void Print(string height)
    {
        Debug.Log(height + "-Empty");
    }
}