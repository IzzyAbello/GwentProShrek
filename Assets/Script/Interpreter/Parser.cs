using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parser
{
    public Lexer lexer;
    public Token currentToken;
    public string curError;
    public Scope GLOBAL_SCOPE;
    public Dictionary<string, EffectNode> EFFECT_LIST;

    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
        currentToken = lexer.GetNextToken();
        curError = "";
        GLOBAL_SCOPE = new Scope();
        EFFECT_LIST = new Dictionary<string, EffectNode>();
    }

    public void Error(string errorTag)
    {
        curError = $"Invalid syntax in Ln {lexer.row}  Col {lexer.column}\n {errorTag} \n";
        curError += "Current token type: " + currentToken.type + "\n";
        curError += "Current token value: " + currentToken.value + "\n";
        curError += "Code:\n";
        curError += (lexer.text.Substring(0, lexer.pos)) + "\n";
        curError += "-----------------------------ERROR-----------------------------\n";
        curError += (lexer.text.Substring(lexer.pos)) + "\n";
        Debug.Log(curError);
    }

    public void CrashLog(int line)
    {
        Debug.Log("Crash in line: " + line.ToString());
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
                if (node.left.type == ASTType.Type.BOOL && (node.op.type == Token.Type.OR || node.op.type == Token.Type.AND))
                    return true;
                if (node.op.type == Token.Type.EQUAL || node.op.type == Token.Type.DIFFER) return true;
                else return (node.left.type == ASTType.Type.INT);
            }
            else return (node.left.type == node.type);
        }

        return false;
    }

    public ASTType Factor(Scope scope)
    {
        try
        {
            Token token = currentToken;
            if (token.type == Token.Type.PLUS || token.type == Token.Type.MINUS)
            {
                Eat(token.type);
                ASTType factor = Factor(scope);
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
                ASTType result = Expression(scope);
                Eat(Token.Type.R_PARENTHESIS);
                return result;
            }
            if (currentToken.type == Token.Type.ID)
            {
                Var node = Variable(scope);
                if (node.GetType() == typeof(Var) && !scope.IsInScope(node)) ErrorHasNotBeenDeclared(node);
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

            if (currentToken.type == Token.Type.FUNCTION)
            {
                return FunctionStatement(currentToken.value, scope);
            }


            Error($"Invalid Factor: {token.value}");
            return new NoOp();
        }
        catch
        {
            CrashLog(188);
            throw;
        }
    }

    public ASTType Term(Scope scope)
    {
        try
        {
            ASTType node = Factor(scope);
            Token token = currentToken;
            if (token.type == Token.Type.MULT || token.type == Token.Type.DIVIDE || token.type == Token.Type.MOD)
            {
                Eat(token.type);
                node = new BinOp(node, token, Expression(scope));
                if (!IsPossibleBinOp(node as BinOp)) ErrorInBinOp(node as BinOp);
            }
            return node;
        }
        catch
        {
            CrashLog(209);
            throw;
        }
    }

    public ASTType Expression(Scope scope)
    {
        try
        {
            ASTType node = Term(scope);
            Token token = currentToken;
            if (token.type == Token.Type.PLUS || token.type == Token.Type.MINUS ||
                token.type == Token.Type.STRING_SUM || token.type == Token.Type.STRING_SUM_S)
            {
                Eat(token.type);
                node = new BinOp(node, token, Expression(scope));
                if (!IsPossibleBinOp(node as BinOp)) ErrorInBinOp(node as BinOp);
            }
            return node;
        }
        catch (System.Exception)
        {
            CrashLog(231);
            throw;
        }
    }

    public ASTType BooleanFactor(Scope scope)
    {
        try
        {
            Token token = currentToken;
            if (token.type == Token.Type.NOT)
            {
                Eat(Token.Type.NOT);
                Eat(Token.Type.L_PARENTHESIS);
                UnaryOp unaryOp = new UnaryOp(token, BooleanExpression(scope));
                if (!IsPossibleUnaryOp(unaryOp)) ErrorInUnaryOp(unaryOp);
                Eat(Token.Type.R_PARENTHESIS);
                return unaryOp;
            }
            if (token.type == Token.Type.L_PARENTHESIS)
            {
                Eat(Token.Type.L_PARENTHESIS);
                ASTType result = BooleanExpression(scope);
                Eat(Token.Type.R_PARENTHESIS);
                return result;
            }

            ASTType left = Expression(scope);

            token = currentToken;
            if (token.type == Token.Type.EQUAL) Eat(Token.Type.EQUAL);
            else if (token.type == Token.Type.DIFFER) Eat(Token.Type.DIFFER);
            else if (token.type == Token.Type.GREATER_E) Eat(Token.Type.GREATER_E);
            else if (token.type == Token.Type.LESS_E) Eat(Token.Type.LESS_E);
            else if (token.type == Token.Type.LESS) Eat(Token.Type.LESS);
            else if (token.type == Token.Type.GREATER) Eat(Token.Type.GREATER);
            else if (token.type == Token.Type.R_PARENTHESIS ||
                token.type == Token.Type.AND || token.type == Token.Type.OR) return left;
            else Error($"Invalid Boolean operator: '{token.value}'");

            ASTType right = Expression(scope);

            BinOp node = new BinOp(left, token, right);
            if (!IsPossibleBinOp(node as BinOp)) ErrorInBinOp(node as BinOp);
            return node;
        }
        catch
        {
            CrashLog(278);
            throw;
        }
    }

    public ASTType BooleanTerm(Scope scope)
    {
        try
        {
            ASTType node = BooleanFactor(scope);
            Token token = currentToken;

            if (token.type == Token.Type.OR)
            {
                Eat(Token.Type.OR);
                node = new BinOp(node, token, BooleanExpression(scope));
                if (!IsPossibleBinOp(node as BinOp)) ErrorInBinOp(node as BinOp);
            }
            return node;
        }
        catch
        {
            CrashLog(300);
            throw;
        }
    }

    public ASTType BooleanExpression(Scope scope)
    {
        try
        {
            ASTType node = BooleanTerm(scope);
            Token token = currentToken;

            if (token.type == Token.Type.AND)
            {
                Eat(Token.Type.AND);
                node = new BinOp(node, token, BooleanExpression(scope));
                if (!IsPossibleBinOp(node as BinOp)) ErrorInBinOp(node as BinOp);
            }
            return node;
        }
        catch
        {
            CrashLog(322);
            throw;
        }
    }

    public bool IsValidIndexer(Indexer node)
    {
        return node.index.type == ASTType.Type.INT;
    }

    public Indexer IndexerParse(Scope scope)
    {
        try
        {
            Eat(Token.Type.L_SQ_BRACKET);
            ASTType index = Expression(scope);
            Eat(Token.Type.R_SQ_BRACKET);
            Indexer node = new Indexer(index);
            if (!IsValidIndexer(node)) Error("Invalid indexer: Expression must be type 'INT'");
            return node;
        }
        catch (System.Exception)
        {
            CrashLog(345);
            throw;
        }
    }

    public Var Variable(Scope scope)
    {
        try
        {
            Var node = new Var(currentToken);
            Eat(Token.Type.ID);
            VarComp nd = new VarComp(node.token);
            if (currentToken.type == Token.Type.DOT || currentToken.type == Token.Type.L_SQ_BRACKET)
            {
                if (currentToken.type == Token.Type.L_SQ_BRACKET)
                {
                    Indexer indexer = IndexerParse(scope);
                    nd.args.Add(indexer);
                }

                while (currentToken.type == Token.Type.DOT && currentToken.type != Token.Type.EOF)
                {
                    Eat(Token.Type.DOT);
                    if (currentToken.type == Token.Type.FUNCTION)
                    {
                        Function f = FunctionStatement(currentToken.value, scope);
                        nd.args.Add(f);
                        if (currentToken.type == Token.Type.L_SQ_BRACKET)
                        {
                            nd.args.Add(IndexerParse(scope));
                        }
                    }
                    else
                    {
                        Token token = currentToken;
                        if (token.type == Token.Type.TYPE || token.type == Token.Type.NAME ||
                            token.type == Token.Type.FACTION || token.type == Token.Type.POWER ||
                            token.type == Token.Type.RANGE)
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
                                nd.args.Add(IndexerParse(scope));
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

            if (node.GetType() == typeof(Var))
            {
                if (scope.IsInScope(node)) node.type = scope.Get(node);
            }
            else if (!scope.IsInScope(node)) ErrorHasNotBeenDeclared(node);
            else
            {
                VarComp vc = node as VarComp;
                if (IsPossibleVarComp(vc, scope))
                {
                    ASTType.Type lastType = vc.args[vc.args.Count - 1].type;
                    vc.type = (lastType == ASTType.Type.INDEXER) ? ASTType.Type.CARD : lastType;
                }
            }

            return node;
        }
        catch
        {
            CrashLog(425);
            throw;
        }
    }

    public bool IsPossibleVarComp(VarComp v, Scope scope)
    {
        for (int i = 0; i < v.args.Count; i++)
        {
            if (i == 0)
            {
                if (!InternalIsPossibleVarComp(scope.Get(v.value), v.args[i])) return false;
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
                return (s == "Hand" || s == "Graveyard" || s == "Deck" ||
                    s == "Melee" || s == "Range" || s == "Siege");
            }
            else if (IsFunction(v)) return (v.type == ASTType.Type.CONTEXT || v.type == ASTType.Type.FIELD);
        }

        if (fatherType == ASTType.Type.FIELD)
        {
            if (v.GetType() == typeof(Indexer)) return true;
            else if (IsFunction(v)) return (v.type == ASTType.Type.FIELD ||
                    v.type == ASTType.Type.VOID || v.type == ASTType.Type.CARD);
        }

        if (fatherType == ASTType.Type.INDEXER || fatherType == ASTType.Type.CARD)
        {
            if (v.GetType() == typeof(Var))
            {
                Var vv = v as Var;
                string s = vv.value;
                return (s == "Type" || s == "Name" || s == "Faction" || s == "Range" || s == "Power" || s == "Owner");
            }
            else if (v.GetType() == typeof(Pointer))
            {
                Pointer p = v as Pointer;
                string s = p.pointer;
                return s == "Owner";
            }
            else return false;
        }

        if (fatherType == ASTType.Type.EFFECT)
        {
            var vv = v as Var;
            string s = vv.value;
            return (s == "Name");
        }

        Error($"Invalid VarComp construction: '{v.ToString()}' is not a valid field of type '{fatherType.ToString()}'");

        return false;
    }

    public Assign AssignmentStatement(Var variable, Scope scope)
    {
        try
        {
            Var left = variable;
            Token token = currentToken;
            Eat(Token.Type.ASSIGN);
            ASTType right = Expression(scope);
            Assign node = new Assign(left, token, right);


            if (token.value != "=")
            {
                if (!scope.IsInScope(variable)) ErrorHasNotBeenDeclared(variable);
                else if ((token.value == "+=" || token.value == "-=" || token.value == "*=" ||
                    token.value == "/=" || token.value == "%=")
                    && (variable.type == node.right.type && variable.type != ASTType.Type.INT))
                    ErrorInAssignment(node);
                else if ((token.value == "@=") &&
                    (variable.type == node.right.type && variable.type != ASTType.Type.STRING))
                    ErrorInAssignment(node);
            }

            if (variable.GetType() == typeof(Var))
            {
                if (!scope.IsInScope(variable))
                {
                    variable.type = node.right.type;
                    scope.Set(variable, variable.type);
                }
                else if (variable.type != node.right.type) ErrorInAssignment(node);
            }
            else
            {
                if (!scope.IsInScope(variable)) ErrorHasNotBeenDeclared(variable);
                else if (variable.type != node.right.type) ErrorInAssignment(node);
            }

            return node;
        }
        catch
        {
            CrashLog(540);
            throw;
        }
    }

    public void ErrorInvalidParameterInFunction(string functionName)
    {
        Error($"Invalid parameter for Function '{functionName}'");
    }

    public Function FindFunction(Scope outScope)
    {
        try
        {
            Scope scope = new Scope(outScope);

            Eat(Token.Type.L_PARENTHESIS);

            Var variable = Variable(scope);
            if (scope.IsInScope(variable) || variable.GetType() == typeof(VarComp)) 
                ErrorInvalidParameterInFunction("Find");
            else
            {
                variable.type = ASTType.Type.CARD;
                scope.Set(variable, variable.type);
            }
            
            Eat(Token.Type.R_PARENTHESIS);

            Eat(Token.Type.ARROW);

            Eat(Token.Type.L_PARENTHESIS);
            ASTType condition = BooleanExpression(scope);
            Eat(Token.Type.R_PARENTHESIS);

            Eat(Token.Type.R_PARENTHESIS);

            Args predicate = new Args();
            predicate.Add(variable);
            predicate.Add(condition);

            Function node = new Function("Find", predicate);
            return node;
        }
        catch
        {
            CrashLog(586);
            throw;
        }
    }

    public Function GetPlayerFunction (string name, Scope scope)
    {
        try
        {
            Var player = Variable(scope);
            if (!scope.IsInScope(player) || player.type != ASTType.Type.FIELD) 
                ErrorInvalidParameterInFunction(name);
            Args args = new Args();
            args.Add(player);
            Function node = new Function(name, args);
            Eat(Token.Type.R_PARENTHESIS);
            return node;
        }
        catch
        {
            CrashLog(606);
            throw;
        }
    }

    public Function NoParametersFunction (string name)
    {
        try
        {
            Function node = new Function(name);
            Eat(Token.Type.R_PARENTHESIS);
            return node;
        }
        catch
        {
            CrashLog(621);
            throw;
        }
    }

    public Function CardParameterFunction (string name, Scope scope)
    {
        try
        {
            Var card = Variable(scope);
            if (!scope.IsInScope(card) || card.type != ASTType.Type.CARD)
                ErrorInvalidParameterInFunction(name);
            Args args = new Args();
            args.Add(card);
            Function node = new Function(name, args);
            Eat(Token.Type.R_PARENTHESIS);
            return node;
        }
        catch
        {
            CrashLog(641);
            throw;
        }
    }

    public Function FunctionStatement(string name, Scope scope)
    {
        try
        {
            Eat(Token.Type.FUNCTION);
            Eat(Token.Type.L_PARENTHESIS);

            if (name == "Find") return FindFunction(scope);

            if (name == "HandOfPlayer" || name == "FieldOfPlayer" ||
                name == "DeckOfPlayer" || name == "GraveyardOfPlayer")
                return GetPlayerFunction(name, scope);

            if (name == "Pop" || name == "Shuffle") return NoParametersFunction(name);

            if (name == "Push" || name == "Remove" || name == "Add" || name == "SendBottom") 
                return CardParameterFunction(name, scope);

            return new Function("NULL_FUNCTION");
        }
        catch
        {
            CrashLog(668);
            throw;
        }
    }

    public ForLoop ForLoopStatement(Scope scope)
    {
        try
        {
            Eat(Token.Type.FOR);
            
            Var target = Variable(scope);
            target.type = ASTType.Type.CARD;
            if (scope.IsInScope(target) || target.GetType() == typeof(VarComp)) ErrorUnvalidAssignment(target);
            else scope.Set(target,target.type);
            
            Eat(Token.Type.IN);
            
            Var targets = Variable(scope);
            if (!scope.IsInScope(targets) || targets.type != ASTType.Type.FIELD) ErrorUnvalidAssignment(targets);

            Compound body = CompoundStatement(scope);

            ForLoop node = new ForLoop(target, targets, body);
            return node;
        }
        catch
        {
            CrashLog(696);
            throw;
        }
    }

    public WhileLoop WhileLoopStatement(Scope scope)
    {
        try
        {
            Eat(Token.Type.WHILE);
            Eat(Token.Type.L_PARENTHESIS);
            AST condition = BooleanExpression(scope);
            Eat(Token.Type.R_PARENTHESIS);
            Compound body = CompoundStatement(scope);
            WhileLoop node = new WhileLoop(condition, body);
            return node;
        }
        catch
        {
            CrashLog(715);
            throw;
        }
    }

    public IfNode IfNodeStatement(Scope scope)
    {
        try
        {
            Eat(Token.Type.IF);
            Eat(Token.Type.L_PARENTHESIS);
            AST condition = BooleanExpression(scope);
            Eat(Token.Type.R_PARENTHESIS);
            Compound body = CompoundStatement(scope);
            IfNode node = new IfNode(condition, body);
            return node;
        }
        catch
        {
            CrashLog(734);
            throw;
        }
    }

    public void ErrorInvalidStatement()
    {
        Error("Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement");
    }

    public AST Statement(Scope scope)
    {
        try
        {
            if (currentToken.type == Token.Type.SEMI)
            {
                return new NoOp();
            }

            if (currentToken.type == Token.Type.WHILE)
            {
                return WhileLoopStatement(scope);
            }

            if (currentToken.type == Token.Type.IF)
            {
                return IfNodeStatement(scope);
            }

            if (currentToken.type == Token.Type.FOR)
            {
                return ForLoopStatement(scope);
            }

            if (currentToken.type == Token.Type.FUNCTION)
            {
                Function node = FunctionStatement(currentToken.value, scope);
                if (node.type != ASTType.Type.VOID) ErrorInvalidStatement();
                return node;
            }

            if (currentToken.type == Token.Type.ID)
            {
                Var variable = Variable(scope);
                if (variable.GetType() == typeof(VarComp) && currentToken.type == Token.Type.SEMI)
                {
                    VarComp v = variable as VarComp;
                    int count = v.args.Count - 1;
                    if (v.args[count].GetType() == typeof(Function))
                    {
                        Function f = v.args[count] as Function;
                        if (f.type != ASTType.Type.VOID) ErrorInvalidStatement();
                    }
                    else ErrorInvalidStatement();

                    return variable;
                }
                else if (currentToken.type == Token.Type.ASSIGN)
                {
                    Assign node = AssignmentStatement(variable, scope);
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
        catch
        {
            CrashLog(814);
            throw;
        }
    }

    public List<AST> StatementList(Scope scope)
    {
        try
        {
            List<AST> results = new List<AST>();

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                AST node = Statement(scope);
                results.Add(node);
                Eat(Token.Type.SEMI);
            }

            return results;
        }
        catch
        {
            CrashLog(836);
            throw;
        }
    }

    public Compound CompoundStatement(Scope outScope)
    {
        try
        {
            Scope scope = new Scope(outScope);

            Eat(Token.Type.L_BRACKET);
            List<AST> nodes = StatementList(scope);
            Eat(Token.Type.R_BRACKET);

            Compound root = new Compound();
            for (int i = 0; i < nodes.Count; i++)
            {
                root.children.Add(nodes[i]);
            }

            return root;
        }
        catch
        {
            CrashLog(861);
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
        catch
        {
            CrashLog(878);
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
        catch
        {
            CrashLog(895);
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
        catch
        {
            CrashLog(912);
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
        catch
        {
            CrashLog(931);
            throw;
        }
    }

    public void ErrorEffectCalling(Name name)
    {
        Error($"Effect '{name.name}' has not been declared");
    }

    public void ErrorEffectCalling(ASTType.Type type, Name name)
    {
        Var aux = new Var(new Token(Token.Type.ID, name.name), type);
        ErrorUnvalidAssignment(aux);
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
                        if (!GLOBAL_SCOPE.IsInScope(name)) ErrorEffectCalling(name);
                        else
                        {
                            if (GLOBAL_SCOPE.Get(name) != ASTType.Type.EFFECT)
                                ErrorEffectCalling(GLOBAL_SCOPE.Get(name), name);
                        }

                        if (currentToken.type != Token.Type.R_BRACKET) Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.ID)
                {
                    EffectNode effect;
                    Var variable = Variable(GLOBAL_SCOPE);

                    if (variable.GetType() == typeof(VarComp) || variable.type != ASTType.Type.NULL) 
                        ErrorUnvalidAssignment(variable);
                    Token token = currentToken;
                    Eat(Token.Type.COLON);
                    ASTType value = Expression(GLOBAL_SCOPE);
                    variable.type = value.type;
                    Assign param = new Assign(variable, token, value);


                    if (name == null) Error("'Name' of effect has not been declared");
                    else if (!EFFECT_LIST.ContainsKey(name.name)) ErrorEffectCalling(name);
                    else 
                    {
                        effect = EFFECT_LIST[name.name];
                        if (effect.scope.IsInScope(variable))
                        {
                            ASTType.Type typeInEffect = effect.scope.Get(variable);
                            if (typeInEffect != variable.type)
                            {
                                Error($"Invalid type of parameter: cannot convert {typeInEffect} to {variable.type}");
                            }
                        }
                        else Error($"Param '{variable.value}' not found in effect '{name.name}'");
                    }


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
        catch
        {
            CrashLog(1024);
            throw;
        }
    }

    public bool IsValidSource(Token token)
    {
        string s = token.value;
        return (s == "board" || s == "hand" || s == "deck" || s == "field" || s == "parent" ||
            s == "otherBoard" || s == "otherHand" || s == "otherDeck" || s == "otherField");
    }

    public Source SourceParse()
    {
        try
        {
            Eat(Token.Type.SOURCE);
            Eat(Token.Type.COLON);

            Token token = currentToken;
            Eat(Token.Type.STRING);

            if (!IsValidSource(token))
                Error($"'{token.value}' is not a valid source of context");

            Source node = new Source(token);
            return node;
        }
        catch
        {
            CrashLog(1054);
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
        catch
        {
            CrashLog(1073);
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
            
            Var unit = Variable(GLOBAL_SCOPE);
            unit.type = ASTType.Type.CARD;
            if (unit.GetType() == typeof(VarComp)) ErrorUnvalidAssignment(unit);

            Eat(Token.Type.R_PARENTHESIS);

            Eat(Token.Type.ARROW);

            Eat(Token.Type.L_PARENTHESIS);
            Scope scope = new Scope(GLOBAL_SCOPE);
            scope.Set(unit, unit.type);
            ASTType condition = BooleanExpression(scope);
            Eat(Token.Type.R_PARENTHESIS);

            Predicate node = new Predicate(unit, condition);

            return node;
        }
        catch
        {
            CrashLog(1107);
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
        catch
        {
            CrashLog(1167);
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
        catch
        {
            CrashLog(1218);
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
        catch
        {
            CrashLog(1278);
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
        catch
        {
            CrashLog(1304);
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
        catch
        {
            CrashLog(1329);
            throw;
        }
    }

    public void ErrorAlReadyDefinesMember(string name)
    {
        Error($"Card Editor already defines a member '{name}'");
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
                        if (GLOBAL_SCOPE.IsInScope(name)) ErrorAlReadyDefinesMember(name.name);
                        else GLOBAL_SCOPE.Set(name, ASTType.Type.CARD);
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
        catch
        {
            CrashLog(1425);
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
        catch
        {
            CrashLog(1444);
            throw;
        }
    }

    public Args GetParametersInParams(Scope scope)
    {
        try
        {
            Args args = new Args();

            while (currentToken.type != Token.Type.R_BRACKET && currentToken.type != Token.Type.EOF)
            {
                Var variable = Variable(scope);
                if (scope.IsInScope(variable) || variable.GetType() == typeof(VarComp))
                    Error("Invalid declaration of param");
                Eat(Token.Type.COLON);
                if (currentToken.type == Token.Type.D_INT ||
                    currentToken.type == Token.Type.D_STRING ||
                    currentToken.type == Token.Type.D_BOOL)
                {
                    variable.TypeInParams(currentToken.type);
                    scope.Set(variable, variable.type);
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
        catch
        {
            CrashLog(1485);
            throw;
        }
    }

    public Args ParamsEffectParse(Scope scope)
    {
        try
        {
            Eat(Token.Type.PARAMS);
            Eat(Token.Type.COLON);
            Eat(Token.Type.L_BRACKET);
            Args node = GetParametersInParams(scope);
            Eat(Token.Type.R_BRACKET);
            return node;
        }
        catch
        {
            CrashLog(1503);
            throw;
        }
    }

    public void ErrorUnvalidAssignment(Var variable)
    {
        Error($"Unvalid Assignment of '{variable.value}'");
    }

    public Action ActionParse(Scope outScope)
    {
        try
        {
            Eat(Token.Type.ACTION);
            Eat(Token.Type.COLON);

            Eat(Token.Type.L_PARENTHESIS);

            Var targets = Variable(outScope);
            targets.type = ASTType.Type.FIELD;
            if (outScope.IsInScope(targets) || targets.GetType() == typeof(VarComp))
                ErrorUnvalidAssignment(targets);
            else outScope.Set(targets, targets.type);

            Eat(Token.Type.COMA);

            Var context = Variable(outScope);
            context.type = ASTType.Type.CONTEXT;
            if (outScope.IsInScope(context) || context.GetType() == typeof(VarComp))
                ErrorUnvalidAssignment(context);
            else outScope.Set(context, context.type);

            Eat(Token.Type.R_PARENTHESIS);
            Eat(Token.Type.ARROW);

            Compound body = CompoundStatement(outScope);

            Action node = new Action(targets, context, body);
            return node;
        }
        catch
        {
            CrashLog(1546);
            throw;
        }
    }

    public EffectNode EffectCreation()
    {
        try
        {
            Eat(Token.Type.EFFECT);
            Eat(Token.Type.L_BRACKET);

            Scope scope = new Scope(GLOBAL_SCOPE);
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
                        if (GLOBAL_SCOPE.IsInScope(name)) ErrorAlReadyDefinesMember(name.name);
                        else GLOBAL_SCOPE.Set(name, ASTType.Type.EFFECT);
                        if (currentToken.type != Token.Type.R_BRACKET)
                            Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.PARAMS)
                {
                    if (parameters == null)
                    {
                        parameters = ParamsEffectParse(scope);
                        if (currentToken.type != Token.Type.R_BRACKET)
                            Eat(Token.Type.COMA);
                    }
                    else ErrorRepeat();
                }
                else if (currentToken.type == Token.Type.ACTION)
                {
                    if (action == null)
                    {
                        action = ActionParse(scope);
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
                node = new EffectNode(name, parameters, action, scope);
            }

            if (name == null || action == null)
            {
                ErrorInNodeCreation(node);
            }

            EFFECT_LIST[name.name] = node;

            return node;
        }
        catch
        {
            CrashLog(1623);
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
        catch
        {
            CrashLog(1657);
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
        catch
        {
            CrashLog(1679);
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
        catch
        {
            CrashLog(1700);
            AST node = new NoOp();
            return node;
        }
    }
}