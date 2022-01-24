using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<string> songKeys;
    public List<AudioClip> songValues;
    private AudioSource songSource;
    public Dictionary<string, AudioClip> songLib = new Dictionary<string, AudioClip>();

    public List<string> soundKeys;
    public List<AudioClip> soundValues;
    private AudioSource soundSource;
    public Dictionary<string, AudioClip> soundLib = new Dictionary<string, AudioClip>();

    public float sfxSoundLevel = 100;
    public float musicSoundLevel = 100;

    public void OnBeforeSerialize()
    {
        songKeys.Clear();
        songValues.Clear();

        foreach (var kvp in songLib)
        {
            songKeys.Add(kvp.Key);
            songValues.Add(kvp.Value);
        }

        soundKeys.Clear();
        soundValues.Clear();
        foreach (var kvp in soundLib)
        {
            soundKeys.Add(kvp.Key);
            soundValues.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        for (int i = 0; i != Mathf.Min(songKeys.Count, songValues.Count); i++)
            songLib.Add(songKeys[i], songValues[i]);

        for (int i = 0; i != Mathf.Min(soundKeys.Count, songKeys.Count); i++)
            soundLib.Add(soundKeys[i], soundValues[i]);
    }

    void OnGUI()
    {
        foreach (var kvp in soundLib)
            GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);

        foreach (var kvp in songLib)
            GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        var audioSources = GetComponentsInChildren<AudioSource>();

        songSource = audioSources[0];
        soundSource = audioSources[1];

        songSource.volume = musicSoundLevel;
        soundSource.volume = sfxSoundLevel;
    }

    public void PlaySound(string soundName)
    {
        if (soundLib.ContainsKey(soundName))
        {
            soundSource.clip = soundLib[soundName];
            soundSource.Play();
        }
        else
        {
            Debug.Log($"Sound {soundName} does not exist in the sound library");
        }
    }

    public void PlaySong(string songName)
    {
        if (soundLib.ContainsKey(songName))
        {
            songSource.clip = soundLib[songName];
            songSource.Play();
        }
        else
        {
            Debug.Log($"Song {songName} does not exist in the sound library");
        }
    }
}
