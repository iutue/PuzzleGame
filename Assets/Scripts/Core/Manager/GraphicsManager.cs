using System;
using UnityEngine;

/// <summary>
/// 그래픽 효과를 관리하는 매니저
/// </summary>
public class GraphicsManager : SingletonBehaviour<GraphicsManager>
{
	[SerializeField]
	GraphicsSetting _graphicsSetting;

	protected void Start()
	{
		_graphicsSetting.SettingChanged += OnSettingChanged;
		//동기화
		OnSettingChanged();
	}

	void OnSettingChanged()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = _graphicsSetting.TargetFrameRate;
	}
}
