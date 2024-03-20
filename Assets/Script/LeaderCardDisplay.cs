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

    public Image nameBorder;
    public Image nameBorderBorder;
    public Image descriptionBorder;
    public Image cardBorder;
    public Image upBorder;
    public Image downBorder;

    public void SetImages ()
    {
        if (cardFaction != 0)
        {
            nameBorder.color = Color.red;
            nameBorderBorder.color = Color.red;
            descriptionBorder.color = Color.red;
            cardBorder.color = Color.red;
            upBorder.color = Color.red;
            downBorder.color = Color.red;
        }
    }



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

        SetImages();
    }

}
