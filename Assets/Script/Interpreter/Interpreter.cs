using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter
{
    public AST tree;
    public Scope<MultiScope> GLOBAL_SCOPE;

    public Interpreter(Parser parser)
    {
        tree = parser.Parse();
        GLOBAL_SCOPE = new Scope<MultiScope>();
    }


    /*public void VisitNode(AST node)
    {
        if (node.GetType() == typeof(BinOp)) VisitBinOp(node as BinOp);
        if (node.GetType() == typeof(UnaryOp)) VisitUnaryOp(node as UnaryOp);
        if (node.GetType() == typeof(Int)) VisitInt(node as Int);
        if (node.GetType() == typeof(Bool)) VisitBool(node as Bool);
        if (node.GetType() == typeof(String)) VisitString(node as String);
        if (node.GetType() == typeof(Name)) VisitName(node as Name);
        if (node.GetType() == typeof(CardNode)) VisitCardNode(node as CardNode);
        if (node.GetType() == typeof(Type)) VisitType(node as Type);
        if (node.GetType() == typeof(Faction)) VisitFaction(node as Faction);
        if (node.GetType() == typeof(Power)) VisitPower(node as Power);
        if (node.GetType() == typeof(Range)) VisitRange(node as Range);
        if (node.GetType() == typeof(OnActivation)) VisitOnActivation(node as OnActivation);
        if (node.GetType() == typeof(OnActivationElement)) VisitOnActivationElement(node as OnActivationElement);
        if (node.GetType() == typeof(EffectOnActivation)) VisitEffectOnActivation(node as EffectOnActivation);
        if (node.GetType() == typeof(PostAction)) VisitPostAction(node as PostAction);
        if (node.GetType() == typeof(Selector)) VisitSelector(node as Selector);
        if (node.GetType() == typeof(Single)) VisitSingle(node as Single);
        if (node.GetType() == typeof(Source)) VisitSource(node as Source);
        if (node.GetType() == typeof(Predicate)) VisitPredicate(node as Predicate);
        if (node.GetType() == typeof(EffectNode)) VisitEffectNode(node as EffectNode);
        if (node.GetType() == typeof(Action)) VisitAction(node as Action);
        if (node.GetType() == typeof(Compound)) VisitCompound(node as Compound);
        if (node.GetType() == typeof(IfNode)) VisitIfNode(node as IfNode);
        if (node.GetType() == typeof(ForLoop)) VisitForLoop(node as ForLoop);
        if (node.GetType() == typeof(WhileLoop)) VisitWhileLoop(node as WhileLoop);
        if (node.GetType() == typeof(Function)) VisitFunction(node as Function);
        if (node.GetType() == typeof(Assign)) VisitAssign(node as Assign);
        if (node.GetType() == typeof(Var)) VisitVar(node as Var);
        if (node.GetType() == typeof(VarComp)) VisitVarComp(node as VarComp);
        if (node.GetType() == typeof(Args)) VisitArgs(node as Args);
        if (node.GetType() == typeof(NoOp)) VisitNoOp(node as NoOp);
    }*/

    public void Visit (AST node)
    {
        node.Print("");
    }

}
