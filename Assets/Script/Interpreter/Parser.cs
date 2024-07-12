using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parser
{
    public Lexer lexer;
    public Token currentToken;
    public string curError;

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
        currentToken = lexer.GetNextToken();
        curError = "";
    }

    public void Error()
    {
        curError = "Invalid syntax\n";
        curError += "Current token type: " + currentToken.type + "\n";
        curError += "Current token value: " + currentToken.value + "\n";
        curError += "Code:\n";
        curError += (lexer.text.Substring(0, lexer.pos)) + "\n";
        curError += "-----------------------------ERROR-----------------------------\n";
        curError += (lexer.text.Substring(lexer.pos)) + "\n";
        Debug.Log(curError);
    }

    public void Eat(Token.Type tokenType)
    {
        try
        {
            if (currentToken.type == tokenType)
            {
                currentToken = lexer.GetNextToken();
            }
            else
            {
                curError = "Invalid syntax: ";
                curError += "Current token type: " + currentToken.type + ". ";
                curError += "Expected token type: " + tokenType + ". ";
                curError += "Code:\n";
                curError += (lexer.text.Substring(0, lexer.pos)) + "\n";
                curError += "-----------------------------ERROR-----------------------------\n";
                curError += (lexer.text.Substring(lexer.pos)) + "\n";
                Debug.Log(curError);
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Factor()
    {
        try
        {
            Token token = currentToken;
            if (token.type == Token.Type.PLUS)
            {
                Eat(Token.Type.PLUS);
                UnaryOp node = new UnaryOp(token, Factor());
                return node;
            }
            if (token.type == Token.Type.MINUS)
            {
                Eat(Token.Type.MINUS);
                UnaryOp node = new UnaryOp(token, Factor());
                return node;
            }
            if (token.type == Token.Type.BOOL)
            {
                Bool node = new Bool(token);
                Eat(Token.Type.BOOL);
                return node;
            }
            if (token.type == Token.Type.INT)
            {
                Eat(Token.Type.INT);
                Int node = new Int(token);
                return node;
            }
            if (token.type == Token.Type.STRING)
            {
                Eat(Token.Type.STRING);
                String node = new String(token);
                return node;
            }
            if (token.type == Token.Type.L_PARENTHESIS)
            {
                Eat(Token.Type.L_PARENTHESIS);
                AST result = Expression();
                Eat(Token.Type.R_PARENTHESIS);
                return result;
            }
            if (currentToken.type == Token.Type.ID)
            {
                return Variable();
            }
            if (currentToken.type == Token.Type.FUNCTION)
            {
                return FunctionStatement(currentToken.value);
            }
            Error();
            return new NoOp();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Term()
    {
        try
        {
            AST node = Factor();
            Token token = currentToken;
            if (token.type == Token.Type.MULT || token.type == Token.Type.DIVIDE || token.type == Token.Type.MOD)
            {
                Eat(token.type);
                node = new BinOp(node, token, Expression());
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Expression()
    {
        try
        {
            AST node = Term();
            Token token = currentToken;
            if (token.type == Token.Type.PLUS || token.type == Token.Type.MINUS)
            {
                Eat(token.type);
                node = new BinOp(node, token, Expression());
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST BooleanFactor()
    {
        try
        {
            Token token = currentToken;
            if (token.type == Token.Type.NOT)
            {
                Eat(Token.Type.NOT);
                Eat(Token.Type.L_PARENTHESIS);
                UnaryOp unaryOp = new UnaryOp(token, BooleanExpression());
                Eat(Token.Type.R_PARENTHESIS);
                return unaryOp;
            }
            if (token.type == Token.Type.L_PARENTHESIS)
            {
                Eat(Token.Type.L_PARENTHESIS);
                AST result = BooleanExpression();
                Eat(Token.Type.R_PARENTHESIS);
                return result;
            }

            AST left = Expression();

            token = currentToken;
            if (token.type == Token.Type.EQUAL) Eat(Token.Type.EQUAL);
            else if (token.type == Token.Type.DIFFER) Eat(Token.Type.DIFFER);
            else if (token.type == Token.Type.GREATER_E) Eat(Token.Type.GREATER_E);
            else if (token.type == Token.Type.LESS_E) Eat(Token.Type.LESS_E);
            else if (token.type == Token.Type.LESS) Eat(Token.Type.LESS);
            else if (token.type == Token.Type.GREATER) Eat(Token.Type.GREATER);
            else if (token.type == Token.Type.R_PARENTHESIS) return left;
            else Error();

            AST right = Expression();

            BinOp node = new BinOp(left, token, right);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST BooleanTerm()
    {
        try
        {
            AST node = BooleanFactor();
            Token token = currentToken;

            if (token.type == Token.Type.OR)
            {
                Eat(Token.Type.OR);
                return new BinOp(node, token, BooleanExpression());
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST BooleanExpression()
    {
        try
        {
            AST node = BooleanTerm();
            Token token = currentToken;

            if (token.type == Token.Type.AND)
            {
                Eat(Token.Type.AND);
                node = new BinOp(node, token, BooleanExpression());
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Var Variable()// Look for this... 
    {
        try
        {
            Var node = new Var(currentToken);
            Eat(Token.Type.ID);

            if (currentToken.type == Token.Type.DOT)
            {
                VarComp nd = new VarComp(node.token);
                while (currentToken.type == Token.Type.DOT && currentToken.type != Token.Type.EOF)
                {
                    Eat(Token.Type.DOT);
                    if (currentToken.type == Token.Type.FUNCTION)
                    {
                        Function f = FunctionStatement(currentToken.value);
                        nd.args.Add(f);
                    }
                    else // Look for this...
                    {
                        Token token = currentToken;
                        if (token.type == Token.Type.TYPE)
                        {
                            Eat(token.type);
                            Type v = new Type(token);
                            nd.args.Add(v);
                        }
                        else if (token.type == Token.Type.NAME)
                        {
                            Eat(token.type);
                            Name v = new Name(token);
                            nd.args.Add(v);
                        }
                        else if (token.type == Token.Type.FACTION)
                        {
                            Eat(token.type);
                            Faction v = new Faction(token);
                            nd.args.Add(v);
                        }
                        else if (token.type == Token.Type.POWER)
                        {
                            Eat(token.type);
                            PowerAsField v = new PowerAsField();
                            nd.args.Add(v);
                        }
                        else if (token.type == Token.Type.RANGE)
                        {
                            Eat(token.type);
                            Range v = new Range(token);
                            nd.args.Add(v);
                        }
                        else if (token.type == Token.Type.POINTER)
                        {
                            Eat(token.type);
                            Pointer v = new Pointer(token);
                            nd.args.Add(v);
                        }
                        else
                        {
                            Error();
                            Eat(currentToken.type);
                        }
                    }
                }
                node = nd;
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Assign AssignmentStatement(Var variable)
    {
        try
        {
            Var left = variable;
            Token token = currentToken;
            Eat(Token.Type.ASSIGN);
            AST right = Expression();
            Assign node = new Assign(left, token, right);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Function FunctionStatement(string name)
    {
        try
        {
            Args args = new Args();
            Eat(Token.Type.FUNCTION);
            Eat(Token.Type.L_PARENTHESIS);

            while (currentToken.type != Token.Type.R_PARENTHESIS && currentToken.type != Token.Type.EOF)
            {
                AST currentArg = Expression();
                args.Add(currentArg);

                if (currentToken.type != Token.Type.R_PARENTHESIS)
                    Eat(Token.Type.COMA);
            }
            Eat(Token.Type.R_PARENTHESIS);

            Function node = new Function(name, args);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public ForLoop ForLoopStatement()
    {
        try
        {
            Eat(Token.Type.FOR);
            Var target = Variable();
            Eat(Token.Type.IN);
            Var targets = Variable();
            Compound body = CompoundStatement();

            ForLoop node = new ForLoop(target, targets, body);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public WhileLoop WhileLoopStatement()
    {
        try
        {
            Eat(Token.Type.WHILE);
            Eat(Token.Type.L_PARENTHESIS);
            AST condition = BooleanExpression();
            Eat(Token.Type.R_PARENTHESIS);
            Compound body = CompoundStatement();
            WhileLoop node = new WhileLoop(condition, body);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public IfNode IfNodeStatement()
    {
        try
        {
            Eat(Token.Type.IF);
            Eat(Token.Type.L_PARENTHESIS);
            AST condition = BooleanExpression();
            Eat(Token.Type.R_PARENTHESIS);
            Compound body = CompoundStatement();
            IfNode node = new IfNode(condition, body);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Statement() // FIX THIS 
    {
        try
        {
            if (currentToken.type == Token.Type.WHILE)
            {
                return WhileLoopStatement();
            }

            if (currentToken.type == Token.Type.IF)
            {
                return IfNodeStatement();
            }

            if (currentToken.type == Token.Type.FOR)
            {
                return ForLoopStatement();
            }

            if (currentToken.type == Token.Type.FUNCTION)
            {
                Function node = FunctionStatement(currentToken.value);
                return node;
            }

            if (currentToken.type == Token.Type.ID) // And this...
            {
                Var variable = Variable();

                if (variable.GetType() == typeof(VarComp) && currentToken.type == Token.Type.SEMI) // THIS
                {
                    VarComp v = variable as VarComp;
                    if (v.args[v.args.Count - 1].GetType() == typeof(Function))
                    {
                        Function f = v.args[v.args.Count - 1] as Function;
                        if (f.type != Var.Type.VOID) Error();
                    }
                    else Error();

                    return variable as VarComp;
                }
                else if (currentToken.type == Token.Type.ASSIGN)
                {
                    Assign node = AssignmentStatement(variable);
                    return node;
                }

                return new NoOp();
            }

            Error();
            return new NoOp();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public List<AST> StatementList()
    {
        try
        {
            List<AST> results = new List<AST>();

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                AST node = Statement();
                results.Add(node);
                Eat(Token.Type.SEMI);
            }

            return results;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Compound CompoundStatement()
    {
        try
        {
            Eat(Token.Type.L_BRACKET);
            List<AST> nodes = StatementList();
            Eat(Token.Type.R_BRACKET);

            Compound root = new Compound();
            for (int i = 0; i < nodes.Count; i++)
            {
                root.children.Add(nodes[i]);
            }

            return root;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Type TypeParse()
    {
        try
        {
            Eat(Token.Type.TYPE);
            Eat(Token.Type.COLON);
            Type node = new Type(currentToken);
            Eat(Token.Type.STRING);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Faction FactionParse()
    {
        try
        {
            Eat(Token.Type.FACTION);
            Eat(Token.Type.COLON);
            Faction node = new Faction(currentToken);
            Eat(Token.Type.STRING);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Power PowerParse()
    {
        try
        {
            Eat(Token.Type.POWER);
            Eat(Token.Type.COLON);
            Power node = new Power(currentToken);
            Eat(Token.Type.INT);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Range RangeParse()
    {
        try
        {
            Eat(Token.Type.RANGE);
            Eat(Token.Type.COLON);
            Eat(Token.Type.L_SQ_BRACKET);
            Range node = new Range(currentToken);
            Eat(Token.Type.STRING);
            Eat(Token.Type.R_SQ_BRACKET);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public EffectOnActivation EffectOnActivationParse()
    {
        try
        {
            Eat(Token.Type.OA_EFFECT);
            Eat(Token.Type.COLON);
            Eat(Token.Type.L_BRACKET);

            Name name = null;
            Args parameters = new Args();

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                if (currentToken.type == Token.Type.NAME)
                {
                    if (name == null)
                    {
                        name = NameParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.ID)
                {
                    Var variable = Variable();
                    Token token = currentToken;
                    Eat(Token.Type.COLON);
                    AST value = Expression();
                    Assign param = new Assign(variable, token, value);
                    parameters.Add(param);
                    if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                }
                else { Error(); Eat(currentToken.type); }
            }
            Eat(Token.Type.R_BRACKET);

            if (name == null) Error();

            EffectOnActivation node;
            if (parameters.args.Count == 0)
                node = new EffectOnActivation(name);
            else node = new EffectOnActivation(name, parameters);

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Source SourceParse()
    {
        try
        {
            Eat(Token.Type.SOURCE);
            Eat(Token.Type.COLON);

            Token token = currentToken;
            Eat(Token.Type.STRING);

            Source node = new Source(token);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Single SingleParse()
    {
        try
        {
            Eat(Token.Type.SINGLE);
            Eat(Token.Type.COLON);

            Token token = currentToken;
            Eat(Token.Type.BOOL);
            Single node = new Single(token);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Predicate PredicateParse()
    {
        try
        {
            Eat(Token.Type.PREDICATE);
            Eat(Token.Type.COLON);

            Eat(Token.Type.L_PARENTHESIS);
            Var unit = Variable();
            unit.type = Var.Type.CARD;
            Eat(Token.Type.R_PARENTHESIS);

            Eat(Token.Type.ARROW);

            Eat(Token.Type.L_PARENTHESIS);
            AST condition = BooleanExpression();
            Eat(Token.Type.R_PARENTHESIS);

            Predicate node = new Predicate(unit, condition);

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Selector SelectorParse()
    {
        try
        {
            Eat(Token.Type.SELECTOR);
            Eat(Token.Type.COLON);
            Eat(Token.Type.L_BRACKET);

            Source source = null;
            Single single = null;
            Predicate predicate = null;

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                if (currentToken.type == Token.Type.SOURCE)
                {
                    if (source == null)
                    {
                        source = SourceParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.SINGLE)
                {
                    if (single == null)
                    {
                        single = SingleParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.PREDICATE)
                {
                    if (predicate == null)
                    {
                        predicate = PredicateParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else { Error(); Eat(currentToken.type); }
            }
            Eat(Token.Type.R_BRACKET);

            if (source == null || predicate == null) Error();

            Selector node;
            if (single == null) node = new Selector(source, predicate);
            else node = new Selector(source, single, predicate);

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public PostAction PostActionParse()
    {
        try
        {
            Eat(Token.Type.POSTACTION);
            Eat(Token.Type.COLON);

            Eat(Token.Type.L_BRACKET);

            Type type = null;
            Selector selector = null;

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                if (currentToken.type == Token.Type.TYPE)
                {
                    if (type == null)
                    {
                        type = TypeParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.SELECTOR)
                {
                    if (selector == null)
                    {
                        selector = SelectorParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else Error();
            }
            Eat(Token.Type.R_BRACKET);

            if (type == null) Error();

            PostAction node;
            if (selector == null) node = new PostAction(type);
            else node = new PostAction(type, selector);

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public OnActivationElement OnActivationElementParse()
    {
        try
        {
            Eat(Token.Type.L_BRACKET);

            EffectOnActivation effectOnActivation = null;
            Selector selector = null;
            PostAction postAction = null;

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                if (currentToken.type == Token.Type.OA_EFFECT)
                {
                    if (effectOnActivation == null)
                    {
                        effectOnActivation = EffectOnActivationParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.SELECTOR)
                {
                    if (selector == null)
                    {
                        selector = SelectorParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.POSTACTION)
                {
                    if (postAction == null)
                    {
                        postAction = PostActionParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else { Error(); Eat(currentToken.type); }
            }
            Eat(Token.Type.R_BRACKET);


            if (effectOnActivation == null) Error();

            OnActivationElement node;
            if (selector == null && postAction == null) node = new OnActivationElement(effectOnActivation);
            else if (postAction == null) node = new OnActivationElement(effectOnActivation, selector);
            else if (selector == null) node = new OnActivationElement(effectOnActivation, postAction);
            else node = new OnActivationElement(effectOnActivation, selector, postAction);

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public List<OnActivationElement> OnActivationList()
    {
        try
        {
            List<OnActivationElement> nodes = new List<OnActivationElement>();

            while (currentToken.type != Token.Type.R_SQ_BRACKET && currentToken.type != Token.Type.EOF)
            {
                if (currentToken.type == Token.Type.L_BRACKET)
                {
                    OnActivationElement node = OnActivationElementParse();
                    nodes.Add(node);
                    if (currentToken.type != Token.Type.R_SQ_BRACKET) Eat(Token.Type.COMA);
                }
                else Error();
            }

            return nodes;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public OnActivation OnActivationParse()
    {
        try
        {
            Eat(Token.Type.ONACTIVATION);
            Eat(Token.Type.COLON);
            Eat(Token.Type.L_SQ_BRACKET);
            List<OnActivationElement> list = OnActivationList();
            Eat(Token.Type.R_SQ_BRACKET);

            OnActivation node = new OnActivation();
            for (int i = 0; i < list.Count; i++)
            {
                node.onActivation.Add(list[i]);
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public CardNode CardCreation()
    {
        try
        {
            Eat(Token.Type.CARD);
            Eat(Token.Type.L_BRACKET);

            Name name = null;
            Type type = null;
            Faction faction = null;
            Power power = null;
            Range range = null;
            OnActivation onActivation = null;

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                if (currentToken.type == Token.Type.NAME)
                {
                    if (name == null)
                    {
                        name = NameParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.TYPE)
                {
                    if (type == null)
                    {
                        type = TypeParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.FACTION)
                {
                    if (faction == null)
                    {
                        faction = FactionParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.POWER)
                {
                    if (power == null)
                    {
                        power = PowerParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.RANGE)
                {
                    if (range == null)
                    {
                        range = RangeParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.ONACTIVATION)
                {
                    if (onActivation == null)
                    {
                        onActivation = OnActivationParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else { Error();  Eat(currentToken.type); }
            }
            Eat(Token.Type.R_BRACKET);

            List<AST> listOfParameters = new List<AST> { name, type, faction, power, range, onActivation };
            foreach (AST child in listOfParameters)
            {
                if (child == null) Error();
            }

            CardNode node = new CardNode(name, type, faction, power, range, onActivation);

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Name NameParse()
    {
        try
        {
            Eat(Token.Type.NAME);
            Eat(Token.Type.COLON);
            Name node = new Name(currentToken);
            Eat(Token.Type.STRING);
            return node;

        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Args GetParametersInParams()
    {
        try
        {
            Args args = new Args();

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                Var variable = Variable();
                Eat(Token.Type.COLON);
                if (currentToken.type == Token.Type.D_INT ||
                    currentToken.type == Token.Type.D_STRING ||
                    currentToken.type == Token.Type.D_BOOL)
                {
                    variable.TypeInParams(currentToken.type);
                    args.Add(variable);
                    Eat(currentToken.type);
                    if (currentToken.type != Token.Type.R_BRACKET)
                    {
                        Eat(Token.Type.COMA);
                    }
                }
                else
                {
                    Error();
                    return args;
                }
            }
            return args;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Args ParamsEffectParse()
    {
        try
        {
            Eat(Token.Type.PARAMS);
            Eat(Token.Type.COLON);
            Eat(Token.Type.L_BRACKET);
            Args node = GetParametersInParams();
            Eat(Token.Type.R_BRACKET);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Action ActionParse()
    {
        try
        {
            Eat(Token.Type.ACTION);
            Eat(Token.Type.COLON);

            Eat(Token.Type.L_PARENTHESIS);

            Var targets = Variable();
            targets.type = Var.Type.TARGETS;

            Eat(Token.Type.COMA);

            Var context = Variable();
            context.type = Var.Type.CONTEXT;

            Eat(Token.Type.R_PARENTHESIS);
            Eat(Token.Type.ARROW);

            Compound body = CompoundStatement();

            Action node = new Action(targets, context, body);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public EffectNode EffectCreation()
    {
        try
        {
            Eat(Token.Type.EFFECT);
            Eat(Token.Type.L_BRACKET);


            Name name = null;
            Args parameters = null;
            Action action = null;

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                if (currentToken.type == Token.Type.NAME)
                {
                    if (name == null)
                    {
                        name = NameParse();
                        if (currentToken.type != Token.Type.R_BRACKET)
                            Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.PARAMS)
                {
                    if (parameters == null)
                    {
                        parameters = ParamsEffectParse();
                        if (currentToken.type != Token.Type.R_BRACKET)
                            Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else if (currentToken.type == Token.Type.ACTION)
                {
                    if (action == null)
                    {
                        action = ActionParse();
                        if (currentToken.type != Token.Type.R_BRACKET)
                            Eat(Token.Type.COMA);
                    }
                    else Error();
                }
                else { Error(); Eat(currentToken.type); }
            }

            Eat(Token.Type.R_BRACKET);

            if (name == null || action == null)
            {
                Error();
            }

            EffectNode node;
            if (parameters == null)
            {
                node = new EffectNode(name, action);
            }
            else
            {
                node = new EffectNode(name, parameters, action);
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public List<AST> ListOfCardAndEffect()
    {
        try
        {
            List<AST> listOfCardAndEffect = new List<AST>();

            while (currentToken.type != Token.Type.EOF)
            {
                if (currentToken.type == Token.Type.CARD)
                {
                    CardNode node = CardCreation();
                    listOfCardAndEffect.Add(node);
                }
                else if (currentToken.type == Token.Type.EFFECT)
                {
                    EffectNode node = EffectCreation();
                    listOfCardAndEffect.Add(node);
                }
                else 
                {
                    Error();
                    Eat(currentToken.type);
                }
            }

            return listOfCardAndEffect;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Compound Program()
    {
        try
        {
            List<AST> programList = ListOfCardAndEffect();

            Compound program = new Compound();

            foreach (AST node in programList)
            {
                program.children.Add(node);
            }

            return program;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Parse()
    {
        try
        {
            Compound node = Program();
            if (currentToken.type != Token.Type.EOF)
            {
                Error();
            }

            node.Print("");

            return node;
        }
        catch (System.Exception)
        {
            AST node = new NoOp();
            return node;
        }
    }
}