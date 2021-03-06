﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct NameClipChannel {
	public string key;
	public AudioClip value;
	[Range(0,20)]
	public int channel;
}

public struct ClipChannelPair {
	public AudioClip audio;
	public int channel;
}

public class SoundManager : MonoBehaviour {
	private static SoundManager m_instance;
	private AudioSource[] soundChannels;
	[Tooltip("Music should go on channel 0, looping sound effects on channel 1, non-looping sound effects on channels 2-16.")]
	public NameClipChannel[] sounds_;
	private Dictionary<string, ClipChannelPair> sounds;
	
	float lowPitchRange = 0.9f;
	float highPitchRange = 1.1f;
	
	bool _easterEgg = false;
	public bool easterEgg {
		get {
			return _easterEgg;
		}
		set {
			if (value) {
				Play("PrettyNeatMusic");
			}
			_easterEgg = value;
			if (!value) {
				Play("TitleTheme");
			}
		}
	}

	bool _muted = false;
	public bool muted {
		get {
			return _muted;
		}
		set {
			if (value != _muted) {
				ToggleMuteMusic();
			}
		}
	}
	
	public static SoundManager instance {
		get {
			if (m_instance == null) {
				//m_instance = new GameObject ("SoundManager").AddComponent<SoundManager> ();                
			}
			return m_instance;
		}
	}
	
	void Awake ()
	{
		if (m_instance == null) {
			m_instance = this;            
		} else if (m_instance != this)        
			Destroy (gameObject);

		sounds = new Dictionary<string, ClipChannelPair>();
		int maxChannelNum = -1;
		for (int i = 0; i < sounds_.Length; i++) {
			ClipChannelPair newValue = new ClipChannelPair();
			newValue.audio = sounds_[i].value;
			newValue.channel = sounds_[i].channel;
			sounds.Add(sounds_[i].key, newValue);

			//Keep track of how many channels we're using
			if (newValue.channel > maxChannelNum) {
				maxChannelNum = newValue.channel;
			}
		}

		soundChannels = new AudioSource[maxChannelNum + 1];
		for (int i = 0; i <= maxChannelNum; i++) {
			soundChannels[i] = gameObject.AddComponent<AudioSource>();
			soundChannels[i].playOnAwake = false;
		}

		//Set music to looping
		soundChannels[0].loop = true;
		
		//Set looping sound effect channel to loop
		if (maxChannelNum > 0) {
			soundChannels[1].loop = true;
		}

		DontDestroyOnLoad (gameObject);
	}
	
	// Use this for initialization
	void Start () {
		
	}

	void Update() {
		soundChannels[0].volume = Options.masterVolume * Options.musicVolume;
	}
	
	public void Play(string soundName){
		//Only play the main theme and PressStart sound on the title screen
		if (GameManager.S.gameState == GameStates.titleScreen && soundName != "TitleTheme" && soundName != "PressStart") {
			return;
		}

		int channel = GetChannelIndex(soundName);
		if (easterEgg) {
			soundName = "PrettyNeat";
		}
		float randPitch = Random.Range(lowPitchRange, highPitchRange);

		//Music channel should not have random pitch
		if (channel == 0) {
			soundChannels[channel].clip = sounds[soundName].audio;
			soundChannels[channel].pitch = 1;
			soundChannels[channel].volume = Options.masterVolume * Options.musicVolume;
			soundChannels[channel].Play();
		}
		else {
			soundChannels[channel].clip = sounds[soundName].audio;
            soundChannels[channel].pitch = randPitch;
			soundChannels[channel].volume = Options.masterVolume * Options.sfVolume;
			soundChannels[channel].Play();
		}
	}
	
	public void Play(string soundName, float pitch){
		//Only play the main theme on the title screen
		if (GameManager.S.gameState == GameStates.titleScreen && soundName != "TitleTheme") {
			return;
		}

		int channel = GetChannelIndex(soundName);
		if (easterEgg) {
			soundName = "PrettyNeat";
		}

		//Music channel should not have random pitch
		if (channel == 0) {
			soundChannels[channel].clip = sounds[soundName].audio;
			soundChannels[channel].pitch = 1;
			soundChannels[channel].volume = Options.masterVolume * Options.musicVolume;
			soundChannels[channel].Play();
		}
		else {
			soundChannels[channel].clip = sounds[soundName].audio;
			soundChannels[channel].pitch = pitch;
			soundChannels[channel].volume = Options.masterVolume * Options.sfVolume;
			soundChannels[channel].Play();
		}
	}

	public bool IsPlaying(string soundName) {
		int channel = GetChannelIndex(soundName);

		return soundChannels[channel].isPlaying;
	}

	public void SetPitch(string soundName, float newPitch) {
		int channel = GetChannelIndex(soundName);

		soundChannels[channel].pitch = newPitch;
	}

	public void SetVolume(string soundName, float newVolume) {
		int channel = GetChannelIndex(soundName);
		float volumeScalarFromOptions = Options.masterVolume * ((channel == 0) ? Options.musicVolume : Options.sfVolume);

		soundChannels[channel].volume = newVolume * volumeScalarFromOptions;
	}

	public void Stop(string soundName) {
		int channel = GetChannelIndex(soundName);

		soundChannels[channel].Stop();
	}

	public AudioSource GetChannelOfSound(string soundName){
		return soundChannels[GetChannelIndex(soundName)];
	}
	
	private int GetChannelIndex(string soundName){
		if (sounds.ContainsKey(soundName)) {
			return sounds[soundName].channel;
		}
		else {
			Debug.LogError("Sound: " + soundName + " not found. Check the Sounds_ array in the inspector.");
			return -1;
		}
	}

	void ToggleMuteMusic() {
		_muted = !_muted;

		soundChannels[0].mute = _muted;
	}
}
