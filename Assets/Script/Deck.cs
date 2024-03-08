﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class Deck : MonoBehaviour
{
    public List<GameObject> deck = new List<GameObject>();
    public TextMeshProUGUI deckSize;

    void ShuffleDeck ()
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
    
    public void RemoveFromDeck (int amount)
    {
        for (int i = 1; i <= amount; i++)
        {
            Debug.Log($"Card removed from deck: {deck[0].name}");
            deck.RemoveAt(0);
        }

    }
    
    void Awake()
    {
        Debug.Log("Creating Deck...\n");


        //deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("")); Template

        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/ShrekCard.prefab"));
        //deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/BurroCard.prefab"));
        //deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/BadFaction/LordFarquaadCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/BadFaction/RumpelstinskinCard.prefab"));

        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/Cerdito1Card.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/Cerdito2Card.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/Cerdito3Card.prefab"));

        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ClimateCard.prefab"));
        //deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DecoyCard.prefab"));

        Debug.Log($"Deck full size: {deck.Count}");

        ShuffleDeck();
    }

    private void Update()
    {
        deckSize.text = deck.Count.ToString();
    }
}
