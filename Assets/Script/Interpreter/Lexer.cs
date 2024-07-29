using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Lexer
{
    public string text;
    public int pos;
    public int row;
    public int column;
    public char currentChar;
    public Dictionary<string, Token> reservedKeywords = new Dictionary<string, Token>();

    public void ReadAllText()
    {
        while (true)
        {
            Token t = GetNextToken();

            Debug.Log("In Row " + row + " and Column " + column + "\nThis Token: " + t.type.ToString() + "\n" + t.value);

            if (t.type == Token.Type.EOF) break;
        }
    }

    public Lexer(string text)
    {
        this.text = text;
        pos = 0;
        row = 0;
        column = 0;
        currentChar = text[0];

        reservedKeywords["if"] = new Token(Token.Type.IF, "if");
        reservedKeywords["while"] = new Token(Token.Type.WHILE, "while");
        reservedKeywords["for"] = new Token(Token.Type.FOR, "for");
        reservedKeywords["in"] = new Token(Token.Type.IN, "in");

        reservedKeywords["effect"] = new Token(Token.Type.EFFECT, "effect");
        reservedKeywords["Name"] = new Token(Token.Type.NAME, "Name");
        reservedKeywords["Params"] = new Token(Token.Type.PARAMS, "Params");

        reservedKeywords["Number"] = new Token(Token.Type.D_INT, "Number");
        reservedKeywords["String"] = new Token(Token.Type.D_STRING, "String");
        reservedKeywords["Bool"] = new Token(Token.Type.D_BOOL, "Bool");
        reservedKeywords["true"] = new Token(Token.Type.BOOL, "true");
        reservedKeywords["false"] = new Token(Token.Type.BOOL, "false");

        reservedKeywords["Action"] = new Token(Token.Type.ACTION, "Action");

        reservedKeywords["TriggerPlayer"] = new Token(Token.Type.POINTER, "TriggerPlayer");

        reservedKeywords["Hand"] = new Token(Token.Type.POINTER, "Hand");
        reservedKeywords["Field"] = new Token(Token.Type.POINTER, "Field");
        reservedKeywords["Graveyard"] = new Token(Token.Type.POINTER, "Graveyard");
        reservedKeywords["Deck"] = new Token(Token.Type.POINTER, "Deck");
        reservedKeywords["Owner"] = new Token(Token.Type.POINTER, "Owner");
        reservedKeywords["Board"] = new Token(Token.Type.POINTER, "Board");

        reservedKeywords["Find"] = new Token(Token.Type.FUNCTION, "Find");
        reservedKeywords["HandOfPlayer"] = new Token(Token.Type.FUNCTION, "HandOfPlayer");
        reservedKeywords["FieldOfPlayer"] = new Token(Token.Type.FUNCTION, "FieldOfPlayer");
        reservedKeywords["GraveyardOfPlayer"] = new Token(Token.Type.FUNCTION, "GraveyardOfPlayer");
        reservedKeywords["DeckOfPlayer"] = new Token(Token.Type.FUNCTION, "DeckOfPlayer");
        reservedKeywords["Push"] = new Token(Token.Type.FUNCTION, "Push");
        reservedKeywords["SendBottom"] = new Token(Token.Type.FUNCTION, "SendBottom");
        reservedKeywords["Pop"] = new Token(Token.Type.FUNCTION, "Pop");
        reservedKeywords["Remove"] = new Token(Token.Type.FUNCTION, "Remove");
        reservedKeywords["Shuffle"] = new Token(Token.Type.FUNCTION, "Shuffle");
        reservedKeywords["Add"] = new Token(Token.Type.FUNCTION, "Add");

        reservedKeywords["card"] = new Token(Token.Type.CARD, "Card");

        reservedKeywords["Type"] = new Token(Token.Type.TYPE, "Type");
        reservedKeywords["Faction"] = new Token(Token.Type.FACTION, "Faction");
        reservedKeywords["Power"] = new Token(Token.Type.POWER, "Power");
        reservedKeywords["Range"] = new Token(Token.Type.RANGE, "Range");

        reservedKeywords["OnActivation"] = new Token(Token.Type.ONACTIVATION, "OnActivation");
        reservedKeywords["Effect"] = new Token(Token.Type.OA_EFFECT, "Effect");
        reservedKeywords["PostAction"] = new Token(Token.Type.POSTACTION, "PostAction");
        reservedKeywords["Selector"] = new Token(Token.Type.SELECTOR, "Selector");
        reservedKeywords["Source"] = new Token(Token.Type.SOURCE, "Source");
        reservedKeywords["Single"] = new Token(Token.Type.SINGLE, "Single");
        reservedKeywords["Predicate"] = new Token(Token.Type.PREDICATE, "Predicate");
    }

    public void Error(string errorTag)
    {
        string error ="Invalid character '" + currentChar + "' at position " + row + " " + column + "\n";
        error += "Error Tag: " + errorTag + "\n";
        error += "Code:\n";
        error += (text.Substring(0, pos)) + "\n";
        error += "-----------------------------ERROR-----------------------------\n";
        error += (text.Substring(pos)) + "\n";

        Debug.Log(error);
    }

    public void Advance()
    {
        pos++;
        if (pos >= text.Length)
            currentChar = '\0';
        else currentChar = text[pos];
    }

    public char Peek()
    {
        int PeekPos = pos + 1;
        if (PeekPos >= text.Length) return '\0';

        return text[PeekPos];
    }

    public void SkipWhitespace()
    {
        while (currentChar != '\0' && (currentChar == ' ' || currentChar == '\n' || currentChar == '\t' || currentChar == '\r'))
        {
            if (currentChar == '\n')
            {
                column = 0;
                row++;
            }
            Advance();
        }
    }

    public string Integer()
    {
        string result = "";
        while (currentChar != '\0' && MyTools.IsDigit(currentChar))
        {
            result += currentChar;
            Advance();
        }
        return result;
    }

    public string GetString()
    {
        string result = "";
        while (currentChar != '\"')
        {

            if (currentChar == '\r' || currentChar == '\n')
            {
                Error("Invalid char in String... You may miss:  ( \" ) ");
                break;
            }
            result += currentChar;
            Advance();
            if (currentChar == '\0')
            {
                Error("Cannot find the end of string... You may miss: \" ");
                break;
            }
        }
        return result;
    }

    public Token GetId()
    {
        Token token;
        string result = "";
        while (currentChar != '\0' && MyTools.IsAlnum(currentChar))
        {
            result += currentChar;
            Advance();
        }

        if (!reservedKeywords.ContainsKey(result))
            reservedKeywords[result] = new Token(Token.Type.ID, result);

        token = reservedKeywords[result];
        return token;
    }

    public Token GetNextToken()
    {
        while (currentChar != '\0')
        {
            column++;
            if (currentChar == ' ' || currentChar == '\n' || currentChar == '\t' || currentChar == '\r')
            {
                if (currentChar == '\n')
                {
                    column = 0;
                    row++;
                }
                SkipWhitespace();
                continue;
            }
            if (MyTools.IsDigit(currentChar))
            {
                Token token = new Token(Token.Type.INT, Integer());
                return token;
            }
            if (currentChar == ',')
            {
                Advance();
                Token token = new Token(Token.Type.COMA, ",");
                return token;
            }
            if (currentChar == '+' && Peek() == '+')
            {
                Advance(); Advance();

                Token token = new Token(Token.Type.PLUSPLUS, "++");
                return token;
            }
            if (currentChar == '+' && Peek() == '=')
            {
                Advance(); Advance();
                Token token = new Token(Token.Type.ASSIGN, "+=");
                return token;
            }
            if (currentChar == '+')
            {
                Advance();
                Token token = new Token(Token.Type.PLUS, "+");
                return token;
            }
            if (currentChar == '-' && Peek() == '-')
            {
                Advance(); Advance();
                Token token = new Token(Token.Type.MINUSMINUS, "--");
                return token;
            }
            if (currentChar == '-' && Peek() == '=')
            {
                Advance(); Advance();
                Token token = new Token(Token.Type.ASSIGN, "-=");
                return token;
            }
            if (currentChar == '-')
            {
                Advance();
                Token token = new Token(Token.Type.MINUS, "-");
                return token;
            }
            if (currentChar == '*' && Peek() == '=')
            {
                Advance(); Advance();
                Token token = new Token(Token.Type.ASSIGN, "*=");
                return token;
            }
            if (currentChar == '*')
            {
                Advance();
                Token token = new Token(Token.Type.MULT, "*");
                return token;
            }
            if (currentChar == '/' && Peek() == '=')
            {
                Advance(); Advance();
                Token token = new Token(Token.Type.ASSIGN, "/=");
                return token;
            }
            if (currentChar == '/')
            {
                Advance();
                Token token = new Token(Token.Type.DIVIDE, "/");
                return token;
            }
            if (currentChar == '%' && Peek() == '=')
            {
                Advance(); Advance();
                Token token = new Token(Token.Type.ASSIGN, "%=");
                return token;
            }
            if (currentChar == '%')
            {
                Advance();
                Token token = new Token(Token.Type.MOD, "%");
                return token;
            }
            if (currentChar == '@' && Peek() == '@')
            {
                Advance(); Advance();

                Token token = new Token(Token.Type.STRING_SUM_S, "@@");
                return token;
            }
            if (currentChar == '@' && Peek() == '=')
            {
                Advance(); Advance();
                Token token = new Token(Token.Type.ASSIGN, "@=");
                return token;
            }
            if (currentChar == '@')
            {
                Advance();

                Token token = new Token(Token.Type.STRING_SUM, "@");
                return token;
            }
            if (currentChar == '=' && Peek() == '>')
            {
                Advance(); Advance();

                Token token = new Token(Token.Type.ARROW, "=>");
                return token;
            }
            if (currentChar == '=' && Peek() == '=')
            {
                Advance(); Advance();

                Token token = new Token(Token.Type.EQUAL, "==");
                return token;
            }
            if (currentChar == '!' && Peek() == '=')
            {
                Advance(); Advance();

                Token token = new Token(Token.Type.DIFFER, "!=");
                return token;
            }
            if (currentChar == '!')
            {
                Advance();
                Token token = new Token(Token.Type.NOT, "!");
                return token;
            }
            if (currentChar == '&' && Peek() == '&')
            {
                Advance(); Advance();

                Token token = new Token(Token.Type.AND, "&&");
                return token;
            }
            if (currentChar == '|' && Peek() == '|')
            {
                Advance(); Advance();

                Token token = new Token(Token.Type.OR, "||");
                return token;
            }
            if (currentChar == '>' && Peek() == '=')
            {
                Advance(); Advance();

                Token token = new Token(Token.Type.GREATER_E, ">=");
                return token;
            }
            if (currentChar == '<' && Peek() == '=')
            {
                Advance(); Advance();

                Token token = new Token(Token.Type.LESS_E, "<=");
                return token;
            }
            if (currentChar == '>')
            {
                Advance();
                Token token = new Token(Token.Type.GREATER, ">");
                return token;
            }
            if (currentChar == '<')
            {
                Advance();
                Token token = new Token(Token.Type.LESS, "<");
                return token;
            }
            if (currentChar == '(')
            {
                Advance();
                Token token = new Token(Token.Type.L_PARENTHESIS, "(");
                return token;
            }
            if (currentChar == ')')
            {
                Advance();
                Token token = new Token(Token.Type.R_PARENTHESIS, ")");
                return token;
            }
            if (currentChar == '{')
            {
                Advance();
                Token token = new Token(Token.Type.L_BRACKET, "{");
                return token;
            }
            if (currentChar == '}')
            {
                Advance();
                Token token = new Token(Token.Type.R_BRACKET, "}");
                return token;
            }
            if (currentChar == '[')
            {
                Advance();
                Token token = new Token(Token.Type.L_SQ_BRACKET, "[");
                return token;
            }
            if (currentChar == ']')
            {
                Advance();
                Token token = new Token(Token.Type.R_SQ_BRACKET, "]");
                return token;
            }
            if (currentChar == ':')
            {
                Advance();
                Token token = new Token(Token.Type.COLON, ":");
                return token;
            }
            if (MyTools.IsAlpha(currentChar))
            {
                return GetId();
            }
            if (currentChar == '=')
            {
                Advance();
                Token token = new Token(Token.Type.ASSIGN, "=");
                return token;
            }
            if (currentChar == ';')
            {
                Advance();
                Token token = new Token(Token.Type.SEMI, ";");
                return token;
            }
            if (currentChar == '.')
            {
                Advance();
                Token token = new Token(Token.Type.DOT, ".");
                return token;
            }
            if (currentChar == '\"')
            {
                Advance();
                Token token = new Token(Token.Type.STRING, GetString());
                Advance();
                return token;
            }
            Error("Invalid character found: " + currentChar);
            Advance();
        }

        Token eof = new Token(Token.Type.EOF, "");
        return eof;
    }
}