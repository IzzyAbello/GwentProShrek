using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Deck : MonoBehaviour
{
    public static List<GameObject> deck = new List<GameObject>();

    static void ShuffleDeck ()
    {
        Debug.Log("Shuffling Deck...\n");

        for (int i = 0; i < deck.Count; i++)
        {
            GameObject temp = deck[i];
            int randomIndex = Random.Range(0, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }   
    }
    
    public static void RemoveFromDeck (int amount)
    {
        for (int i = 1; i <= amount; i++)
        {
            Debug.Log($"Card removed: {deck[0].name}");
            deck.RemoveAt(0);
        }

    }
    
    void Awake()
    {
        Debug.Log("Creating Deck...\n");


        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/ShrekCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/BurroCard.prefab"));

        Debug.Log($"Deck full size: {deck.Count}");

        ShuffleDeck();
    }
}
