using DG.Tweening;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 슬라이드해서 열고 닫을 수 있는 패널 GUI
/// </summary>
public class SlidePanel : UIBehaviour
{

	/// <summary>
	/// 초기화 후 자동으로 애니메이션을 재생하는가
	/// </summary>
	[Tooltip("자동으로 슬라이드 인/아웃 애니메이션을 재생하는가")]
	[SerializeField]
	bool _playOnAwake;
	/// <summary>
	/// 현재 상태
	/// </summary>
	[SerializeField]
	State _currentState = State.Focus;
	/// <summary>
	/// 슬라이드인 설정
	/// </summary>
	StateSetting _slideInSetting => _stateSettings[(int)State.Focus];
	/// <summary>
	/// 패널이 열려있는가
	/// </summary>
	bool _isOpened => _currentState == State.Focus;
	[SerializeField]
	Ease _defaultEase;

	/// <summary>
	/// 슬라이딩할 패널
	/// </summary>
	[Space]
	[SerializeField]
	RectTransform _panelToSlide;
	/// <summary>
	/// 패널을 슬라이딩할 때 이동 거리의 단위가 될 GUI
	/// </summary>
	[SerializeField]
	RectTransform _boundary;

	/// <summary>
	/// 스케일링할 패널
	/// </summary>
	[Space]
	[SerializeField]
	RectTransform _panelToScale;

	/// <summary>
	/// 불투명도를 조절할 그룹
	/// </summary>
	[Space]
	[SerializeField]
	CanvasGroup _transparentGroup;
	/// <summary>
	/// 슬라이드 아웃시 비활성화할 캔버스
	/// </summary>
	[SerializeField]
	Canvas _canvasToDisable;

	/// <summary>
	/// 패널의 상태
	/// </summary>
	public enum State
	{
		#region SlideIn 상태
		/// <summary>
		/// 화면에서 보는 중
		/// </summary>
		Focus,
		#endregion
		#region SlideOut 상태
		/// <summary>
		/// 화면에 나타나기 전
		/// </summary>
		Show,
		/// <summary>
		/// 화면에서 사라진 후
		/// </summary>
		Hide,
		/// <summary>
		/// 다음으로 넘어감
		/// </summary>
		Next,
		/// <summary>
		/// 이전으로 돌아감
		/// </summary>
		Prev
		#endregion
	}
	/// <summary>
	/// 상태와 이에 대응하는 애니메이션 설정
	/// </summary>
	[Serializable]
	public class StateSetting
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
		public Ease CustomEase = Ease.Unset;

		/// <summary>
		/// 슬라이드 방향
		/// </summary>
		[Range(0f, 360f)]
		public float SlideAngle = 180f;
		/// <summary>
		/// 슬라이드 거리, 경계 크기에 비례함
		/// </summary>
		[Range(0f, 2f)]
		public float SlideDistance;
		/// <summary>
		/// 슬라이드 위치가 고정됐는가
		/// true면 위치 수동 고정
		/// false면 위치 자동 계산
		/// </summary>
		public bool IsLocked;
		/// <summary>
		/// 방향, 거리, 경계로 계산된 슬라이드 위치
		/// </summary>
		public Vector3 SlidePosition;

		/// <summary>
		/// 크기
		/// </summary>
		[Range(0f, 2f)]
		public float Scale = 1f;

		/// <summary>
		/// 불투명도
		/// </summary>
		[Range(0f, 1f)]
		public float Alpha = 0f;

		/// <summary>
		/// 주어진 위치를 기준으로 이 상태의 슬라이드 위치 계산
		/// </summary>
		public void UpdateSlidePosition(Vector2 position, Vector2 boundarySize)
		{
			if (IsLocked)
			{
				//이 상태의 위치가 고정됨
				return;
			}
			float radian = SlideAngle * Mathf.Deg2Rad;
			Vector2 slideVector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * SlideDistance;
			SlidePosition = position + slideVector * boundarySize;
		}
	}
	[SerializeField]
	StateSetting[] _stateSettings = new StateSetting[0];

#if UNITY_EDITOR
	protected override void OnValidate()
	{
		base.OnValidate();
		//패널의 모든 상태에 대응하는 세팅 생성
		Array states = Enum.GetValues(typeof(State));
		Array.Resize(ref _stateSettings, states.Length);
		for (int i = 0; i < states.Length; i++)
		{
			_stateSettings[i] ??= new StateSetting();
			_stateSettings[i].State = (State)states.GetValue(i);
		}

		//경계 초기화
		InitializeBoundary();
		//현재 위치로 설정
		if (_panelToSlide && !_slideInSetting.IsLocked)
		{
			_slideInSetting.SlidePosition = _panelToSlide.anchoredPosition;
		}
	}
#endif

	protected override void Awake()
	{
		base.Awake();
		//경계 초기화
		InitializeBoundary();
		//위치 초기화
		if (_panelToSlide && !_slideInSetting.IsLocked)
		{
			//초기 위치 저장
			_slideInSetting.IsLocked = true;
			_slideInSetting.SlidePosition = _panelToSlide.anchoredPosition;
		}
		UpdatePositions();
		//상태 초기화
		if (_playOnAwake)
		{
			//부드럽게 초기화
			if (_currentState == State.Focus)
			{
				//슬라이드 인
				ChangeState(State.Show, _currentState);
			}
			else
			{
				//슬라이드 아웃
				ChangeState(State.Focus, _currentState);
			}
		}
		else
		{
			//즉시 초기화
			SetState(_currentState);
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		if (_panelToSlide && _slideInSetting.IsLocked)
		{
			//변경된 경계에 맞는 상태를 즉시 반영
			UpdatePositions();
			SetState(_currentState);
		}
	}

	protected void OnDrawGizmosSelected()
	{
		if (_panelToSlide)
		{
			UpdatePositions();
			var tr = transform.parent ? transform.parent : transform;
			Vector2 focusPos = tr.TransformPoint(_slideInSetting.SlidePosition);
			Color[] gizmoColors = new Color[] { Color.black, Color.red, Color.magenta, Color.blue, Color.cyan, };
			for (int i = 0; i < _stateSettings.Length; i++)
			{
				//로컬 좌표를 월드 좌표로 변환
				Vector2 globalPos = tr.TransformPoint(_stateSettings[i].SlidePosition);
				Gizmos.color = gizmoColors[i];
				Gizmos.DrawWireSphere(globalPos, 50f);
				Gizmos.DrawLine(focusPos, globalPos);
			}
		}
	}

	/// <summary>
	/// 경계 초기화
	/// </summary>
	void InitializeBoundary()
	{
		if (!_panelToSlide)
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

		//임의의 경계가 없으면 루트 캔버스(화면)를 경계로 사용
		Canvas canvas = _panelToSlide.GetComponentInParent<Canvas>();
		if (canvas)
		{
			_boundary = canvas.rootCanvas.GetComponent<RectTransform>();
		}
	}

	void UpdatePositions()
	{
		Vector2 position = _slideInSetting.SlidePosition;
		Vector2 boundarySize = _boundary ? _boundary.rect.size : Vector2.one;
		for (int i = 1; i < _stateSettings.Length; i++)
		{
			_stateSettings[i].UpdateSlidePosition(position, boundarySize);
		}
	}

	/// <summary>
	/// 패널 슬라이드 인
	/// </summary>
	public void Open(State from = State.Show)
	{
		if (_isOpened)
		{
			//이미 열려있음
			return;
		}
		ChangeState(from, State.Focus);
	}

	/// <summary>
	/// 패널 슬라이드 아웃
	/// </summary>
	public void Close(State to = State.Hide)
	{
		if (!_isOpened)
		{
			//이미 닫혀있음
			return;
		}
		ChangeState(State.Focus, to);
	}

	/// <summary>
	/// 해당 상태로 설정
	/// </summary>
	public void SetState(State state)
	{
		_currentState = state;
		var setting = _stateSettings[(int)state];
		//위치
		if (_panelToSlide) _panelToSlide.anchoredPosition = setting.SlidePosition;
		//크기
		if (_panelToScale) _panelToScale.localScale = Vector3.one * setting.Scale;
		//불투명도
		if (_transparentGroup) _transparentGroup.alpha = setting.Alpha;
		//캔버스 (비)활성화
		if (_canvasToDisable) _canvasToDisable.enabled = setting.Alpha != 0f;
	}
	/// <summary>
	/// 이전 상태에서 다음 상태로 부드럽게 전환
	/// </summary>
	void ChangeState(State from, State to)
	{
		_currentState = to;
		var fromSetting = _stateSettings[(int)from];
		var toSetting = _stateSettings[(int)to];

		if (_panelToSlide)
		{
			//위치
			_panelToSlide
				.DOAnchorPos(toSetting.SlidePosition, toSetting.Duration)
				.From(fromSetting.SlidePosition)
				.SetEase(toSetting.CustomEase != Ease.Unset ? toSetting.CustomEase : _defaultEase)
				.SetDelay(toSetting.Delay);
		}
		if (_panelToScale)
		{
			//크기
			_panelToScale
				.DOScale(toSetting.Scale, toSetting.Duration)
				.From(fromSetting.Scale)
				.SetEase(toSetting.CustomEase != Ease.Unset ? toSetting.CustomEase : _defaultEase)
				.SetDelay(toSetting.Delay);
		}
		if (_transparentGroup)
		{
			//불투명도
			_transparentGroup
				.DOFade(toSetting.Alpha, toSetting.Duration)
				.From(fromSetting.Alpha)
				.SetEase(toSetting.CustomEase != Ease.Unset ? toSetting.CustomEase : _defaultEase)
				.SetDelay(toSetting.Delay);
		}
		if (_canvasToDisable)
		{
			//비활성화
			if (fromSetting.Alpha == 0f && toSetting.Alpha == 0f)
			{
				_canvasToDisable.enabled = false;
			}
			//활성화
			else
			{
				_canvasToDisable.enabled = true;
				if (toSetting.Alpha == 0f)
				{
					//재생 후 비활성화
					DOVirtual
						.DelayedCall(toSetting.Delay + toSetting.Duration,
						() => _canvasToDisable.enabled = false);
				}
			}
		}
	}
}
