using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 현재 맵의 상태를 보여주는 창
/// </summary>
[RequireComponent(typeof(Canvas))]
public class MapCanvas : UIBehaviour
{
	[SerializeField]
	PlaySetting _playSetting;
	GameModeSetting _gameModeSetting;

	[SerializeField]
	SlidePanel _map;
	[SerializeField]
	RectTransform _mapParent;
	GraphicRaycaster _mapRaycaster;
	List<RaycastResult> _mapRaycastresult = new();

	/// <summary>
	/// 생성할 맵 뷰
	/// </summary>
	[SerializeField]
	GameObject _mapViewPrefab;
	/// <summary>
	/// 생성된 맵 뷰
	/// </summary>
	BlockGroupView _mapView;
	/// <summary>
	/// 스케일을 적용한 맵 블록의 크기
	/// </summary>
	public Vector2 BlockViewSize => _mapView.BlockViewSize * _mapParent.localScale;

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

	public void Init(GameModeSetting gameModeSetting)
	{
		_gameModeSetting = gameModeSetting;
		_mapParent = _map.GetComponent<RectTransform>();
		_mapRaycaster = _map.GetComponentInParent<GraphicRaycaster>();
	}

	/// <summary>
	/// 맵 뷰 초기화
	/// </summary>
	public void ResetMap(BlockGroup map)
	{
		//기존의 뷰 제거
		if (_mapView)
		{
			Destroy(_mapView.gameObject);
		}
		//새로운 뷰 생성
		_mapView = Instantiate(_mapViewPrefab, _mapParent).GetComponent<BlockGroupView>();
		_mapView.Init(map, _gameModeSetting.MapTheme);
	}

	/// <summary>
	/// 맵 뷰에서 해당 위치의 블록 뷰 검출
	/// </summary>
	public BlockView GetMapBlockAt(Vector2 position)
	{
		//TODO[개선] 맵 패널 위 포인터의 위치로 블록 위치 계산하기
		PointerEventData eventData = new PointerEventData(EventSystem.current);
		eventData.position = position;
		_mapRaycastresult.Clear();
		_mapRaycaster.Raycast(eventData, _mapRaycastresult);
		foreach (var element in _mapRaycastresult)
		{
			if (element.gameObject.TryGetComponent<BlockView>(out var origin))
			{
				return origin;
			}
		}
		return null;
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
