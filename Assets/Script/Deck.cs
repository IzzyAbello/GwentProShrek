using System.Collections;
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


        //deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/MerlinCard.prefab")); Template

        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/ShrekCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/BurroCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/Los7EnanosCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/PinochoCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/LosRatonesCiegosCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/JenjiCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/LoboFerozCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/Cerdito1Card.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/Cerdito2Card.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/Cerdito3Card.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/EspejoMagicoCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/DragonCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/FionaCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/FionaHumanaCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/BlancanievesCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/ReySapoCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/ReinaLillian.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/DorisCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/GatoConBotasCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/ShrekHumanoCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/BurroSementalCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/JengibreGiganteCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/HijosBurroCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/FionaGuerreraCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/CookieCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/ArtiCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/GoodFaction/MerlinCard.prefab"));


        /*deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/BadFaction/LordFarquaadCard.prefab"));
        deck.Add(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cards/BadFaction/RumpelstinskinCard.prefab"));*/

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
