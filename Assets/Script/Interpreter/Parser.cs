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
            if (token.type == Token.Type.INT)
            {
                Eat(Token.Type.INT);
                Num node = new Num(token);
                return node;
            }
            else if (token.type == Token.Type.L_PARENTHESIS)
            {
                Eat(Token.Type.L_PARENTHESIS);
                AST result = Expression();
                Eat(Token.Type.R_PARENTHESIS);
                return result;
            }
            else
            {
                if (currentToken.type == Token.Type.ID) return Variable();
                else return FunctionStatement(currentToken.value);
            }
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

            while (currentToken.type == Token.Type.MULT ||
                    currentToken.type == Token.Type.DIVIDE ||
                    currentToken.type == Token.Type.MOD)
            {
                Token token = currentToken;
                if (token.type == Token.Type.MULT)
                {
                    Eat(Token.Type.MULT);
                }
                else if (token.type == Token.Type.DIVIDE)
                {
                    Eat(Token.Type.DIVIDE);
                }
                else if (token.type == Token.Type.MOD)
                {
                    Eat(Token.Type.MOD);
                }

                node = new BinOp(node, token, Factor());
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

            while (currentToken.type == Token.Type.PLUS || currentToken.type == Token.Type.MINUS)
            {
                Token token = currentToken;
                if (token.type == Token.Type.PLUS)
                {
                    Eat(Token.Type.PLUS);
                }
                else if (token.type == Token.Type.MINUS)
                {
                    Eat(Token.Type.MINUS);
                }
                node = new BinOp(node, token, Term());
            }
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
            Token token = currentToken;
            if (token.type == Token.Type.NOT)
            {
                Eat(Token.Type.NOT);
                Eat(Token.Type.L_PARENTHESIS);
                UnaryOp node = new UnaryOp(token, BooleanTerm());
                Eat(Token.Type.R_PARENTHESIS);
                return node;
            }
            if (token.type == Token.Type.L_PARENTHESIS)
            {
                Eat(Token.Type.L_PARENTHESIS);
                AST result = BooleanTerm();
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
            else Error();

            AST right = Expression();
            return new BinOp(left, token, right);
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
            Token.Type type = currentToken.type;
            if (type != Token.Type.AND && type != Token.Type.OR) return node;

            while (currentToken.type == Token.Type.AND || currentToken.type == Token.Type.OR)
            {
                Token token = currentToken;
                if (token.type == Token.Type.AND)
                {
                    if (type == Token.Type.OR) Error();
                    Eat(Token.Type.AND);
                }
                else if (token.type == Token.Type.OR)
                {
                    if (type == Token.Type.AND) Error();
                    Eat(Token.Type.OR);
                }
                node = new BinOp(node, token, BooleanTerm());
            }
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public NoOp Empty()
    {
        try
        {
            return new NoOp();
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
            return node;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Assign AssignmentStatement()
    {
        try
        {
            Var left = Variable();
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

    public Conditional ConditionalStatement()
    {
        try
        {
            Token.Type type = currentToken.type;
            if (type == Token.Type.WHILE) Eat(Token.Type.WHILE);
            else Eat(Token.Type.IF);
            Eat(Token.Type.L_PARENTHESIS);
            AST condition = BooleanExpression();
            Eat(Token.Type.R_PARENTHESIS);
            Compound body = CompoundStatement();
            return new Conditional(type, condition, body);
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
            Args args = new Args(new List<AST>());
            Eat(Token.Type.FUNCTION);
            Eat(Token.Type.L_PARENTHESIS);

            while (currentToken.type != Token.Type.COMA && currentToken.type != Token.Type.R_PARENTHESIS)
            {
                AST currentArg = Expression();
                args.Add(currentArg);
                if (currentToken.type == Token.Type.COMA) Eat(Token.Type.COMA);
                else if (currentToken.type == Token.Type.R_PARENTHESIS)
                {
                    Eat(Token.Type.R_PARENTHESIS);
                    return new Function(name, args);
                }
            }
            Eat(Token.Type.R_PARENTHESIS);
            return new Function(name, args);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Statement()
    {
        try
        {
            if (currentToken.type == Token.Type.L_BRACKET)
            {
                return CompoundStatement();
            }

            if (currentToken.type == Token.Type.WHILE || currentToken.type == Token.Type.IF)
            {
                return ConditionalStatement();
            }

            if (currentToken.type == Token.Type.FUNCTION)
            {
                Function node = FunctionStatement(currentToken.value);
                Eat(Token.Type.SEMI);
                return node;
            }

            if (currentToken.type == Token.Type.ID)
            {
                Assign node = AssignmentStatement();
                Eat(Token.Type.SEMI);
                return node;
            }

            if (currentToken.type == Token.Type.R_BRACKET)
            {
                return Empty();
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
            results.Add(Statement());

            while (currentToken.type != Token.Type.R_BRACKET)
            {
                results.Add(Statement());
                if (currentToken.type != Token.Type.R_BRACKET)
                {
                    Eat(Token.Type.SEMI);
                }
            }

            if (currentToken.type == Token.Type.ID)
            {
                Error();
            }


            return results;
        }
        catch (System.Exception)
        {
            throw;
        }
    }


    public AST Parameter()
    {
        try
        {
            if (currentToken.type == Token.Type.NAME)
            {
                Eat(Token.Type.COLON);
                string name = currentToken.value;
                Name node = new Name(name);
                Eat(Token.Type.STRING);
                return node;
            }

            if (currentToken.type == Token.Type.PARAMS)
            {
                Eat(Token.Type.COLON);
                Eat(Token.Type.L_BRACKET);
                Args node = GetParameters();
                Eat(Token.Type.R_BRACKET);
                return node;
            }

            if (currentToken.type == Token.Type.TYPE)
            {
                Eat(Token.Type.COLON);
                string type = currentToken.value;
                Type node = new Type(type);
                Eat(Token.Type.STRING);
                return node;
            }

            if (currentToken.type == Token.Type.FACTION)
            {
                Eat(Token.Type.COLON);
                string faction = currentToken.value;
                Faction node = new Faction(faction);
                Eat(Token.Type.STRING);
                return node;
            }

            if (currentToken.type == Token.Type.POWER)
            {
                Eat(Token.Type.COLON);
                int power = int.Parse(currentToken.value);
                Power node = new Power(power);
                Eat(Token.Type.INT);
                return node;
            }

            if (currentToken.type == Token.Type.RANGE)
            {
                Eat(Token.Type.COLON);
                string range = currentToken.value;
                Range node = new Range(range);
                Eat(Token.Type.STRING);
                return node;
            }

            if (currentToken.type == Token.Type.ACTION)
            {
                Eat(Token.Type.COLON);
                Eat(Token.Type.L_PARENTHESIS);
                Args args = new Args(new List<AST>());

                while (currentToken.type != Token.Type.COMA && currentToken.type != Token.Type.R_PARENTHESIS)
                {
                    AST currentArg = Variable();
                    args.Add(currentArg);
                    if (currentToken.type == Token.Type.COMA) Eat(Token.Type.COMA);
                    else if (currentToken.type == Token.Type.R_PARENTHESIS)
                    {
                        Eat(Token.Type.R_PARENTHESIS);
                        break;
                    }
                }
                Eat(Token.Type.ARROW);
                AST node = CompoundStatement();
                return node;
            }

            if (currentToken.type == Token.Type.ONACTIVATION)
            {
                Eat(Token.Type.COLON);
                AST node = OnActivationCompound();
                return node;
            }

            Error();
            return new NoOp();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Args GetParameters()
    {
        try
        {
            Args args = new Args(new List<AST>());

            while (currentToken.type != Token.Type.R_BRACKET)
            {
                if (currentToken.type == Token.Type.NAME)
                {
                    Eat(Token.Type.COLON);
                    string name = currentToken.value;
                    args.Add(new ExpectedParameter((name, Token.Type.NAME)));
                }
                if (currentToken.type == Token.Type.ID)
                {
                    string name = currentToken.value;
                    Eat(Token.Type.COLON);
                    Token.Type type = Token.Type.NULL;
                    if (currentToken.type == Token.Type.INT) type = Token.Type.INT;
                    if (currentToken.type == Token.Type.STRING) type = Token.Type.STRING;
                    if (currentToken.type == Token.Type.BOOL) type = Token.Type.BOOL;
                    if (type == Token.Type.NULL)
                    {
                        Error();
                        args.Add(new NoOp());
                    }
                    else
                    {
                        args.Add(new ExpectedParameter((name, type)));
                    }
                }
                if (currentToken.type != Token.Type.R_BRACKET)
                {
                    Eat(Token.Type.COMA);
                }
            }
            return args;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Args GetSelector()
    {
        try
        {
            Args args = new Args(new List<AST>());
            while (currentToken.type != Token.Type.R_BRACKET)
            {
                if (currentToken.type == Token.Type.SOURCE)
                {
                    Eat(Token.Type.COLON);
                    args.Add(new String(currentToken.value));
                    Eat(Token.Type.STRING);
                }
                if (currentToken.type == Token.Type.SINGLE)
                {
                    Eat(Token.Type.COLON);
                    bool aux = false;
                    if (currentToken.value == "true") aux = true;
                    else if (currentToken.value == "false") aux = false;
                    else { Error(); args.Add(new NoOp()); continue; }

                    args.Add(new Bool(aux));
                    Eat(currentToken.type);
                }
                if (currentToken.type == Token.Type.PREDICATE)
                {
                    Eat(Token.Type.COLON);
                    Eat(Token.Type.L_PARENTHESIS);
                    AST var = Variable();
                    args.Add(var);
                    Eat(Token.Type.R_PARENTHESIS);
                    Eat(Token.Type.ARROW);
                    Eat(Token.Type.L_PARENTHESIS);
                    AST condition = BooleanExpression();
                    Eat(Token.Type.R_PARENTHESIS);
                    args.Add(condition);
                }

                if (currentToken.type != Token.Type.R_BRACKET)
                {
                    Eat(Token.Type.COMA);
                }
            }
            return args;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST OnActivationParameter()
    {
        try
        {
            if (currentToken.type == Token.Type.EFFECT)
            {
                Eat(Token.Type.COLON);
                Eat(Token.Type.L_BRACKET);
                AST node = GetParameters();
                Eat(Token.Type.R_BRACKET);
                return node;
            }

            if (currentToken.type == Token.Type.SELECTOR)
            {
                Eat(Token.Type.COLON);
                Eat(Token.Type.L_BRACKET);
                AST node = GetSelector();
                Eat(Token.Type.R_BRACKET);
            }

            if (currentToken.type == Token.Type.POSTACTION)
            {
                Eat(Token.Type.COLON);
                AST node = OnActivationCompound();
                return node;
            }

            Error();
            return new NoOp();
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public List<AST> OnActivationList()
    {
        try
        {
            List<AST> results = new List<AST>();
            results.Add(OnActivationParameter());

            while (currentToken.type != Token.Type.R_SQ_BRACKET)
            {
                Eat(Token.Type.L_BRACKET);
                results.Add(OnActivationParameter());
                Eat(Token.Type.R_BRACKET);
                if (currentToken.type != Token.Type.R_SQ_BRACKET)
                {
                    Eat(Token.Type.COMA);
                }
            }

            if (currentToken.type == Token.Type.ID)
            {
                Error();
            }

            return results;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Compound OnActivationCompound()
    {
        try
        {
            Eat(Token.Type.L_SQ_BRACKET);
            List<AST> nodes = OnActivationList();
            Eat(Token.Type.R_SQ_BRACKET);

            Compound root = new Compound();
            foreach (AST node in nodes)
            {
                root.children.Add(node);
            }
            return root;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public List<AST> ParameterList()
    {
        try
        {
            List<AST> results = new List<AST>();
            results.Add(Parameter());

            while (currentToken.type != Token.Type.R_BRACKET)
            {
                results.Add(Parameter());
                if (currentToken.type != Token.Type.R_BRACKET)
                {
                    Eat(Token.Type.COMA);
                }
            }

            if (currentToken.type == Token.Type.ID)
            {
                Error();
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
            foreach (AST node in nodes)
            {
                root.children.Add(node);
            }

            return root;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Compound CardOrEffectStatement()
    {
        try
        {
            bool isCard = false, isEffect = false;

            if (currentToken.type == Token.Type.CARD)
            {
                Eat(Token.Type.CARD);
                isCard = true;
            }
            else
            {
                Eat(Token.Type.EFFECT);
                isEffect = true;
            }

            Eat(Token.Type.L_BRACKET);
            List<AST> nodes = ParameterList();
            Eat(Token.Type.R_BRACKET);

            Compound root = new Compound();
            foreach (AST node in nodes)
            {
                root.children.Add(node);
            }

            if (isCard && root.IsValidCard() || isEffect && root.IsValidEffect())
            {
                return root;
            }
            else
            {
                Error();
                root = new Compound();
                root.children.Add(new NoOp());
                return root;
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public AST Program()
    {
        try
        {
            AST node = CardOrEffectStatement();
            return node;
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
            AST node = Program();
            if (currentToken.type != Token.Type.EOF)
            {
                Error();
            }
            return node;
        }
        catch (System.Exception)
        {
            AST node = new NoOp();
            return node;
        }
    }
}
