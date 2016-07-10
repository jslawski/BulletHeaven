using UnityEngine;
using System.Collections;
using JSONObj;

public class Options : MonoBehaviour {
	const string PLAYERPREFS_OPTIONS = "OptionsConfig";

	private static readonly int numRoundsDefault = 3;
	private static int _numRounds;
	public  static int numRounds {
		get {
			return _numRounds;
		}
		set {
			if (_numRounds == value) {
				return;
			}
			_numRounds = value;
			SaveOptionsToPlayerPrefs();
		}
	}

	private static readonly float minDamageAmpDefault = 1f;
	private static float _minDamageAmp;
	public  static float minDamageAmp {
		get {
			return _minDamageAmp;
		}
		set {
			if (_minDamageAmp == value) {
				return;
			}
			_minDamageAmp = value;
			SaveOptionsToPlayerPrefs();
		}
	}

	private static readonly float maxDamageAmpDefault = 3f;
	private static float _maxDamageAmp;
	public static float maxDamageAmp {
		get {
			return _maxDamageAmp;
		}
		set {
			if (_maxDamageAmp == value) {
				return;
			}
			_maxDamageAmp = value;
			SaveOptionsToPlayerPrefs();
		}
	}

	private static readonly float damageAmpTimeDefault = 120f;
	private static float _damageAmpTime;
	public static float damageAmpTime {
		get {
			return _damageAmpTime;
		}
		set {
			if (_damageAmpTime == value) {
				return;
			}
			_damageAmpTime = value;
			SaveOptionsToPlayerPrefs();
		}
	}

	private static readonly float masterVolumeDefault = 1f;
	private static float _masterVolume;
	public  static float masterVolume {
		get {
			return _masterVolume;
		}
		set {
			if (_masterVolume == value) {
				return;
			}
			_masterVolume = value;
			SaveOptionsToPlayerPrefs();
		}
	}

	private static readonly float musicVolumeDefault = 1f;
	private static float _musicVolume;
	public  static float musicVolume {
		get {
			return _musicVolume;
		}
		set {
			if (_musicVolume == value) {
				return;
			}
			_musicVolume = value;
			SaveOptionsToPlayerPrefs();
		}
	}

	private static readonly float sfVolumeDefault = 1f;
	private static float _sfVolume;
	public  static float sfVolume {
		get {
			return _sfVolume;
		}
		set {
			if (_sfVolume == value) {
				return;
			}
			_sfVolume = value;
			SaveOptionsToPlayerPrefs();
		}
	}

	void Awake() {
		LoadOptionsFromPlayerPrefs();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void LoadOptionsFromPlayerPrefs() {
		SetDefaultOptions();
		if (!PlayerPrefs.HasKey(PLAYERPREFS_OPTIONS)) {
			return;
		}
		JSONObject playerPrefsJSON = new JSONObject(PlayerPrefs.GetString(PLAYERPREFS_OPTIONS));
		print(playerPrefsJSON.ToString());

		if (playerPrefsJSON == null || playerPrefsJSON.keys == null) {
			PlayerPrefs.DeleteKey(PLAYERPREFS_OPTIONS);
			return;
		}
		
		foreach (var key in playerPrefsJSON.keys) {
			JSONObject option = playerPrefsJSON[key];

			switch (key) {
				case "numRounds":
					numRounds = Mathf.RoundToInt(option.n);
					break;
				case "minDamageAmp":
					minDamageAmp = option.n;
					break;
				case "maxDamageAmp":
					maxDamageAmp = option.n;
					break;
				case "damageAmpTime":
					damageAmpTime = option.n;
					break;
				case "masterVolume":
					masterVolume = option.n;
					break;
				case "musicVolume":
					musicVolume = option.n;
					break;
				case "sfVolume":
					sfVolume = option.n;
					break;
				default:
					Debug.LogError("Key " + key + " not recognized while loading options from PlayerPrefs.");
					break;
			}
		}
	}
	public static void SaveOptionsToPlayerPrefs(){
		JSONObject playerPrefsJSONToBeSaved = new JSONObject();

		//Only save options whose values have been changed from the defaults
		if (numRounds != numRoundsDefault) {
			playerPrefsJSONToBeSaved.AddField("numRounds", numRounds);
		}
		if (masterVolume != masterVolumeDefault) {
			playerPrefsJSONToBeSaved.AddField("masterVolume", masterVolume);
		}
		if (musicVolume != musicVolumeDefault) {
			playerPrefsJSONToBeSaved.AddField("musicVolume", musicVolume);
		}
		if (sfVolume != sfVolumeDefault) {
			playerPrefsJSONToBeSaved.AddField("sfVolume", sfVolume);
		}
		if (minDamageAmp != minDamageAmpDefault) {
			playerPrefsJSONToBeSaved.AddField("minDamageAmp", minDamageAmp);
		}
		if (maxDamageAmp != maxDamageAmpDefault) {
			playerPrefsJSONToBeSaved.AddField("maxDamageAmp", maxDamageAmp);
		}
		if (damageAmpTime != damageAmpTimeDefault) {
			playerPrefsJSONToBeSaved.AddField("damageAmpTime", damageAmpTime);
		}

		if (playerPrefsJSONToBeSaved.keys != null) {
			print("Saved to PlayerPrefs:\n" + playerPrefsJSONToBeSaved.ToString());
			PlayerPrefs.SetString(PLAYERPREFS_OPTIONS, playerPrefsJSONToBeSaved.ToString());
		}
		else {
			PlayerPrefs.DeleteKey(PLAYERPREFS_OPTIONS);
		}
	}

	static void SetDefaultOptions() {
		_numRounds = numRoundsDefault;

		_minDamageAmp = minDamageAmpDefault;
		_maxDamageAmp = maxDamageAmpDefault;
		_damageAmpTime = damageAmpTimeDefault;

		_masterVolume = masterVolumeDefault;
		_musicVolume = musicVolumeDefault;
		_sfVolume = sfVolumeDefault;
	}
}
