using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;


public class DisplayCard : MonoBehaviour
{
    public bool isPowerUp = false;
    public bool isUnderClimateEffect = false;
    public bool isAverage = false;
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
    public string cardCommunion;
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
        isAverage = false;
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

    public void AverageCard (int average)
    {
        displayCard.cardPower = average;
        if (isPowerUp)
            displayCard.cardPower *= 2;
        cardPower = displayCard.cardPower;
        powerText.text = cardPower.ToString();
        powerText.color = Color.yellow;
        isAverage = true;
    }

    public void SetImages ()
    {
        Sprite aux; // Miss... ...


        if (cardZone == 'M')
        {
            zoneImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/MMM.png");
        }
        if (cardZone == 'R')
        {
            zoneImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/RRR.png");
        }
        if (cardZone == 'S')
        {
            zoneImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/SSS.png");
        }

        if (cardEffect == "PowerUp")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/POWER_UP.png");
        }
        if (cardEffect == "Climate")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/CLIMATE.png");
        }
        if (cardEffect == "Decoy")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/DECOY.png");
        }
        if (cardEffect == "Destroyer")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/DESTROYER.png");
        }
        if (cardEffect == "WeakDestroyer")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/WEAK_DESTROYER.png");
        }
        if (cardEffect == "Communion")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/COMMUNION.png");
        }
        if (cardEffect == "ClearClimate")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/CLEAR_CLIMATE.png");
        }
        if (cardEffect == "Average")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/AVERAGE.png");
        }
        if (cardEffect == "Take")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/TAKE.png");
        }
        if (cardEffect == "DestroyLine")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/DESTROY_LINE.png");
        }
        if (cardEffect == "None")
        {
            effectImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/NULL_IMAGE.png");
        }

        aux = effectImage.sprite;
        effectImage.sprite = zoneImage.sprite;
        zoneImage.sprite = aux;
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
        cardCommunion = displayCard.cardCommunion;
        cardSprite = displayCard.cardSprite;

        nameText.text = cardName;
        powerText.text = cardPower.ToString();
        descriptionText.text = cardDescription;
        cardImage.sprite = cardSprite;

        SetImages();
    }
}
