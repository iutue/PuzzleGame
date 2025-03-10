using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 음악, 음향 효과를 관리하는 매니저
/// </summary>
public class SoundManager : SingletonBehaviour<SoundManager>
{
	[SerializeField]
	SoundSetting _soundSetting;

	[SerializeField]
	AudioMixer _mixer;
	[SerializeField]
	AudioSource _bgmSource;
	[SerializeField]
	AudioSource _sfxSource;

	protected void Start()
	{
		_soundSetting.SettingChanged += OnSettingChanged;
		//동기화
		OnSettingChanged();
	}

	void OnSettingChanged()
	{
		_mixer.SetFloat("MasterVolume", _soundSetting.MasterVolume);
		_mixer.SetFloat("BgmVolume", _soundSetting.BgmVolume);
		_mixer.SetFloat("SfxVolume", _soundSetting.SfxVolume);
	}

	/// <summary>
	/// 효과음 재생
	/// </summary>
	public void PlaySoundEffect(AudioClip clip)
	{
		_sfxSource.PlayOneShot(clip);
	}
}
