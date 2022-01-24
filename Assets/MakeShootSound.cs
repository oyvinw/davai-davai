using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeShootSound : MonoBehaviour
{
    public AudioClip[] gunShots;
    private AudioSource gunAudio;

    private void Start()
    {
        gunAudio = GetComponent<AudioSource>(); 
    }

    public void BangBang()
    {
        gunAudio.clip = (gunShots[Random.Range(0, gunShots.Length)]); 
        gunAudio.Play();
    }
}
