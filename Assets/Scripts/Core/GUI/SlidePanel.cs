using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static SlidePanel;

/// <summary>
/// 슬라이드해서 열고 닫을 수 있는 패널 GUI
/// </summary>
public class SlidePanel : UIBehaviour
{
	[SerializeField] float _openDelay = 0f;
	[SerializeField] float _closeDelay = 0f;
	[SerializeField] float _duration = 0.5f;
	[SerializeField] Ease _easeType = Ease.OutExpo;

	/// <summary>
	/// 패널의 상태
	/// </summary>
	public enum State
	{
		/// <summary>
		/// 화면에 있음
		/// </summary>
		Idle,
		/// <summary>
		/// 화면에 나타나는 중
		/// </summary>
		Show,
		/// <summary>
		/// 화면에서 사라지는 중
		/// </summary>
		Hide,
		/// <summary>
		/// 다음 창으로 넘어가는 중
		/// </summary>
		Next,
		/// <summary>
		/// 이전 창으로 돌아가는 중
		/// </summary>
		Prev
	}
	[Serializable]
	public class StateData
	{
		public State State;
		/// <summary>
		/// 대기 시간
		/// </summary>
		public float Delay = 0f;
		/// <summary>
		/// 재생 시간
		/// </summary>
		public float Duration = 0.5f;
		/// <summary>
		/// 재생 스타일
		/// </summary>
		public Ease EaseType = Ease.OutExpo;
		/// <summary>
		/// 슬라이드 방향
		/// </summary>
		[Range(0f, 360f)]
		public float SlideDirection;
		/// <summary>
		/// 슬라이드 거리, 경계 크기에 비례함
		/// </summary>
		[Range(0f, 1f)]
		public float SlideDistance = 1f;
		/// <summary>
		/// 크기
		/// </summary>
		[Range(0f, 2f)]
		public float Scale = 1f;
		/// <summary>
		/// 불투명도
		/// </summary>
		[Range(0f, 1f)]
		public float Alpha = 1f;
	}
	/// <summary>
	/// 상태에 해당하는 애니메이션 정보
	/// </summary>
	[SerializeField]
	StateData[] _stateDatas = new StateData[0];

	#region Move
	/// <summary>
	/// 패널을 슬라이딩할 때 이동 거리의 단위가 될 GUI
	/// </summary>
	[SerializeField]
	RectTransform _boundary;
	/// <summary>
	/// 슬라이딩할 패널
	/// </summary>
	[Space]
	[SerializeField]
	RectTransform _panel;
	Canvas _canvas;
	/// <summary>
	/// 패널이 열려있는가
	/// </summary>
	[SerializeField]
	bool _isOpened;
	/// <summary>
	/// 초기화 후 자동으로 애니메이션을 재생하는가
	/// </summary>
	[SerializeField]
	bool _playOnAwake;
	/// <summary>
	/// 닫혔을 때 제거할 게임 오브젝트
	/// </summary>
	[SerializeField]
	GameObject _destroyOnClose;
	/// <summary>
	/// 패널 위치가 초기화됐는가<br/>
	/// true면 직렬화된 현재 위치로 고정되고, false면 초기화시 위치로 고정됨
	/// </summary>
	[SerializeField]
	bool _isPositionInitialized;
	/// <summary>
	/// 열리기 전 위치
	/// </summary>
	Vector2 _openingPosition;
	/// <summary>
	/// 열린 후 위치
	/// </summary>
	[SerializeField]
	Vector2 _openedPosition;
	/// <summary>
	/// 닫힌 후 위치
	/// </summary>
	Vector2 _closedPosition;
	/// <summary>
	/// 열릴 때 패널의 이동 방향과 거리
	/// </summary>
	[Tooltip(
		"이동 거리는 경계의 크기에 비례하고 크기가 1이면 경계의 크기만큼 이동한다.\n" +
		"(열리기 전 위치) = (direction) X (boundary.size)")]
	[SerializeField]
	Vector2 _openDirection = Vector2.left;
	/// <summary>
	/// 닫힐 때 패널의 이동 방향과 거리
	/// </summary>
	[Tooltip(
		"이동 거리는 경계의 크기에 비례하고 크기가 1이면 경계의 크기만큼 이동한다.\n" +
		"(닫힌 후 위치) = (direction) X (boundary.size)")]
	[SerializeField]
	Vector2 _closeDirection = Vector2.left;
	#endregion

	#region Alpha
	/// <summary>
	/// 불투명도를 조절할 그룹
	/// </summary>
	[Space]
	[SerializeField]
	CanvasGroup _transparentGroup;
	/// <summary>
	/// 열렸을 때 패널의 불투명도
	/// </summary>
	[SerializeField, Range(0f, 1f)]
	float _openedAlpha = 1f;
	/// <summary>
	/// 닫혔을 때 패널의 불투명도
	/// </summary>
	[SerializeField, Range(0f, 1f)]
	float _closedAlpha = 0f;
	#endregion

	protected override void OnValidate()
	{
		base.OnValidate();

		Array states = Enum.GetValues(typeof(State));
		Array.Resize(ref _stateDatas, states.Length);
		for (int i = 0; i < states.Length; i++)
		{
			_stateDatas[i].State = (State)states.GetValue(i);
		}

		//경계 초기화
		InitializeBoundary();
		//위치 초기화
		if (_panel && !_isPositionInitialized)
		{
			_openedPosition = _panel.anchoredPosition;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		//경계 초기화
		InitializeBoundary();
		//위치 초기화
		if (_panel && !_isPositionInitialized)
		{
			_isPositionInitialized = true;
			_openedPosition = _panel.anchoredPosition;
		}
		CalculatePositions();
		//상태 초기화
		if (_playOnAwake)
		{
			//부드럽게 상태 초기화
			if (_isOpened)
			{
				Open();
			}
			else
			{
				Close();
			}
		}
		else
		{
			//즉시 상태 초기화
			if (_panel) _panel.anchoredPosition = _isOpened ? _openedPosition : _closedPosition;
			if (_transparentGroup) _transparentGroup.alpha = _isOpened ? _openedAlpha : _closedAlpha;
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		if (_panel && _isPositionInitialized)
		{
			//해상도에 맞는 위치로 즉시 이동
			CalculatePositions();
			_panel.anchoredPosition = _isOpened ? _openedPosition : _closedPosition;
		}
	}

	protected void OnDrawGizmosSelected()
	{
		if (_panel && _boundary)
		{
			CalculatePositions();
			//로컬 좌표를 월드 좌표로 변환
			var parentTr = transform.parent;
			Vector2 opened = parentTr.TransformPoint(_openedPosition);
			Vector2 opening = parentTr.TransformPoint(_openingPosition);
			Vector2 closed = parentTr.TransformPoint(_closedPosition);
			//열리기 전 위치
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(opening, 50f);
			Gizmos.DrawLine(opening, opened);
			//열린 후 위치
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(opened, 50f);
			//닫힌 후 위치
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(closed, 50f);
			Gizmos.DrawLine(opened, closed);
		}
	}

	void InitializeBoundary()
	{
		if (!_panel)
		{
			//이동하지 않으면 경계가 필요 없음
			_boundary = null;
			return;
		}
		if (_boundary)
		{
			//설정된 경계가 이미 있음
			return;
		}
		Canvas canvas = _panel.GetComponentInParent<Canvas>();
		if (!canvas)
		{
			//캔버스 내 UI 엘리먼트가 아님
			_boundary = null;
			return;
		}
		//임의의 경계가 없으면 루트 캔버스(화면)를 경계로 사용
		_boundary = canvas.rootCanvas.GetComponent<RectTransform>();
	}

	/// <summary>
	/// 경계의 크기에 비례하는 상태별 패널 위치 계산
	/// </summary>
	void CalculatePositions()
	{
		Vector2 boundarySize = _boundary.rect.size;
		_openingPosition = _openedPosition - _openDirection * boundarySize;
		_closedPosition = _openedPosition + _closeDirection * boundarySize;
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
				.SetEase(_easeType)
				.SetDelay(_openDelay);
		}
		//불투명도
		if (_transparentGroup)
		{
			_transparentGroup
				.DOFade(_openedAlpha, _duration)
				.From(_closedAlpha)
				.SetEase(_easeType)
				.SetDelay(_openDelay);
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
				.SetDelay(_closeDelay)
				.onComplete = () =>
				{
					if (_destroyOnClose)
					{
						Destroy(_destroyOnClose);
					}
				};
		}
		if (_transparentGroup)
		{
			//불투명도
			_transparentGroup
				.DOFade(_closedAlpha, _duration)
				.From(_openedAlpha)
				.SetEase(_easeType)
				.SetDelay(_closeDelay);
		}
	}
}
