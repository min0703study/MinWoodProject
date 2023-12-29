using System;

using System.Collections.Generic;
using UnityEngine;

public class SoundManager: BaseManager<SoundManager>
{
	public enum SoundType
	{
		Bgm,
		SubBgm,
		Effect,
		Max,
	}
	
	private AudioSource[] audioSources = new AudioSource[(int)SoundType.Max];
	private Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();

	private GameObject soundRoot = null;

	protected override void init()
	{
		if (soundRoot == null)
		{
			soundRoot = GameObject.Find("@SoundRoot");
			if (soundRoot == null)
			{
				soundRoot = new GameObject { name = "@SoundRoot" };
				UnityEngine.Object.DontDestroyOnLoad(soundRoot);

				string[] soundTypeNames = System.Enum.GetNames(typeof(SoundType));
				for (int count = 0; count < soundTypeNames.Length - 1; count++)
				{
					GameObject go = new GameObject { name = soundTypeNames[count] };
					audioSources[count] = go.AddComponent<AudioSource>();
					go.transform.parent = soundRoot.transform;
				}

				audioSources[(int)SoundType.Bgm].loop = true;
				audioSources[(int)SoundType.SubBgm].loop = true;
			}
		}
	}

	public void Clear()
	{
		foreach (AudioSource audioSource in audioSources)
			audioSource.Stop();
		audioClipDict.Clear();
	}

	public void Play(SoundType type)
	{
		AudioSource audioSource = audioSources[(int)type];
		audioSource.Play();
	}

	public void Play(SoundType type, string key, float pitch = 1.0f)
	{
		AudioSource audioSource = audioSources[(int)type];

		if (type == SoundType.Bgm)
		{
			LoadAudioClip(key, (audioClip) =>
			{
				audioSource.pitch = pitch;
				
				if (audioSource.isPlaying)
					audioSource.Stop();

				audioSource.clip = audioClip;
				audioSource.Play();
				//if (Managers.Game.BGMOn)
			});
		}
		else if (type == SoundType.SubBgm)
		{
			LoadAudioClip(key, (audioClip) =>
			{
				if (audioSource.isPlaying)
					audioSource.Stop();

				audioSource.clip = audioClip;
				audioSource.Play();
			});
		}
		else
		{
			LoadAudioClip(key, (audioClip) =>
			{
				audioSource.pitch = pitch;
				audioSource.PlayOneShot(audioClip);
			});
		}
	}

	public void Play(SoundType type, AudioClip audioClip, float pitch = 1.0f)
	{
		AudioSource audioSource = audioSources[(int)type];

		if (type == SoundType.Bgm)
		{
			if (audioSource.isPlaying)
				audioSource.Stop();

			audioSource.clip = audioClip;
			audioSource.Play();
		}
		else if (type == SoundType.SubBgm)
		{
			if (audioSource.isPlaying)
				audioSource.Stop();

			audioSource.clip = audioClip;
			audioSource.Play();
		}
		else
		{
			audioSource.pitch = pitch;
			audioSource.PlayOneShot(audioClip);
		}
	}

	public void Stop(SoundType type)
	{
		AudioSource audioSource = audioSources[(int)type];
		audioSource.Stop();
	}
	
	private void LoadAudioClip(string key, Action<AudioClip> callback)
	{
		AudioClip audioClip = null;
		if (audioClipDict.TryGetValue(key, out audioClip))
		{
			callback?.Invoke(audioClip);
			return;
		}

		audioClip = ResourceManager.Instance.Load<AudioClip>(key);

		if (!audioClipDict.ContainsKey(key))
			audioClipDict.Add(key, audioClip);

		callback?.Invoke(audioClip);
	}
	
	public void PlayLevelUpSound()
	{
		Play(SoundType.Effect, "Sound_LevelUp");
	}

	public void PlayQuestComplete()
	{
		Play(SoundType.Effect, "Sound_QuestComplete");
	}
	
	public void PlayButtonPressSound()
	{
		Play(SoundType.Effect, "Sound_BeepUp");
	}

	public void PlayPopupClose()
	{
		Play(SoundType.Effect, "PopupClose_Common");
	}
	
	public void PlaySkillSound()
	{
		Play(SoundType.Effect, "Sound_SwordSkil");
	}

	
}
