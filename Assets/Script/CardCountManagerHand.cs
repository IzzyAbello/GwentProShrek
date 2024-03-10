using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCountManagerHand : MonoBehaviour
{
    public void GetGridLayOutSpacing(int count)
    {
        GridLayoutGroup gridSpacing = gameObject.GetComponent<GridLayoutGroup>();

        if (count >= 0 && count < 5)
            gridSpacing.spacing = new Vector2(20, 0);
        if (count >= 5 && count < 9)
            gridSpacing.spacing = new Vector2(-20, 0);
        if (count >= 9 && count < 12)
            gridSpacing.spacing = new Vector2(-50, 0);
    }

    void Update()
    {
        int count;
        count = gameObject.GetComponent<Hand>().hand.Count;

        GetGridLayOutSpacing(count);
    }
}
