using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct NameClipChannel {
	public string key;
	public AudioClip value;
	[Range(0,8)]
	public int channel;
}

public struct ClipChannelPair {
	public AudioClip audio;
	public int channel;
}

public class SoundManager : MonoBehaviour {
	private static SoundManager m_instance;
	private AudioSource[] soundChannels;
	[Header("Music should go on channel 0, sound effects on channels 1-8.")]
	public NameClipChannel[] sounds_;
	private Dictionary<string, ClipChannelPair> sounds;
	
	float lowPitchRange = 0.9f;
	float highPitchRange = 1.1f;

	bool _muted = false;
	public bool muted {
		get {
			return _muted;
		}
		set {
			if (value != _muted) {
				ToggleMute();
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
		soundChannels[0].volume = 0.75f;

		DontDestroyOnLoad (gameObject);
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	public void Play(string soundName){
		//Only play the main theme on the title screen
		if (Application.loadedLevelName == GameManager.S.titleSceneName && soundName != "MainTheme") {
			return;
		}

		int channel = GetChannelIndex(soundName);
		float randPitch = Random.Range(lowPitchRange, highPitchRange);

		//Music channel should not have random pitch
		if (channel == 0) {
			soundChannels[channel].clip = sounds[soundName].audio;
			soundChannels[channel].pitch = 1;
			soundChannels[channel].Play();
		}
		else {
			soundChannels[channel].clip = sounds[soundName].audio;
			soundChannels[channel].pitch = randPitch;
			soundChannels[channel].Play();
		}
	}
	
	public void Play(string soundName, float pitch){
		//Only play the main theme on the title screen
		if (Application.loadedLevelName == GameManager.S.titleSceneName && soundName != "MainTheme") {
			return;
		}

		int channel = GetChannelIndex(soundName);

		//Music channel should not have random pitch
		if (channel == 0) {
			soundChannels[channel].clip = sounds[soundName].audio;
			soundChannels[channel].pitch = 1;
			soundChannels[channel].Play();
		}
		else {
			soundChannels[channel].clip = sounds[soundName].audio;
			soundChannels[channel].pitch = pitch;
			soundChannels[channel].Play();
		}
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

	void ToggleMute() {
		_muted = !_muted;
		
		foreach (var channel in soundChannels) {
			channel.mute = muted;
		}
	}
}
