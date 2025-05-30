using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 현재 맵의 상태를 보여주는 창
/// </summary>
[RequireComponent(typeof(Canvas))]
public class MapView : UIBehaviour
{
	[SerializeField]
	SlidePanel _panel;
	[SerializeField]
	RectTransform _mapParent;
	GraphicRaycaster _mapRaycaster;
	List<RaycastResult> _mapRaycastresult = new();

	/// <summary>
	/// 생성할 맵 뷰
	/// </summary>
	[SerializeField]
	GameObject _mapViewPrefab;
	BlockTheme[] _mapThemes;
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
		SettingManager.Instance.Play.SettingChanged += OnSettingChanged;
		//동기화
		OnSettingChanged();
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		SettingManager.Instance.Play.SettingChanged -= OnSettingChanged;
	}

	public void Init(BlockTheme[] mapThemes)
	{
		_mapThemes = mapThemes;
		_mapParent = _panel.GetComponent<RectTransform>();
		_mapRaycaster = _panel.GetComponentInParent<GraphicRaycaster>();
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
		_mapView.Init(map, _mapThemes[0]);
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
		_panel.Open();
	}
	public void Close()
	{
		_panel.Close();
	}

	void OnSettingChanged()
	{
		_panel.GetComponent<RectTransform>().localScale = Vector3.one * SettingManager.Instance.Play.MapScale;
	}
}
