using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetPoints : MonoBehaviour
{
    public TextMeshProUGUI pointsText;

    public DropZoneCards dropZone;


    void Update()
    {
        dropZone.GetPoints();
        pointsText.text = dropZone.pointsInDropZone.ToString();
    }
}
