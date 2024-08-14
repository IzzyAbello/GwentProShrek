using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private GameObject canvas;
    private bool isDragging = false;
    private bool isOverDropZone = false;
    private GameObject dropZone;
    private Vector2 startPosition;
    private GameObject hand;
    private GameObject startParent;
    private GameObject audioManager;
    private AudioManager audioM;


    private void Awake()
    {
        canvas = GameObject.Find("MainCanvas");
        audioManager = GameObject.Find("AudioManager");
        audioM = audioManager.GetComponent<AudioManager>();
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(canvas.transform, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.GetComponent<CanPlayCard>().CanPlaySpecificCard(collision.gameObject))
        {
            isOverDropZone = true;
            dropZone = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropZone = null;
    }

    public void StartDrag ()
    {
        if (!isOverDropZone)
        {
            audioM.PlaySound(audioM.drawCardAudio);

            startPosition = transform.position;
            startParent = transform.parent.gameObject;
            hand = transform.parent.gameObject;
            isDragging = true;
        }
    }

    public void EndDrag ()
    {
        isDragging = false;

        if (isOverDropZone)
        {
            transform.SetParent(dropZone.transform, false);

            if (startParent != dropZone)
            {
                GameObject button = GameObject.Find("PlayTurnButton");
                GameObject hand = startParent;

                startParent = dropZone;

                dropZone.GetComponent<DropZoneCards>().cardsDropZone.Add(gameObject);

                hand.GetComponent<Hand>().RemoveFromHand(gameObject);

                audioM.PlaySound(audioM.drawCardAudio);

                gameObject.GetComponent<CardEffect>().PlayEffect();

                /*Debug.Log($"Card removed from Hand: {gameObject.name}");
                Debug.Log($"Card added to Drop Zone: {gameObject.name}");*/

                if (!hand.GetComponent<Hand>().isPass)
                {
                    if (GetComponent<DisplayCard>().cardEffect != "Decoy")
                        button.GetComponent<SwitchTurn>().SwitchTurnPlayer(hand);
                }
            }
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);

            audioM.PlaySound(audioM.drawCardAudio);
        }
    }
}
