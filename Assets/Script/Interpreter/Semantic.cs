using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Semantic
{
    public string name;
    public ASTType.Type type;
    public List<Semantic> fields;
}

public class StringSemantic : Semantic
{
    public StringSemantic(string name)
    {
        this.name = name;
        type = ASTType.Type.STRING;
    }
}

public class IntSemantic : Semantic
{
    public IntSemantic(string name)
    {
        this.name = name;
        type = ASTType.Type.INT;
    }
}

public class BoolSemantic : Semantic
{
    public BoolSemantic(string name)
    {
        this.name = name;
        type = ASTType.Type.BOOL;
    }
}

public class EffectSemantic : Semantic
{
    public EffectSemantic(string name)
    {
        this.name = name;
        type = ASTType.Type.EFFECT;
        fields = new List<Semantic>();
        fields.Add(new StringSemantic("Name"));
    }
}

public class CardSemantic : Semantic
{
    public CardSemantic(string name)
    {
        this.name = name;
        type = ASTType.Type.CARD;
        fields = new List<Semantic>();
        fields.Add(new StringSemantic("Type"));
        fields.Add(new StringSemantic("Name"));
        fields.Add(new StringSemantic("Faction"));
        fields.Add(new StringSemantic("Range"));
        fields.Add(new IntSemantic("Power"));
    }
}

public class IndexerSemantic : Semantic
{
    public IndexerSemantic(string name)
    {
        this.name = name + "->Indexer";
        type = ASTType.Type.INDEXER;
    }
}

public class FieldSemantic : Semantic
{
    public FieldSemantic(string name)
    {
        this.name = name;
        type = ASTType.Type.FIELD;
        fields = new List<Semantic>();
        fields.Add(new IndexerSemantic(name));
    }
}

public class TriggerPlayerSemantic : Semantic
{
    public TriggerPlayerSemantic(string name)
    {
        this.name = name;
        type = ASTType.Type.CONTEXT;
        fields = new List<Semantic>();
        fields.Add(new ContextSemanticTP("ContextOf->" + name));
    }
}

public class ContextSemantic : Semantic
{
    public ContextSemantic(string name)
    {
        this.name = name;
        type = ASTType.Type.CONTEXT;
        fields = new List<Semantic>();
        fields.Add(new TriggerPlayerSemantic("TriggerPlayer"));
        fields.Add(new TriggerPlayerSemantic("OppositePlayer"));
    }
}

public class ContextSemanticTP : Semantic
{
    public ContextSemanticTP(string name)
    {
        this.name = name;
        type = ASTType.Type.CONTEXT;
        fields = new List<Semantic>();
        fields.Add(new FieldSemantic("Hand"));
        fields.Add(new FieldSemantic("Graveyard"));
        fields.Add(new FieldSemantic("Deck"));
        fields.Add(new FieldSemantic("Melee"));
        fields.Add(new FieldSemantic("Range"));
        fields.Add(new FieldSemantic("Siege"));
    }
}