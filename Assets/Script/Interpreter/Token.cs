using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token
{
    public enum Type
    {
        // Single Char Tokens...
        L_BRACKET, R_BRACKET, L_PARENTHESIS, R_PARENTHESIS, L_SQ_BRACKET, R_SQ_BRACKET, SEMI, COMA, 
        DOT, DOTS, COLON, ARROW,


        // Conditional Tokens...
        AND, OR, NOT, GREATER, LESS, GREATER_E, LESS_E, EQUAL, DIFFER, NULL,

        // Declarations...
        D_INT, D_BOOL, D_STRING,

        // Literals...
        INT, BOOL, STRING, ID,
        
        // Operators
        PLUS, MINUS, MULT, DIVIDE, MOD, POW, PLUS1, STRING_SUM, STRING_SUM_S, ASSIGN, 

        // Keywords...
        IF, FOR, WHILE,  ACTION, CARD, EFFECT, NAME, PARAMS, TYPE, FACTION, POWER, RANGE,
        ONACTIVATION, SELECTOR, SOURCE, SINGLE, PREDICATE, POSTACTION, IN, OA_EFFECT,

        // Functions...
        FUNCTION, POINTER,
        
        
        EOF
    }

    public Type type;
    public string value;

    public Token (Type type, string value)
    {
        this.type = type;
        this.value = value;
    }
}
