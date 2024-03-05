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
    private GameObject startParent;

    private void Awake()
    {
        canvas = GameObject.Find("MainCanvas");
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
        isOverDropZone = true;
        dropZone = collision.gameObject;
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
            startPosition = transform.position;
            startParent = transform.parent.gameObject;
            isDragging = true;
        }
    }

    public void EndDrag ()
    {
        isDragging = false;
        if (isOverDropZone)
        {
            transform.SetParent(dropZone.transform, false);
            DropZoneCards.cardsDropZone.Add(gameObject);

            DisplayCardsInZone.RemoveFromHand(gameObject);
            
            Debug.Log($"Card removed from Hand: {gameObject.name}");
            Debug.Log($"Card added to Drop Zone: {gameObject.name}");
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }
}
