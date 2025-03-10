using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
public class MapCanvas : UIBehaviour
{
	[SerializeField]
	PlaySetting _playSetting;

	[SerializeField]
	SlidePanel _map;

	protected override void OnEnable()
	{
		base.OnEnable();
		_playSetting.SettingChanged += OnSettingChanged;
		//동기화
		OnSettingChanged();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		_playSetting.SettingChanged -= OnSettingChanged;
	}

	public void Open()
	{
		_map.Open();
	}
	public void Close()
	{
		_map.Close();
	}

	void OnSettingChanged()
	{
		_map.GetComponent<RectTransform>().localScale = Vector3.one * _playSetting.MapScale;
	}
}
