using UnityEngine;

public class SettingManager : SingletonBehaviour<SettingManager>
{
	[Header("Graphics")]
	public int TargetFrameRate = 60;

	protected void Start()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = TargetFrameRate;
	}
}
