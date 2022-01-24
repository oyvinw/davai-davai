using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
public class Radio : MonoBehaviour
{
    AudioSource chatterAudio;

    public AudioClip radioButton;

    public AudioClip[] selected;
    public AudioClip[] affirmatives;
    public AudioClip[] deaths;
    public AudioClip[] enemyDeads;
    public AudioClip[] takingFires;
    public AudioClip[] enemySpotted;

    private void Start()
    {
        var audios = GetComponents<AudioSource>();
        chatterAudio = audios[0];
    }

    public void ReportSelected()
    {
        StartCoroutine(PlaySoundWithBeep(selected[Random.Range(0, selected.Length)]));
    }

    public void ReportAffirmative()
    {
        StartCoroutine(PlaySoundWithBeep(affirmatives[Random.Range(0, affirmatives.Length)]));
    }

    public void ReportSquaddieDead()
    {
        StartCoroutine(PlaySoundWithBeep(deaths[Random.Range(0, deaths.Length)]));
    }

    public void ReportEnemyDead()
    {
        StartCoroutine(PlaySoundWithBeep(enemyDeads[Random.Range(0, enemyDeads.Length)]));
    }

    public void ReportTakingFire()
    {
        StartCoroutine(PlaySoundWithBeep(takingFires[Random.Range(0, takingFires.Length)]));
    }
    public void EnemySpotted()
    {
        StartCoroutine(PlaySoundWithBeep(enemySpotted[Random.Range(0, enemySpotted.Length)]));
    }

    private IEnumerator PlaySoundWithBeep(AudioClip audioClip)
    {
        chatterAudio.clip = radioButton;
        chatterAudio.Play();
        yield return new WaitForSeconds(chatterAudio.clip.length);

        chatterAudio.clip = audioClip;
        chatterAudio.Play();
    }
}
