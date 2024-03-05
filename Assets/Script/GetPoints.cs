using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetPoints : MonoBehaviour
{
    public TextMeshProUGUI pointsText;

    void Update()
    {
        DropZoneCards.GetPoints();
        pointsText.text = DropZoneCards.pointsInDropZone.ToString();
    }
}
