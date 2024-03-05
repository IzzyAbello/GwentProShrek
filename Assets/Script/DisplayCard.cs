using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class DisplayCard : MonoBehaviour
{
    public Card displayCard;


    public int cardId;
    public int cardFaction;
    public int cardPower;
    public string cardName;
    public string cardDescription;
    public int cardKind;
    public int cardZone;
    public int cardEffect;
    public Sprite cardSprite;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI descriptionText;

    public Image cardImage;
    public Image effectImage;
    public Image zoneImage;

    void Start()
    {

        cardId = displayCard.cardId;
        cardFaction = displayCard.cardFaction;
        cardPower = displayCard.cardPower;
        cardName = displayCard.cardName;
        cardDescription = displayCard.cardDescription;
        cardKind = displayCard.cardKind;
        cardZone = displayCard.cardZone;
        cardEffect = displayCard.cardEffect;
        cardSprite = displayCard.cardSprite;

        nameText.text = cardName;
        powerText.text = cardPower.ToString();
        descriptionText.text = cardDescription;
        cardImage.sprite = cardSprite;
    }
}
