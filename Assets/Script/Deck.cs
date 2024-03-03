using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Deck : MonoBehaviour
{
    public static List<GameObject> deck = new List<GameObject>();

    void Awake()
    {
        Debug.Log("Creating Deck...\n");


        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/ShrekCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/BurroCard.prefab"));

        Debug.Log($"Deck full size: {deck.Count}");
    }
}
