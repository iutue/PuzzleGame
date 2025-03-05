using DG.Tweening;
using UnityEngine;

public class SlidePanel : MonoBehaviour
{
	[SerializeField] float _duration = 0.5f;
	[SerializeField] Ease _easeType;

	/// <summary>
	/// 슬라이딩할 패널
	/// </summary>
	[Space]
	[SerializeField]
	RectTransform _panel;
	/// <summary>
	/// 창이 열려있는가
	/// </summary>
	[SerializeField] bool _isOpened;
	/// <summary>
	/// 열리기 전 위치
	/// </summary>
	Vector2 _openingPosition;
	/// <summary>
	/// 열린 후 위치
	/// </summary>
	Vector2 _openedPosition;
	/// <summary>
	/// 닫힌 후 위치
	/// </summary>
	Vector2 _closedPosition;
	/// <summary>
	/// 열릴 때 패널의 이동 방향
	/// </summary>
	[SerializeField] Vector2 _openDirection = Vector2.left;
	/// <summary>
	/// 닫힐 때 패널의 이동 방향
	/// </summary>
	[SerializeField] Vector2 _closeDirection = Vector2.left;
	/// <summary>
	/// 닫혔을 때 패널을 제거하는가
	/// </summary>
	[SerializeField] bool _destroyOnClose;

	/// <summary>
	/// 불투명도를 조절할 그룹
	/// </summary>
	[Space]
	[SerializeField]
	CanvasGroup _transparentGroup;
	/// <summary>
	/// 열렸을 때 패널의 불투명도
	/// </summary>
	[SerializeField, Range(0f, 1f)] float _openedAlpha = 1f;
	/// <summary>
	/// 닫혔을 때 패널의 불투명도
	/// </summary>
	[SerializeField, Range(0f, 1f)] float _closedAlpha = 0f;

	protected void Awake()
	{
		if (_panel)
		{
			//열렸을 때 위치 저장
			_openedPosition = _panel.anchoredPosition;
			//_panel.anchoredPosition = _isOpened ? _openedPosition : _closedPosition;
		}
		if (_transparentGroup)
		{
			_transparentGroup.alpha = _isOpened ? _openedAlpha : _closedAlpha;
		}
	}
	protected void OnEnable()
	{
		//CalculatePositions();
		if (_isOpened)
		{
			Open();
		}
		else
		{
			Close();
		}
	}
	protected void OnRectTransformDimensionsChange()
	{
		if (_panel)
		{
			//해상도에 맞는 위치로 이동
			CalculatePositions();
			_panel.anchoredPosition = _isOpened ? _openedPosition : _closedPosition;
		}
	}
	protected void OnDrawGizmosSelected()
	{
		if (_panel)
		{
			CalculatePositions();
			Transform canvasTr = GetComponentInParent<Canvas>().transform;
			Vector2 opened = canvasTr.TransformPoint(_openedPosition);
			Vector2 opening = canvasTr.TransformPoint(_openingPosition);
			Vector2 closed = canvasTr.TransformPoint(_closedPosition);

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(opening, 50f);
			Gizmos.DrawLine(opening, opened);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(opened, 50f);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(closed, 50f);
			Gizmos.DrawLine(opened, closed);
		}
	}

	/// <summary>
	/// 루트 캔버스 크기에 비례하는 상태별 패널 위치 계산
	/// </summary>
	void CalculatePositions()
	{
		Canvas rootCanvas = _panel.GetComponentInParent<Canvas>().rootCanvas;
		Vector2 rootCanvasSize = rootCanvas.GetComponent<RectTransform>().rect.size;
		_openingPosition = _openedPosition - _openDirection * rootCanvasSize;
		_closedPosition = _openedPosition + _closeDirection * rootCanvasSize;
	}

	/// <summary>
	/// 패널 슬라이드 인
	/// </summary>
	public void Open()
	{
		_isOpened = true;
		//이동
		if (_panel)
		{
			_panel
				.DOAnchorPos(_openedPosition, _duration)
				.From(_openingPosition)
				.SetEase(_easeType);
		}
		//불투명도
		if (_transparentGroup)
		{
			_transparentGroup
				.DOFade(_openedAlpha, _duration)
				.From(_closedAlpha)
				.SetEase(_easeType);
		}
	}
	/// <summary>
	/// 패널 슬라이드 아웃
	/// </summary>
	public void Close()
	{
		_isOpened = false;
		//이동
		if (_panel)
		{
			_panel
				.DOAnchorPos(_closedPosition, _duration)
				.From(_openedPosition)
				.SetEase(_easeType)
				.onComplete = () =>
				{
					if (_destroyOnClose)
					{
						Destroy(gameObject);
					}
				};
		}
		if (_transparentGroup)
		{
			//불투명도
			_transparentGroup
				.DOFade(_closedAlpha, _duration)
				.From(_openedAlpha)
				.SetEase(_easeType);
		}
	}
}
