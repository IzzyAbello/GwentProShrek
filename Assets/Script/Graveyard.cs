using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Graveyard : MonoBehaviour
{
    public List<GameObject> cardsGraveyard = new List<GameObject>();
    public TextMeshProUGUI graveyardSize;

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
