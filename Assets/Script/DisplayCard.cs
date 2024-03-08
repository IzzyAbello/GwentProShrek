using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class DisplayCard : MonoBehaviour
{
    public bool isPowerUp = false;
    public bool isUnderClimateEffect = false;
    public Card displayCard;


    public int cardId;
    public int cardFaction;
    public int cardPower;
    public int cardPowerOG;
    public string cardName;
    public string cardDescription;
    public char cardKind;
    public char cardZone;
    public string cardEffect;
    public Sprite cardSprite;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI descriptionText;

    public Image cardImage;
    public Image effectImage;
    public Image zoneImage;


    public void CardReset ()
    {
        cardPower = cardPowerOG;
        powerText.text = cardPower.ToString();
        powerText.color = Color.black;
        displayCard.cardPower = displayCard.cardPowerOG;
        isPowerUp = false;
        isUnderClimateEffect = false;
    }

    public void CardPowerUp (int n = 2)
    {
        displayCard.cardPower *= n;
        cardPower *= n;
        powerText.text = cardPower.ToString();
        powerText.color = Color.green;
        isPowerUp = true;
    }

    public void CardUnderClimateEffect()
    {
        if (cardPower > 0)
        displayCard.cardPower = 1;
        if (isPowerUp)
            displayCard.cardPower = 2;
        cardPower = displayCard.cardPower;
        powerText.text = cardPower.ToString();
        powerText.color = Color.red;
        isUnderClimateEffect = true;
    }

    void Start()
    {

        cardId = displayCard.cardId;
        cardFaction = displayCard.cardFaction;
        cardPower = displayCard.cardPower;
        cardPowerOG = displayCard.cardPowerOG;
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
