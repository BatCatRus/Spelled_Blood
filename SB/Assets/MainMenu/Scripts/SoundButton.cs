using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    public AudioSource soundPlay;

    public void StartSound ()
    {
        soundPlay.Play ();
    }
}
