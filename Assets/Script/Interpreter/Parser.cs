using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parser
{
    public Lexer lexer;
    public Token currentToken;
    public string curError;
    public Dictionary<string, ASTType.Type> GLOBAL_SCOPE;

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
        currentToken = lexer.GetNextToken();
        curError = "";
        GLOBAL_SCOPE = new Dictionary<string, ASTType.Type>();
    }

    public bool IsInScope(Var variable)
    {
        return GLOBAL_SCOPE.ContainsKey(variable.value);
    }

    public void Error(string errorTag)
    {
        curError = $"Invalid syntax in    Ln {lexer.row}    Col {lexer.column}\n \t {errorTag} \n";
        curError += "Current token type: " + currentToken.type + "\n";
        curError += "Current token value: " + currentToken.value + "\n";
        curError += "Code:\n";
        curError += (lexer.text.Substring(0, lexer.pos)) + "\n";
        curError += "-----------------------------ERROR-----------------------------\n";
        curError += (lexer.text.Substring(lexer.pos)) + "\n";
        Debug.Log(curError);
    }

    public void ErrorRepeat()
    {
        Error($"Found invalid repetition of field {currentToken.type} in current context");
        Eat(currentToken.type);
    }

    public void ErrorNotCorrespondingField()
    {
        Error($"Invalid syntax in current context of {currentToken.type.ToString()} token");
        Eat(currentToken.type);
    }

    public void ErrorInNodeCreation(AST node)
    {
        Error($"Invalid construction of {node.GetType().ToString()} maybe you miss fields of {node.GetType().ToString()}");
    }

    public void ErrorInUnaryOp(UnaryOp node)
    {
        Error($"Unvalid use of Unary Operator ({node.operation.value}) in {node.expression.type}  cannot convert {node.expression.type} to {node.type}");
    }

    public void ErrorInBinOp(BinOp node)
    {
        Error($"Invalid Binary Operator: Operator '{node.op.value}' cannot be applied to operands of type '{node.left.type}' and '{node.right.type}'");
    }

    public void ErrorHasNotBeenDeclared(Var variable)
    {
        Error($"Variable '{variable.value}' has not been declared");
    }

    public void ErrorInAssignment(Assign assign)
    {
        Error($"Invalid Assignment, cannot convert {assign.left.type}  to  {assign.right.type}");
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

    public bool IsPossibleUnaryOp(UnaryOp node)
    {
        return (node.type == node.expression.type);
    }

    public bool IsPossibleBinOp(BinOp node)
    {
        if (node.left.type == node.right.type)
        {
            if (node.type == ASTType.Type.BOOL)
            {
                if (node.op.type == Token.Type.EQUAL || node.op.type == Token.Type.DIFFER) return true;
                else return (node.left.type == ASTType.Type.INT);
            }
            else return (node.left.type == node.type);
        }
        
        return false;
    }

    public ASTType Factor()
    {
        try
        {
            Token token = currentToken;
            if (token.type == Token.Type.PLUS || token.type == Token.Type.MINUS)
            {
                Eat(token.type);
                ASTType factor = Factor();
                UnaryOp node = new UnaryOp(token, factor);
                if (!IsPossibleUnaryOp(node)) ErrorInUnaryOp(node);
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
                ASTType result = Expression();
                Eat(Token.Type.R_PARENTHESIS);
                return result;
            }
            if (currentToken.type == Token.Type.ID)
            {
                Var node = Variable();
                if (node.GetType() == typeof(Var) && IsInScope(node)) ErrorHasNotBeenDeclared(node);
                token = currentToken;
                if (token.type == Token.Type.PLUSPLUS || token.type == Token.Type.MINUSMINUS)
                {
                    UnaryOp unode = new UnaryOp(token, node);
                    if (node.type != ASTType.Type.INT) ErrorInUnaryOp(unode);
                    Eat(token.type);
                    return unode;
                }
                return node;
            }

            // FIND NODE FOR FIND FUNCTION

            if (currentToken.type == Token.Type.FUNCTION)
            {
                return FunctionStatement(currentToken.value);
            }
            Error($"Invalid Factor: {token.value}");
            return new NoOp();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public ASTType Term()
    {
        try
        {
            ASTType node = Factor();
            Token token = currentToken;
            if (token.type == Token.Type.MULT || token.type == Token.Type.DIVIDE || token.type == Token.Type.MOD)
            {
                Eat(token.type);
                node = new BinOp(node, token, Expression());
                if (!IsPossibleBinOp(node as BinOp)) ErrorInBinOp(node as BinOp);
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public ASTType Expression()
    {
        try
        {
            ASTType node = Term();
            Token token = currentToken;
            if (token.type == Token.Type.PLUS || token.type == Token.Type.MINUS ||
                token.type == Token.Type.STRING_SUM || token.type == Token.Type.STRING_SUM_S)
            {
                Eat(token.type);
                node = new BinOp(node, token, Expression());
                if (!IsPossibleBinOp(node as BinOp)) ErrorInBinOp(node as BinOp);
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public ASTType BooleanFactor()
    {
        try
        {
            Token token = currentToken;
            if (token.type == Token.Type.NOT)
            {
                Eat(Token.Type.NOT);
                Eat(Token.Type.L_PARENTHESIS);
                UnaryOp unaryOp = new UnaryOp(token, BooleanExpression());
                if (!IsPossibleUnaryOp(unaryOp)) ErrorInUnaryOp(unaryOp);
                Eat(Token.Type.R_PARENTHESIS);
                return unaryOp;
            }
            if (token.type == Token.Type.L_PARENTHESIS)
            {
                Eat(Token.Type.L_PARENTHESIS);
                ASTType result = BooleanExpression();
                Eat(Token.Type.R_PARENTHESIS);
                return result;
            }

            ASTType left = Expression();

            token = currentToken;
            if (token.type == Token.Type.EQUAL) Eat(Token.Type.EQUAL);
            else if (token.type == Token.Type.DIFFER) Eat(Token.Type.DIFFER);
            else if (token.type == Token.Type.GREATER_E) Eat(Token.Type.GREATER_E);
            else if (token.type == Token.Type.LESS_E) Eat(Token.Type.LESS_E);
            else if (token.type == Token.Type.LESS) Eat(Token.Type.LESS);
            else if (token.type == Token.Type.GREATER) Eat(Token.Type.GREATER);
            else if (token.type == Token.Type.R_PARENTHESIS) return left;
            else Error($"Invalid Boolean operator: '{token.value}'");

            ASTType right = Expression();

            BinOp node = new BinOp(left, token, right);
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public ASTType BooleanTerm()
    {
        try
        {
            ASTType node = BooleanFactor();
            Token token = currentToken;

            if (token.type == Token.Type.OR)
            {
                Eat(Token.Type.OR);
                node = new BinOp(node, token, BooleanExpression());
                if (!IsPossibleBinOp(node as BinOp)) ErrorInBinOp(node as BinOp);
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public ASTType BooleanExpression()
    {
        try
        {
            ASTType node = BooleanTerm();
            Token token = currentToken;

            if (token.type == Token.Type.AND)
            {
                Eat(Token.Type.AND);
                node = new BinOp(node, token, BooleanExpression());
                if (!IsPossibleBinOp(node as BinOp)) ErrorInBinOp(node as BinOp);
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public bool IsValidIndexer(Indexer node)
    {
        return (node.index.type == ASTType.Type.INT);
    }

    public Indexer IndexerParse()
    {
        try
        {
            Eat(Token.Type.L_SQ_BRACKET);
            ASTType index = Expression();
            Eat(Token.Type.R_SQ_BRACKET);
            Indexer node = new Indexer(index);
            if (!IsValidIndexer(node)) Error("Invalid indexer: Expression must be type 'INT'");
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Var Variable() 
    {
        try
        {
            Var node = new Var(currentToken);
            Eat(Token.Type.ID);
            VarComp nd = new VarComp(node.token);
            if (currentToken.type == Token.Type.DOT || currentToken.type == Token.Type.L_SQ_BRACKET)
            {
                while (currentToken.type == Token.Type.DOT && currentToken.type != Token.Type.EOF)
                {
                    Eat(Token.Type.DOT);
                    if (currentToken.type == Token.Type.FUNCTION)
                    {
                        Function f = FunctionStatement(currentToken.value);
                        nd.args.Add(f);
                        if (currentToken.type == Token.Type.L_SQ_BRACKET)
                        {
                            nd.args.Add(IndexerParse());
                        }
                    }
                    else
                    {
                        Token token = currentToken;
                        if (token.type == Token.Type.TYPE || token.type == Token.Type.NAME || token.type == Token.Type.FACTION
                            || token.type == Token.Type.POWER || token.type == Token.Type.RANGE)
                        {
                            Eat(token.type);
                            Var v = new Var(token, ASTType.Type.STRING);
                            if (token.type == Token.Type.POWER) v.type = ASTType.Type.INT;
                            nd.args.Add(v);
                        }
                        else if (token.type == Token.Type.POINTER)
                        {
                            Eat(token.type);
                            Pointer pointer = new Pointer(token);
                            nd.args.Add(pointer);
                            if (currentToken.type == Token.Type.L_SQ_BRACKET)
                            {
                                nd.args.Add(IndexerParse());
                            }
                        }
                        else
                        {
                            Error($"Invalid Field: '{currentToken.value}'");
                            Eat(currentToken.type);
                        }
                    }
                }
                node = nd;
            }
            else if (currentToken.type == Token.Type.L_SQ_BRACKET)
            {
                Eat(Token.Type.L_SQ_BRACKET);
                ASTType index = Expression();
                Eat(Token.Type.R_SQ_BRACKET);
                Indexer indexer = new Indexer(index);
                nd.args.Add(indexer);
                node = nd;
            }

            if (node.GetType() == typeof(Var))
            {
                if (IsInScope(node)) node.type = GLOBAL_SCOPE[node.value];
            }
            else if (!IsInScope(node)) ErrorHasNotBeenDeclared(node);
            else
            {
                VarComp vc = node as VarComp;
                if (IsPossibleVarComp(vc))
                    vc.type = vc.args[vc.args.Count - 1].type;
            }

            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public bool IsPossibleVarComp(VarComp v)
    {
        for (int i = 0; i < v.args.Count; i++)
        {
            if (i == 0)
            {
                if (!InternalIsPossibleVarComp(GLOBAL_SCOPE[v.value], v.args[i])) return false;
            }
            else if (!InternalIsPossibleVarComp(v.args[i - 1].type, v.args[i])) return false;
        }
        return true;
    }

    public bool IsFunction(ASTType node)
    {
        return (node.GetType() == typeof(Function));
    }

    public bool InternalIsPossibleVarComp(ASTType.Type fatherType, ASTType v)
    {
        if (fatherType == ASTType.Type.CONTEXT)
        {
            if (v.GetType() == typeof(Pointer))
            {
                Pointer p = v as Pointer;
                string s = p.pointer;
                return (s == "Hand" || s == "Graveyard" || s == "Deck" || s == "Melee" || s == "Range" || s == "Siege");
            }
            else if (IsFunction(v)) return (v.type == ASTType.Type.CONTEXT || v.type == ASTType.Type.FIELD);
        }

        if (fatherType == ASTType.Type.FIELD)
        {
            if (v.GetType() == typeof(Indexer)) return true;
            else if (IsFunction(v)) return (v.type == ASTType.Type.VOID || v.type == ASTType.Type.CARD);
        }

        if (fatherType == ASTType.Type.CARD && v.GetType() == typeof(Var))
        {
            Var vv = v as Var;
            string s = vv.value;
            return (s == "Type" || s == "Name" || s == "Faction" || s == "Range" || s == "Power");
        }

        Error($"Invalid VarComp construction: '{v.ToString()}' is not a field of '{fatherType.ToString()}'");

        return false;
    }

    public Assign AssignmentStatement(Var variable)
    {
        try
        {
            Var left = variable;
            Token token = currentToken;
            Eat(Token.Type.ASSIGN);
            ASTType right = Expression();
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

    public void ErrorInvalidIDStatement()
    {
        Error("Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement");
    }

    public AST Statement() 
    {
        try
        {
            if (currentToken.type == Token.Type.SEMI)
            {
                return new NoOp();
            }

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

            if (currentToken.type == Token.Type.ID)
            {
                Var variable = Variable();
                if (variable.GetType() == typeof(VarComp) && currentToken.type == Token.Type.SEMI)
                {
                    VarComp v = variable as VarComp;
                    int count = v.args.Count - 1;
                    if (v.args[count].GetType() == typeof(Function))
                    {
                        Function f = v.args[count] as Function;
                        if (f.type != Var.Type.VOID) ErrorInvalidIDStatement();
                    }
                    else ErrorInvalidIDStatement();

                    return variable;
                }
                else if (currentToken.type == Token.Type.ASSIGN)
                {
                    Assign node = AssignmentStatement(variable);
                    if (variable.GetType() == typeof(Var))
                    {
                        if (!IsInScope(variable))
                        {
                            variable.type = node.right.type;
                            GLOBAL_SCOPE[variable.value] = variable.type;
                        }
                        else if (variable.type != node.right.type) ErrorInAssignment(node);
                    }
                    else
                    {
                        if (!IsInScope(variable)) ErrorHasNotBeenDeclared(variable);
                        else if (variable.type != node.right.type) ErrorInAssignment(node); 
                    }
                    
                    return node;
                }
                else if (currentToken.type == Token.Type.MINUSMINUS || currentToken.type == Token.Type.PLUSPLUS)
                {
                    UnaryOp node = new UnaryOp(currentToken, variable);
                    if (variable.type != ASTType.Type.INT) ErrorInUnaryOp(node); 
                    Eat(currentToken.type);
                    return node;
                }

                Error($"Invalid Statement: token {currentToken.type}");
                return new NoOp();
            }

            Error($"Invalid Statement: token {currentToken.type}");
            Eat(currentToken.type);
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

    public TypeNode TypeParse()
    {
        try
        {
            Eat(Token.Type.TYPE);
            Eat(Token.Type.COLON);
            TypeNode node = new TypeNode(currentToken);
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
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.ID)
                {
                    Var variable = Variable();
                    Token token = currentToken;
                    Eat(Token.Type.COLON);
                    ASTType value = Expression();
                    Assign param = new Assign(variable, token, value);
                    parameters.Add(param);
                    if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                }
                else ErrorNotCorrespondingField();
            }
            Eat(Token.Type.R_BRACKET);

            EffectOnActivation node;
            if (parameters.args.Count == 0)
                node = new EffectOnActivation(name);
            else node = new EffectOnActivation(name, parameters);

            if (name == null) ErrorInNodeCreation(node);

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
            unit.type = ASTType.Type.CARD;
            if (IsInScope(unit) || unit.GetType() == typeof(VarComp)) ErrorUnvalidAssignment(unit);
            Eat(Token.Type.R_PARENTHESIS);

            Eat(Token.Type.ARROW);

            Eat(Token.Type.L_PARENTHESIS);
            ASTType condition = BooleanExpression();
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
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.SINGLE)
                {
                    if (single == null)
                    {
                        single = SingleParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.PREDICATE)
                {
                    if (predicate == null)
                    {
                        predicate = PredicateParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else ErrorNotCorrespondingField();
            }
            Eat(Token.Type.R_BRACKET);

            Selector node;
            if (single == null) node = new Selector(source, predicate);
            else node = new Selector(source, single, predicate);

            if (source == null || predicate == null) ErrorInNodeCreation(node);

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

            EffectOnActivation effectOnActivation = null;
            Selector selector = null;

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                if (currentToken.type == Token.Type.OA_EFFECT)
                {
                    if (effectOnActivation == null)
                    {
                        effectOnActivation = EffectOnActivationParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.SELECTOR)
                {
                    if (selector == null)
                    {
                        selector = SelectorParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else ErrorNotCorrespondingField();
            }
            Eat(Token.Type.R_BRACKET);

            PostAction node;
            if (selector == null) node = new PostAction(effectOnActivation);
            else node = new PostAction(effectOnActivation, selector);

            if (effectOnActivation == null) ErrorInNodeCreation(node);

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
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.SELECTOR)
                {
                    if (selector == null)
                    {
                        selector = SelectorParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.POSTACTION)
                {
                    if (postAction == null)
                    {
                        postAction = PostActionParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else ErrorNotCorrespondingField();
            }
            Eat(Token.Type.R_BRACKET);

            OnActivationElement node;
            if (selector == null && postAction == null) node = new OnActivationElement(effectOnActivation);
            else if (postAction == null) node = new OnActivationElement(effectOnActivation, selector);
            else if (selector == null) node = new OnActivationElement(effectOnActivation, postAction);
            else node = new OnActivationElement(effectOnActivation, selector, postAction);

            if (effectOnActivation == null) ErrorInNodeCreation(node);

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
                else ErrorNotCorrespondingField();
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
            TypeNode type = null;
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
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.TYPE)
                {
                    if (type == null)
                    {
                        type = TypeParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.FACTION)
                {
                    if (faction == null)
                    {
                        faction = FactionParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.POWER)
                {
                    if (power == null)
                    {
                        power = PowerParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.RANGE)
                {
                    if (range == null)
                    {
                        range = RangeParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.ONACTIVATION)
                {
                    if (onActivation == null)
                    {
                        onActivation = OnActivationParse();
                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else ErrorNotCorrespondingField();
            }
            Eat(Token.Type.R_BRACKET);

            CardNode node = new CardNode(name, type, faction, power, range, onActivation);

            List<AST> listOfParameters = new List<AST> { name, type, faction, power, range, onActivation };
            foreach (AST child in listOfParameters)
                if (child == null) { ErrorInNodeCreation(node); break; }

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
            if (node.name == "") Error("Name must not be an empty string");
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
                if (variable.GetType() == typeof(VarComp)) Error("Invalid declaration of param");
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
                    Error($"Invalid Type '{currentToken.value}' found in current context\n Expecting 'STRING', 'BOOL', 'Number'");
                    Eat(currentToken.type);
                    if (currentToken.type == Token.Type.COMA) Eat(currentToken.type);
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

    public void ErrorUnvalidAssignment(Var variable)
    {
        Error($"Unvalid Assignment of '{variable.value}'");
    }

    public Action ActionParse()
    {
        try
        {
            Eat(Token.Type.ACTION);
            Eat(Token.Type.COLON);

            Eat(Token.Type.L_PARENTHESIS);

            Var targets = Variable();
            targets.type = ASTType.Type.FIELD;
            if (IsInScope(targets) || targets.GetType() == typeof(VarComp)) ErrorUnvalidAssignment(targets);

            Eat(Token.Type.COMA);

            Var context = Variable();
            context.type = ASTType.Type.CONTEXT;
            if (IsInScope(context) || context.GetType() == typeof(VarComp)) ErrorUnvalidAssignment(context);
            

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
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.PARAMS)
                {
                    if (parameters == null)
                    {
                        parameters = ParamsEffectParse();
                        if (currentToken.type != Token.Type.R_BRACKET)
                            Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.ACTION)
                {
                    if (action == null)
                    {
                        action = ActionParse();
                        if (currentToken.type != Token.Type.R_BRACKET)
                            Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else ErrorNotCorrespondingField();
            }

            Eat(Token.Type.R_BRACKET);

            EffectNode node;
            if (parameters == null)
            {
                node = new EffectNode(name, action);
            }
            else
            {
                node = new EffectNode(name, parameters, action);
            }

            if (name == null || action == null)
            {
                ErrorInNodeCreation(node);
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
                    Error($"Expecting (card) or (effect) token, found: {currentToken.type.ToString()}");
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
                Error("Cannot parse all of text");
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