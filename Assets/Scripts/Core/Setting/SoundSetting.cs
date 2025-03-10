using UnityEngine;

[CreateAssetMenu(fileName = "SoundSetting", menuName = "Scriptable Objects/Setting/Sound")]
public class SoundSetting : ScriptableSetting
{
	/// <summary>
	/// 전체 볼륨의 크기
	/// </summary>
	public float MasterVolume;
	/// <summary>
	/// 배경음의 크기
	/// </summary>
	public float BgmVolume;
	/// <summary>
	/// 효과음의 크기
	/// </summary>
	public float SfxVolume;
}
