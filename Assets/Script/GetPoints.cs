using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetPoints : MonoBehaviour
{
    public TextMeshProUGUI pointsText;

    public DropZoneCards dropZoneM;
    public DropZoneCards dropZoneR;
    public DropZoneCards dropZoneS;

    public int points;

    void Update()
    {
        dropZoneM.GetPoints();
        dropZoneR.GetPoints();
        dropZoneS.GetPoints();

        points = dropZoneM.pointsInDropZone + dropZoneR.pointsInDropZone + dropZoneS.pointsInDropZone;

        pointsText.text = points.ToString();
    }
}
