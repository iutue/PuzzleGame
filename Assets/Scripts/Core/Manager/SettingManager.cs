using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 플레이어 및 매치의 설정을 관리하는 매니저
/// </summary>
public class SettingManager : SingletonBehaviour<SettingManager>
{
	[field: Header("Graphics")]
	[field: SerializeField]
	public GraphicsSetting Graphics { get; private set; }

	[field: Header("Sound")]
	[field: SerializeField]
	public SoundSetting Sound { get; private set; }
	[SerializeField]
	AudioMixer _mixer;
	[SerializeField]
	AudioSource _bgmSource;
	[SerializeField]
	AudioSource _sfxSource;

	[field: Header("Play")]
	[field: SerializeField]
	public PlaySetting Play { get; private set; }

	protected void Start()
	{
		Graphics.SettingChanged += OnGraphicsSettingChanged;
		OnGraphicsSettingChanged();
		Sound.SettingChanged += OnSoundSettingChanged;
		OnSoundSettingChanged();
	}

	#region Graphics
	void OnGraphicsSettingChanged()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = Graphics.TargetFrameRate;
	}
	#endregion

	#region Sound
	void OnSoundSettingChanged()
	{
		_mixer.SetFloat("MasterVolume", Sound.MasterVolume);
		_mixer.SetFloat("BgmVolume", Sound.BgmVolume);
		_mixer.SetFloat("SfxVolume", Sound.SfxVolume);
	}

	/// <summary>
	/// 효과음 재생
	/// </summary>
	public void PlaySoundEffect(AudioClip clip)
	{
		_sfxSource.PlayOneShot(clip);
	}
	#endregion
}
