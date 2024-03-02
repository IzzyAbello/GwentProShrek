using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class DisplayCard : MonoBehaviour
{
    public List<Card> displayCardList = new List<Card>();
    public int displayId = 0;


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

    // Start is called before the first frame update
    void Start()
    {
        displayCardList = CardDatabase.cardListDatabase;

        cardId = displayCardList[displayId].cardId;
        cardFaction = displayCardList[displayId].cardFaction;
        cardPower = displayCardList[displayId].cardPower;
        cardName = displayCardList[displayId].cardName;
        cardDescription = displayCardList[displayId].cardDescription;
        cardKind = displayCardList[displayId].cardKind;
        cardZone = displayCardList[displayId].cardZone;
        cardEffect = displayCardList[displayId].cardEffect;
        cardSprite = displayCardList[displayId].cardSprite;

        nameText.text = cardName;
        powerText.text = cardPower.ToString();
        descriptionText.text = cardDescription;
        cardImage.sprite = cardSprite;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
