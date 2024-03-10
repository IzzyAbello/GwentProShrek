using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCountManager : MonoBehaviour
{

    public void GetGridLayOutSpacing (int count)
    {
        GridLayoutGroup gridSpacing = gameObject.GetComponent<GridLayoutGroup>();

        if (count >= 0 && count < 8)
            gridSpacing.spacing = new Vector2(20, 0);
        if (count >= 8 && count < 12)
            gridSpacing.spacing = new Vector2(-15, 0);
        if (count >= 12 && count < 16)
            gridSpacing.spacing = new Vector2(-30, 0);
        if (count >= 16 && count < 24)
            gridSpacing.spacing = new Vector2(-50, 0);
        if (count >= 24)
            gridSpacing.spacing = new Vector2(-60, 0);
    }

    void Update()
    {
        int count;
        count = gameObject.GetComponent<DropZoneCards>().cardsDropZone.Count;

        GetGridLayOutSpacing(count);
    }
}
