using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LeaderCardDisplay : MonoBehaviour
{
    public Card leaderCard;

    public int cardId;
    public int cardFaction;
    public string cardName;
    public string cardDescription;
    public Sprite cardSprite;


    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public Image cardImage;

    private void Start()
    {
        cardId = leaderCard.cardId;
        cardFaction = leaderCard.cardFaction;
        cardName = leaderCard.cardName;
        cardDescription = leaderCard.cardDescription;
        cardSprite = leaderCard.cardSprite;

        nameText.text = cardName;
        descriptionText.text = cardDescription;
        cardImage.sprite = cardSprite;
    }

}
