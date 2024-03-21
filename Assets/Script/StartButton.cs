using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public Vector2 pos;

    private void Start()
    {
        pos = new Vector2 (transform.position.x, transform.position.y);
    }


    public void OnClickStart ()
    {
        GameObject handShrek = GameObject.Find("HandShrek");
        GameObject handBad = GameObject.Find("HandBad");


        transform.position = new Vector2(transform.position.x, transform.position.y + 1000);
        
        if (handShrek.transform.position.y > handBad.transform.position.y)
        {
            handShrek.GetComponent<Hand>().isFirstRound = false;
        }
        else
        {
            handBad.GetComponent<Hand>().isFirstRound = false;
        }
    }
}
