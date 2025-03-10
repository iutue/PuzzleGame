using System;
using UnityEngine;

/// <summary>
/// 직렬화해서 저장할 수 있는 설정 데이터
/// </summary>
public abstract class ScriptableSetting : ScriptableObject
{
	/// <summary>
	/// 설정값이 변경됐을 때
	/// </summary>
	public Action SettingChanged;

	protected void Awake()
	{
		Load();
	}

	/// <summary>
	/// 현재 설정 저장
	/// </summary>
	public void Save()
	{
		string json = JsonUtility.ToJson(this);
		PlayerPrefs.SetString(name, json);
	}

	/// <summary>
	/// 저장된 설정 불러오기
	/// </summary>
	public void Load()
	{
		string setting = PlayerPrefs.GetString(name);
		JsonUtility.FromJsonOverwrite(setting, this);
		SetDirty();
	}

	/// <summary>
	/// 수정된 설정값을 적용하고 싶을 때 호출해야 함
	/// </summary>
	public new void SetDirty()
	{
		Save();
		SettingChanged?.Invoke();
	}
}
