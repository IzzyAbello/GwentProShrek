using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Graveyard : MonoBehaviour
{
    public List<GameObject> cardsGraveyard = new List<GameObject>();
    public TextMeshProUGUI graveyardSize;

    void ShuffleGraveyard()
    {
        Debug.Log("Shuffling Graveyard...\n");

        for (int i = 0; i < cardsGraveyard.Count; i++)
        {
            GameObject temp = cardsGraveyard[i];
            int randomIndex = Random.Range(0, cardsGraveyard.Count);
            cardsGraveyard[i] = cardsGraveyard[randomIndex];
            cardsGraveyard[randomIndex] = temp;
        }
    }

    public void RemoveFromGraveyard(int amount)
    {
        for (int i = 1; i <= amount; i++)
        {
            Debug.Log($"Card removed from graveyard: {cardsGraveyard[cardsGraveyard.Count - 1].name}");
            cardsGraveyard.RemoveAt(cardsGraveyard.Count - 1);
        }

    }

    private void Update()
    {

    }
}
