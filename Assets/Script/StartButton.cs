using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public Vector2 boardPosition;
    private AudioManager audioM;

    private void Start()
    {
        boardPosition = new Vector2 (transform.position.x, transform.position.y);
        audioM = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }


    public void OnClickStart ()
    {
        GameObject handShrek = GameObject.Find("HandShrek");
        GameObject handBad = GameObject.Find("HandBad");

        audioM.PlaySound(audioM.startAudio);

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