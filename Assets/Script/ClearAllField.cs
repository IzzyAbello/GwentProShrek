using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAllField : MonoBehaviour
{
    //GAME MASTER


    public List<GameObject> AllDropZones = new List<GameObject>(7);

    public void Clear()
    {
        for (int i = 0; i < AllDropZones.Count; i++)
        {
            AllDropZones[i].GetComponent<DropZoneCards>().ClearDropZone();
        }
    }

    public void ClearClimate()
    {
        for (int i = 0; i < AllDropZones.Count; i++)
        {
            if (AllDropZones[i].GetComponent<DropZoneConditions>().zone == 'C')
                AllDropZones[i].GetComponent<DropZoneCards>().ClearDropZone();
            AllDropZones[i].GetComponent<DropZoneCards>().ResetAllCards();
        }
    }
}
