using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource drawCardAudio;
    public AudioSource effectPowerUpAudio;
    public AudioSource turnAudio;
    public AudioSource roundAudio;
    public AudioSource climateAudio;
    public AudioSource startAudio;

    public void PlaySound (AudioSource audio)
    {
        audio.Play();
    }
}
