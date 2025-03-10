using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO[개선] 바인딩 지원하는 UIToolkit으로 기능 구현하기
/// <summary>
/// 설정을 변경할 수 있는 패널 GUI
/// </summary>
public class SettingPanel : UIBehaviour
{
	[Header("Graphics")]
	[SerializeField]
	GraphicsSetting _graphicsSetting;
	[SerializeField]
	TMP_Dropdown _targetFps;

	[Header("Sound")]
	[SerializeField]
	SoundSetting _soundSetting;
	[SerializeField]
	Slider _masterVolume;
	[SerializeField]
	Slider _bgmVolume;
	[SerializeField]
	Slider _sfxVolume;

	[Header("Play")]
	[SerializeField]
	PlaySetting _playSetting;
	[SerializeField]
	Slider _mapScale;

	protected override void Start()
	{
		#region Graphics
		_targetFps.value = (_graphicsSetting.TargetFrameRate / 30) - 1;
		_targetFps.onValueChanged.AddListener(new UnityAction<int>((i) =>
		{
			_graphicsSetting.TargetFrameRate = int.Parse(_targetFps.options[i].text);
			_graphicsSetting.SetDirty();
		}));
		#endregion

		#region Sound
		_masterVolume.value = _soundSetting.MasterVolume;
		_masterVolume.onValueChanged.AddListener(new UnityAction<float>((i) =>
		{
			_soundSetting.MasterVolume = i;
			_soundSetting.SetDirty();
		}));
		_bgmVolume.value = _soundSetting.BgmVolume;
		_bgmVolume.onValueChanged.AddListener(new UnityAction<float>((i) =>
		{
			_soundSetting.BgmVolume = i;
			_soundSetting.SetDirty();
		}));
		_sfxVolume.value = _soundSetting.SfxVolume;
		_sfxVolume.onValueChanged.AddListener(new UnityAction<float>((i) =>
		{
			_soundSetting.SfxVolume = i;
			_soundSetting.SetDirty();
		}));
		#endregion

		#region Play
		_mapScale.value = _playSetting.MapScale;
		_mapScale.onValueChanged.AddListener(new UnityAction<float>((i) =>
		{
			_playSetting.MapScale = i;
			_playSetting.SetDirty();
		}));
		#endregion
	}
}
