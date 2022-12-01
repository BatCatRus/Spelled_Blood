using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource soundPlayer;

    public AudioSource soundPlayer1;

    public void playThisSoundEffect()
    {
        soundPlayer.Play();
    }

    public void playThisSoundEffect1()
    {
        soundPlayer1.Play();
    }
}
