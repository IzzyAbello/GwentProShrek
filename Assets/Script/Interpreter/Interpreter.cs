using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter
{
    public Parser parser;
    public Dictionary<string, int> GLOBAL_SCOPE;

    public Interpreter(Parser parser)
    {
        this.parser = parser;
        GLOBAL_SCOPE = new Dictionary<string, int>();
    }

    public void Error()
    {
        Debug.Log("Operator not found");
        Debug.Break();
    }

    public int Visit(AST node)
    {
        if (node.GetType() == typeof(BinOp)) return VisitBinOp((BinOp)node);
        if (node.GetType() == typeof(UnaryOp)) return VisitUnaryOp((UnaryOp)node);
        if (node.GetType() == typeof(Num)) return VisitNum((Num)node);
        if (node.GetType() == typeof(String)) return VisitString((String)node);
        if (node.GetType() == typeof(Compound)) return VisitCompound((Compound)node);
        if (node.GetType() == typeof(Assign)) return VisitAssign((Assign)node);
        if (node.GetType() == typeof(Var)) return VisitVar((Var)node);
        if (node.GetType() == typeof(NoOp)) return VisitNoOp((NoOp)node);
        if (node.GetType() == typeof(Conditional)) return VisitConditional((Conditional)node);
        if (node.GetType() == typeof(Function)) return VisitFunction((Function)node);
        Error();
        return 0;
    }

    public int VisitBinOp(BinOp node)
    {
        if (node.op.type == Token.Type.PLUS)
        {
            return Visit(node.left) + Visit(node.right);
        }
        if (node.op.type == Token.Type.MINUS)
        {
            return Visit(node.left) - Visit(node.right);
        }
        if (node.op.type == Token.Type.MULT)
        {
            return Visit(node.left) * Visit(node.right);
        }
        if (node.op.type == Token.Type.DIVIDE)
        {
            return Visit(node.left) / Visit(node.right);
        }
        if (node.op.type == Token.Type.MOD)
        {
            return Visit(node.left) % Visit(node.right);
        }
        if (node.op.type == Token.Type.AND)
        {
            return MyTools.BoolToInt(MyTools.IntToBool(Visit(node.left)) & MyTools.IntToBool(Visit(node.right)));
        }
        if (node.op.type == Token.Type.OR)
        {
            return MyTools.BoolToInt(MyTools.IntToBool(Visit(node.left)) | MyTools.IntToBool(Visit(node.right)));
        }
        if (node.op.type == Token.Type.EQUAL)
        {
            return MyTools.BoolToInt(Visit(node.left) == Visit(node.right));
        }
        if (node.op.type == Token.Type.DIFFER)
        {
            return MyTools.BoolToInt(Visit(node.left) != Visit(node.right));
        }
        if (node.op.type == Token.Type.GREATER_E)
        {
            return MyTools.BoolToInt(Visit(node.left) >= Visit(node.right));
        }
        if (node.op.type == Token.Type.LESS_E)
        {
            return MyTools.BoolToInt(Visit(node.left) <= Visit(node.right));
        }
        if (node.op.type == Token.Type.GREATER)
        {
            return MyTools.BoolToInt(Visit(node.left) > Visit(node.right));
        }
        if (node.op.type == Token.Type.LESS)
        {
            return MyTools.BoolToInt(Visit(node.left) < Visit(node.right));
        }
        return 0;
    }

    public int VisitUnaryOp(UnaryOp node)
    {
        if (node.op.type == Token.Type.PLUS)
        {
            return +Visit(node.expr);
        }
        if (node.op.type == Token.Type.MINUS)
        {
            return -Visit(node.expr);
        }
        if (node.op.type == Token.Type.NOT)
        {
            if (Visit(node.expr) == 1) return 0;
            if (Visit(node.expr) == 0) return 1;
            Error();
            return 0;
        }
        return 0;
    }

    public int VisitNum(Num node)
    {
        return node.value;
    }

    public int VisitString(String node)
    {
        return 0;
    }

    public int VisitCompound(Compound node)
    {
        foreach (AST child in node.children)
        {
            int v = Visit(child);
            if ((child.GetType() == typeof(Compound) && v == 1)
            || (child.GetType() == typeof(Conditional) && v == 1)
            || (child.GetType() == typeof(Function) && ((Function)child).functionName == "break")) return 1;
        }
        return 0;
    }

    public int VisitAssign(Assign node)
    {
        string varName = node.left.value;
        GLOBAL_SCOPE[varName] = Visit(node.right);
        return 0;
    }

    public int VisitVar(Var node)
    {
        string varName = node.value;
        
        //if (node.type == Var.Type.CARD)
        
        if (GLOBAL_SCOPE.ContainsKey(varName))
        {
            return GLOBAL_SCOPE[varName];
        }


        Debug.Log("Variable '" + varName + "' not declared");
        Debug.Break();
        return 0;
    }

    public int VisitNoOp(NoOp node)
    {
        return 0;
    }

    public int VisitConditional(Conditional node)
    {
        if (node.type == Token.Type.IF)
        {
            if (MyTools.IntToBool(Visit(node.condition))) return Visit(node.body);
            return 0;
        }
        if (node.type == Token.Type.WHILE)
        {
            while (MyTools.IntToBool(Visit(node.condition)))
            {
                if (Visit(node.body) == 1) break;
            }
            return 0;
        }
        Error();
        return 0;
    }

    public int VisitFunction(Function node)
    {
        List<int> args = new List<int>();

        foreach (AST arg in node.args.args)
        {
            args.Add(Visit(arg));
        }
        return Commands.activate(node.functionName, args);
    }

    public int Interpret()
    {
        AST tree = parser.Parse();
        return Visit(tree);
    }
}
