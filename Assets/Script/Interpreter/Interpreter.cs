using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class Interpreter
{
    // Fields of Interpreter
    Parser parser;
    RefToBoard Ref;
    public MultiScope globalScope;
    public ContextStruct allBoard;

    public Interpreter(Parser parser)
    {
        try
        {
            this.parser = parser;
            parser.Parse();
            globalScope = new MultiScope();
            Ref = GameObject.Find("ReferenceToTheBoard").GetComponent<RefToBoard>();
            ResetRefAndContext();

            if (parser.curError == "")
            {
                Debug.Log("Parse-Semantic check: OK");
                CardVisit(parser.CARD);
            }
            else Debug.Log("Parse-Semantic check: ERROR");
        }
        catch
        {
            Debug.Log("FATAL ERROR IN INTERPRETER CONSTRUCTOR");
        }
    }

    public object Visit(AST node, MultiScope scope)
    {
        if (node.GetType() == typeof(BinOp)) return BinOpVisit(node as BinOp, scope);
        if (node.GetType() == typeof(UnaryOp)) return UnaryOpVisit(node as UnaryOp, scope);
        if (node.GetType() == typeof(Int)) return IntVisit(node as Int);
        if (node.GetType() == typeof(Bool)) return BoolVisit(node as Bool);
        if (node.GetType() == typeof(String)) return StringVisit(node as String);
        if (node.GetType() == typeof(Var)) return VarVisit(node as Var, scope);
        if (node.GetType() == typeof(VarComp)) return VarCompVisit(node as VarComp, scope);
        if (node.GetType() == typeof(Indexer)) return IndexerVisit(node as Indexer, scope);

        if (node.GetType() == typeof(Assign)) AssignVisit(node as Assign, scope);
        if (node.GetType() == typeof(IfNode)) IfNodeVisit(node as IfNode, scope);
        if (node.GetType() == typeof(WhileLoop)) WhileLoopVisit(node as WhileLoop, scope);
        if (node.GetType() == typeof(ForLoop)) ForLoopVisit(node as ForLoop, scope);

        if (node.GetType() == typeof(NoOp)) return NoOpVisit();

        return default;
    }

    public void ResetRefAndContext()
    {
        Ref.ResetBoard();
        allBoard = Ref.FillAllBoard();
    }

    public object BinOpVisit(BinOp node, MultiScope scope)
    {
        object left = Visit(node.left, scope);
        object right = Visit(node.right, scope);

        if (node.type == ASTType.Type.INT)
        {
            int l = (int)left, r = (int)right;

            if (node.op.type == Token.Type.PLUS) return l + r;
            if (node.op.type == Token.Type.MINUS) return l - r;
            if (node.op.type == Token.Type.MULT) return l * r;
            if (node.op.type == Token.Type.DIVIDE) return l / r;
            if (node.op.type == Token.Type.MOD) return l % r;
        }

        if (node.type == ASTType.Type.STRING)
        {
            string l = left as string, r = right as string;

            if (node.op.type == Token.Type.STRING_SUM) return l + r;
            if (node.op.type == Token.Type.STRING_SUM_S) return l + " " + r;
        }

        if (node.type == ASTType.Type.BOOL)
        {
            if (left.GetType() == typeof(int))
            {
                int l = (int)left, r = (int)right;

                if (node.op.type == Token.Type.EQUAL) return l == r;
                if (node.op.type == Token.Type.DIFFER) return l != r;
                if (node.op.type == Token.Type.GREATER) return l > r;
                if (node.op.type == Token.Type.GREATER_E) return l >= r;
                if (node.op.type == Token.Type.LESS) return l < r;
                if (node.op.type == Token.Type.LESS_E) return l <= r;
            }

            if (left.GetType() == typeof(bool))
            {
                bool l = (bool)left, r = (bool)right;
                if (node.op.type == Token.Type.EQUAL) return l == r;
                if (node.op.type == Token.Type.DIFFER) return l != r;
                if (node.op.type == Token.Type.AND) return l && r;
                if (node.op.type == Token.Type.OR) return l || r;
            }

            if (left.GetType() == typeof(string))
            {
                string l = left as string, r = right as string;
                if (node.op.type == Token.Type.EQUAL) return l == r;
                if (node.op.type == Token.Type.DIFFER) return l != r;
            }
        }

        return default;
    }

    public object UnaryOpVisit(UnaryOp node, MultiScope scope)
    {
        object expression = Visit(node.expression, scope);

        if (node.type == ASTType.Type.INT)
        {
            int e = (int)expression;

            if (node.operation.type == Token.Type.PLUS) return +e;
            else if (node.operation.type == Token.Type.MINUS) return -e;
            else
            {
                Var variable = node.expression as Var;
                if (node.operation.type == Token.Type.PLUSPLUS) e++;
                if (node.operation.type == Token.Type.MINUSMINUS) e--;

                SetValue(variable, scope, e);
                return e;
            }
            
            
        }

        if (node.type == ASTType.Type.BOOL)
        {
            bool e = (bool)expression;

            if (node.operation.type == Token.Type.NOT) return !e;
        }

        return default;
    }

    public int IndexerVisit(Indexer node, MultiScope scope)
    {
        int expression = (int)Visit(node.index, scope);
        return expression;
    }

    public int IntVisit(Int node)
    {
        return node.value;
    }

    public bool BoolVisit(Bool node)
    {
        return node.value;
    }

    public string StringVisit(String node)
    {
        return node.text;
    }

    public void AssignVisit(Assign node, MultiScope scope)
    {
        Var v = node.left;
        string op = node.op.value;
        string key = node.left.value;
        object left = default;
        if (node.left.GetType() == typeof(VarComp)) left = VarCompVisit(node.left as VarComp, scope);
        else if (scope.IsInScope(key)) left = scope.Get(key);
        object right = Visit(node.right, scope);

        if (op == "=") SetValue(v, scope, right);
        if (op == "+=") SetValue(v, scope, (int)left + (int)right);
        if (op == "-=") SetValue(v, scope, (int)left - (int)right);
        if (op == "*=") SetValue(v, scope, (int)left * (int)right);
        if (op == "/=") SetValue(v, scope, (int)left / (int)right);
        if (op == "%=") SetValue(v, scope, (int)left % (int)right);
        if (op == "@=") SetValue(v, scope, (string)left + (string)right);
    }

    public object NoOpVisit()
    {
        return default;
    }

    public object VarVisit(Var node, MultiScope scope)
    {
        return scope.Get(node.value);
    }

    public void SetValue(Var node, MultiScope scope, object value)
    {
        if (node.GetType() == typeof(Var))
        {
            scope.Set(node.value, value);
        }
        else
        {
            int index = 0;
            VarComp v = node as VarComp;
            object last = scope.Get(node.value);
            AST lastAST = v;

            foreach (var member in v.args)
            {
                bool isLast = (index == v.args.Count - 1);

                if (member.GetType() == typeof(Function))
                {
                    Function function = member as Function;
                    last = FunctionVisit(function, lastAST, last, scope);
                }

                if (member.GetType() == typeof(Indexer))
                {
                    FieldStruct field = last as FieldStruct;
                    Indexer indexer = member as Indexer;
                    int ind = (int)Visit(indexer, scope);
                    last = field.SetAcces(ind, value, isLast);
                }
                else if (member.GetType() == typeof(Pointer))
                {
                    ContextStruct context = last as ContextStruct;
                    Pointer pointer = member as Pointer;
                    last = context.SetAcces(pointer.pointer, value, isLast);
                }
                else
                {
                    CardStruct card = last as CardStruct;
                    Var SetAcces = member as Var;
                    last = card.SetAcces(SetAcces.value, value, isLast);
                }
                index++;
                lastAST = member;
            }
        }
    }

    public object VarCompVisit(VarComp node, MultiScope scope)
    {
        object last = scope.Get(node.value);
        AST lastAST = node;

        foreach (var member in node.args)
        {
            if (member.GetType() == typeof(Function))
            {
                Function function = member as Function;
                last = FunctionVisit(function, lastAST, last, scope);
            }
            else if (member.GetType() == typeof(Indexer))
            {
                FieldStruct field = last as FieldStruct;
                Indexer indexer = member as Indexer;
                int index = (int)Visit(indexer, scope);
                last = field.Acces(index);
            }
            else if (member.GetType() == typeof(Pointer))
            {
                ContextStruct context = last as ContextStruct;
                Pointer pointer = member as Pointer;
                last = context.Acces(pointer.pointer);
            }
            else
            {
                CardStruct card = last as CardStruct;
                Var acces = member as Var;
                last = card.Acces(acces.value);
            }
            lastAST = member;
        }

        return last;
    }

    public object FunctionVisit(Function node, AST realContext, object context, MultiScope scope)
    {
        string name = node.functionName;
        object parameter = (node.args != null && name != "Find")
            ? Visit(node.args.args[0], scope) : default;
        bool isReal = (realContext.GetType() == typeof(Pointer));

        if (name == "Add")
        {
            FieldStruct field = context as FieldStruct;
            CardStruct card = parameter as CardStruct;
            field.Add(card);

            if (isReal)
            {
                GameObject parent = MyTools.SetPointer(Ref, realContext as Pointer);
                GameObject.Instantiate(card.card, parent.transform);
            }
        }

        if (name == "Shuffle")
        {
            FieldStruct field = context as FieldStruct;
            field.Shuffle();
        }

        if (name == "Remove")
        {
            FieldStruct field = context as FieldStruct;
            CardStruct card = parameter as CardStruct;
            if (isReal)
            {
                GameObject parent = GameObject.Find("Abyss");
                GameObject.Instantiate(card.card, parent.transform);
                GameObject.Destroy(card.card);
            }
            field.Remove(card);
        }

        if (name == "Pop")
        {
            FieldStruct field = context as FieldStruct;
            if (isReal)
            {
                GameObject parent = GameObject.Find("Abyss");
                GameObject card = field.cardList[0].card;
                GameObject.Instantiate(card, parent.transform);
                GameObject.DestroyImmediate(card, true);
            }

            return field.Pop();
        }

        if (name == "SendBottom")
        {
            FieldStruct field = context as FieldStruct;
            CardStruct card = parameter as CardStruct;
            field.SendBottom(card);
        }

        if (name == "Push")
        {
            FieldStruct field = context as FieldStruct;
            CardStruct card = parameter as CardStruct;
            field.Push(card);
        }

        if (name == "DeckOfPlayer")
        {
            ContextStruct field = context as ContextStruct;
            ContextStruct player = parameter as ContextStruct;
            return field.DeckOfPlayer(player);
        }

        if (name == "HandOfPlayer")
        {
            ContextStruct field = context as ContextStruct;
            ContextStruct player = parameter as ContextStruct;
            return field.HandOfPlayer(player);
        }

        if (name == "GraveyardOfPlayer")
        {
            ContextStruct field = context as ContextStruct;
            ContextStruct player = parameter as ContextStruct;
            return field.GraveyardOfPlayer(player);
        }

        if (name == "FieldOfPlayer")
        {
            ContextStruct field = context as ContextStruct;
            ContextStruct player = parameter as ContextStruct;
            return field.FieldOfPlayer(player);
        }

        if (name == "Find")
        {
            FieldStruct field = context as FieldStruct;
            Var unit = node.args.args[0] as Var;
            AST condition = node.args.args[1];
            FieldStruct toReturn = new FieldStruct();
            foreach (CardStruct card in field.cardList)
            {
                SetValue(unit, scope, card);

                if ((bool)Visit(condition, new MultiScope(scope)))
                {
                    toReturn.Add(card);
                }
            }
            return toReturn;
        }

        return default;
    }

    public void OnActivationVisit(OnActivation node, MultiScope scope)
    {
        foreach (OnActivationElement element in node.onActivation)
        {
            ResetRefAndContext();
            MultiScope innerScope = new MultiScope(scope);
            OnActivationElementVisit(element, innerScope);
        }
    }

    public void PostActionVisit(PostAction node, MultiScope scope, string source)
    {
        ResetRefAndContext();
        if (node.selector.source.source == "parent") node.selector.source.source = source;
        string name = node.effectOnActivation.name.name;
        MultiScope effectParams = EffectOnActivationVisit(node.effectOnActivation, scope);
        allBoard = SelectorVisit(node.selector, scope);
        EffectNode effect = parser.EFFECT_LIST[name];
        EffectActionVisit(effect, effectParams);
    }

    public void OnActivationElementVisit(OnActivationElement node, MultiScope scope)
    {
        string name = node.effectOnActivation.name.name;
        MultiScope effectParams = EffectOnActivationVisit(node.effectOnActivation, scope);
        allBoard = SelectorVisit(node.selector, scope);
        string source = node.selector.source.source;
        EffectNode effect = parser.EFFECT_LIST[name];
        EffectActionVisit(effect, effectParams);

        if (node.postAction != null)
        {
            PostActionVisit(node.postAction, scope, source);
        }
    }

    public ContextStruct SelectorVisit(Selector node, MultiScope scope)
    {
        ContextStruct context = new ContextStruct();
        string source = node.source.source;

        if (source == "board") context = Ref.allBoard;
        else if (source == "hand")
            context.Add((scope.Get("$$$Card->\tFaction") as string == Ref.shrekFactionString)
                ? Ref.shrekHand : Ref.badHand);
        else if (source == "otherHand")
            context.Add((scope.Get("$$$Card->\tFaction") as string == Ref.shrekFactionString)
                ? Ref.badHand : Ref.shrekHand);
        else if (source == "deck")
            context.Add((scope.Get("$$$Card->\tFaction") as string == Ref.shrekFactionString)
                ? Ref.shrekDeck : Ref.badDeck);
        else if (source == "otherDeck")
            context.Add((scope.Get("$$$Card->\tFaction") as string == Ref.shrekFactionString)
                ? Ref.badDeck : Ref.shrekDeck);
        else if (source == "field")
            context = (scope.Get("$$$Card->\tFaction") as string == Ref.shrekFactionString)
                ? Ref.shrekFaction : Ref.badFaction;
        else if (source == "otherField")
            context = (scope.Get("$$$Card->\tFaction") as string == Ref.shrekFactionString)
                ? Ref.badFaction : Ref.shrekFaction;

        FieldStruct allCardsAfterPredicate = new FieldStruct();
        foreach (CardStruct card in context.allCards.cardList)
        {
            SetValue(node.predicate.unit, scope, card);
            bool condition = (bool)Visit(node.predicate.condition, new MultiScope(scope));
            if (condition) allCardsAfterPredicate.Add(card);
        }

        if (node.single == null) Ref.AfterPredicateFilter(allCardsAfterPredicate);
        else Ref.AfterPredicateFilter(allCardsAfterPredicate, node.single.single);
        context = Ref.allBoard;

        return context;
    }

    public MultiScope EffectOnActivationVisit(EffectOnActivation node, MultiScope outScope)
    {
        MultiScope scope = new MultiScope(outScope);

        if (node.parameters != null)
            foreach (AST element in node.parameters.args)
            {
                Assign assign = element as Assign;
                scope.Set(assign.left.value, Visit(assign.right, scope));
            }

        return scope;
    }

    public void EffectActionVisit(EffectNode node, MultiScope scope)
    {
        scope.Set(node.action.context.value, allBoard);
        scope.Set(node.action.targets.value, allBoard.allCards);

        CompoundVisit(node.action.body, scope);
    }

    public void IfNodeVisit(IfNode node, MultiScope scope)
    {
        bool condition = (bool)Visit(node.condition, scope);
        if (condition) CompoundVisit(node.body, scope);
    }

    public void WhileLoopVisit(WhileLoop node, MultiScope scope)
    {
        while ((bool)Visit(node.condition, scope))
        {
            CompoundVisit(node.body, scope);
        }       
    }

    public void ForLoopVisit(ForLoop node, MultiScope scope)
    {   
        FieldStruct field = Visit(node.targets, scope) as FieldStruct;

        foreach (CardStruct card in field.cardList)
        {
            SetValue(node.target, scope, card);    
            CompoundVisit(node.body, scope);
        }
    }

    public void CompoundVisit(Compound node, MultiScope outScope)
    {
        MultiScope scope = new MultiScope(outScope);

        foreach (AST child in node.children)
            Visit(child, scope);
    }

    public void CardVisit(CardNode node)
    {
        string name = node.name.name;
        char type = (node.typeNode.type == "Oro") ? 'g' : 's';
        int faction = (node.faction.faction == "Shrek") ? 0 : 1;
        int power = node.power.power;
        char range = node.range.range[0];

        string scriptableObjectPath = "Assets/Prefabs/Cards/UserCards/UserCardSO.asset";
        string cardPath = "Assets/Prefabs/Cards/UserCards/UserCard.prefab";

        Card cardScriptableObject = ScriptableObject.CreateInstance<Card>();
        cardScriptableObject.cardId = Random.Range(100, int.MaxValue);
        cardScriptableObject.cardName = name;
        cardScriptableObject.cardKind = type;
        cardScriptableObject.cardFaction = faction;
        cardScriptableObject.cardPowerOG = power;
        cardScriptableObject.cardPower = power;
        cardScriptableObject.cardZone = range;
        cardScriptableObject.cardEffect = "Special";
        cardScriptableObject.cardCommunion = "None";
        cardScriptableObject.cardDescription = "Carta pilla creada por el usuario";
        cardScriptableObject.interpreter = this;
        cardScriptableObject.cardSprite =
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/GENERIC_IMAGE_FOR_CARD_EDITOR.jpg");

        AssetDatabase.CreateAsset(cardScriptableObject, scriptableObjectPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GameObject cardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Card.prefab");

        GameObject cardCopy = GameObject.Instantiate(cardPrefab);
        cardCopy.GetComponent<DisplayCard>().displayCard =
            (Card)AssetDatabase.LoadAssetAtPath<ScriptableObject>(scriptableObjectPath);

        PrefabUtility.SaveAsPrefabAsset(cardCopy, cardPath);

        GameObject.DestroyImmediate(cardCopy);

        GameObject deck = (faction == 0) ? GameObject.Find("PlayerDeck") : GameObject.Find("PlayerDeckBad");
        deck.GetComponent<Deck>().deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>(cardPath));
        deck.GetComponent<Deck>().deck.Reverse();
    }

    public void InterpretEffectToPlay()// RUN FROM OUTSIDE...
    {
        MultiScope outScope = new MultiScope(globalScope);
        CardNode card = parser.CARD;
        outScope.Set("$$$Card->\tFaction", (card.faction.faction == "Shrek") ? "Shrek" : "Lord Farquaad");

        MultiScope scope = new MultiScope(outScope);

        OnActivationVisit(card.onActivation, scope);

    }
}