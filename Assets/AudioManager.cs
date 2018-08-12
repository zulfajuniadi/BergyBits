using System.Collections.Generic;
using UnityEngine;

class AudioManager : MonoBehaviour
{
    static AudioManager _instance;
    static GameManager _gameManager;
    static GameManager gameManager
    {
        get
        {
            if (_gameManager == null)
            {
                _gameManager = GameObject.FindObjectOfType<GameManager>();
            }
            return _gameManager;
        }
    }

    [SerializeField]
    AudioClip addFishSound, jumpSound, minionSpawnedSound, fishFellSound, fellSound, headButt, dissolvedSound;

    Queue<AudioSource> idleSources = new Queue<AudioSource>();
    List<AudioSource> playingSources = new List<AudioSource>();

    private void Awake()
    {
        _instance = this;
        var go = new GameObject("AudioSources");
        go.transform.parent = transform;
        for (int i = 0; i < 20; i++)
        {
            var source = go.AddComponent<AudioSource>();
            source.pitch = Random.Range(0.9f, 1.1f);
            source.volume = 0.2f;
            source.loop = false;
            source.playOnAwake = false;
            idleSources.Enqueue(source);
        }
    }

    private void Update()
    {
        for (int i = playingSources.Count - 1; i > -1; i--)
        {
            if (!playingSources[i].isPlaying)
            {
                idleSources.Enqueue(playingSources[i]);
                playingSources.RemoveAt(i);
            }
        }
    }

    public static void PlayAddFish()
    {
        _instance.Play(_instance.addFishSound);
    }

    public static void PlayFishFell()
    {
        _instance.Play(_instance.fishFellSound);
    }

    public static void PlayJump()
    {
        _instance.Play(_instance.jumpSound);
    }

    public static void PlayMinionSpawned()
    {
        _instance.Play(_instance.minionSpawnedSound);
    }

    public static void PlayHeadButt()
    {
        _instance.Play(_instance.headButt);
    }

    public static void PlayFell()
    {
        _instance.Play(_instance.fellSound);
    }

    public static void PlayDissolved()
    {
        _instance.Play(_instance.dissolvedSound);
    }

    private void Play(AudioClip clip)
    {
        if (idleSources.Count == 0) return;
        var source = idleSources.Dequeue();
        source.clip = clip;
        source.Play();
        playingSources.Add(source);
    }
}
