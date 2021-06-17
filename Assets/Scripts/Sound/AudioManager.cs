using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}

	public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

    public void Stop(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void PlayIntroAndLoop(string intro, string loop)
    {
        Sound i = Array.Find(sounds, item => item.name == intro);
        Sound l = Array.Find(sounds, item => item.name == loop);
        bool notFound = false;

        if (i == null)
        {
            Debug.LogWarning("Sound: " + intro + " not found!");
            notFound = true;
        }
        if (l == null)
        {
            Debug.LogWarning("Sound: " + loop + " not found!");
            notFound = true;
        }
        if(!notFound)
            StartCoroutine(PlayIntroAndLoop(i, l));
    }

    private IEnumerator PlayIntroAndLoop(Sound intro, Sound loop)
    {
        intro.source.volume = intro.volume * (1f + UnityEngine.Random.Range(-intro.volumeVariance / 2f, intro.volumeVariance / 2f));
        intro.source.pitch = intro.pitch * (1f + UnityEngine.Random.Range(-intro.pitchVariance / 2f, intro.pitchVariance / 2f));
        loop.source.volume = loop.volume * (1f + UnityEngine.Random.Range(-loop.volumeVariance / 2f, loop.volumeVariance / 2f));
        loop.source.pitch = loop.pitch * (1f + UnityEngine.Random.Range(-loop.pitchVariance / 2f, loop.pitchVariance / 2f));

        intro.source.Play();
        yield return new WaitForSecondsRealtime(intro.source.clip.length);
        loop.source.Play();
    }

}