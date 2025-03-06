using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BlockGroupView;

[RequireComponent(typeof(Canvas))]
public class PlayCanvas : MonoBehaviour
{
	/// <summary>
	/// 탑바 패널
	/// </summary>
	[SerializeField]
	TopBarCanvas _topBar;

	/// <summary>
	/// 맵 패널
	/// </summary>
	[Space]
	[SerializeField]
	SlidePanel _map;
	/// <summary>
	/// 맵 뷰를 가지는 부모
	/// </summary>
	[SerializeField]
	RectTransform _mapParent;
	/// <summary>
	/// 현재 맵 뷰
	/// </summary>
	BlockGroupView _mapView;
	GraphicRaycaster _mapRaycaster;
	List<RaycastResult> _mapRaycastresult = new();
	/// <summary>
	/// 맵에 적용할 테마
	/// </summary>
	[SerializeField]
	BlockThemeSet _mapTheme;

	/// <summary>
	/// 카드 패널
	/// </summary>
	[Space]
	[SerializeField]
	SlidePanel _cards;
	/// <summary>
	/// 모든 카드 뷰를 가지는 부모
	/// </summary>
	[SerializeField]
	RectTransform _cardParent;
	/// <summary>
	/// 현재 카드 뷰
	/// </summary>
	List<BlockGroupView> _cardViews = new();
	/// <summary>
	/// 카드를 집었을 때 포인터와 카드의 상대 위치
	/// </summary>
	[SerializeField]
	Vector2 _cardOffset;
	/// <summary>
	/// 카드에 무작위로 적용할 테마
	/// </summary>
	[SerializeField]
	BlockThemeSet[] _cardThemes;

	/// <summary>
	/// 결과 패널
	/// </summary>
	ResultCanvas _result;

	[Space]
	[SerializeField]
	GameObject _blockGroupViewPrefab;
	[SerializeField]
	GameObject _resultPrefab;

	#region Game
	/// <summary>
	/// 게임 초기화 후 호출됨
	/// </summary>
	public void OngameInitialized(int drawCount, ScoreContainer scoreTable, Action backButtonClicked, Action refreshButtonClicked)
	{
		//탑바
		_topBar.Init(scoreTable["Total"], backButtonClicked, refreshButtonClicked);
		//맵
		_mapRaycaster = _mapParent.GetComponentInParent<GraphicRaycaster>();
		//카드
		for (int i = 0; i < drawCount; i++)
		{
			//카드가 들어갈 자리를 미리 계산
			var child = new GameObject().AddComponent<RectTransform>();
			child.transform.SetParent(_cardParent);
		}
	}

	/// <summary>
	/// 게임 시작 후 호출됨
	/// </summary>
	public void OnGameStarted()
	{
		//결과 패널은 닫고
		if (_result) _result.Close();
		//필수 패널은 열기
		_topBar.Open();
		_map.Open();
		_cards.Open();
	}

	/// <summary>
	/// 게임 종료 후 호출됨
	/// </summary>
	public void OnGameEnded(ScoreContainer scoreTable, Action backButtonClicked, Action retryButtonClicked, Action nextButtonClicked)
	{
		_topBar.Close();
		//결과 표시
		_result = Instantiate(_resultPrefab).GetComponent<ResultCanvas>();
		_result.Init(scoreTable, backButtonClicked, retryButtonClicked, nextButtonClicked);
	}
	#endregion

	#region Map
	/// <summary>
	/// 맵 뷰 재설정
	/// </summary>
	public void ResetMap(BlockGroup map)
	{
		if (_mapView)
		{
			Destroy(_mapView.gameObject);
		}
		_mapView = Instantiate(_blockGroupViewPrefab, _mapParent).GetComponent<BlockGroupView>();
		_mapView.Init(map, _mapTheme);
	}

	/// <summary>
	/// 맵 뷰에서 해당 위치의 블록 뷰 검출
	/// </summary>
	public BlockView GetMapBlockAt(Vector2 position)
	{
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
	#endregion

	#region Card
	/// <summary>
	/// 카드 뷰 재설정
	/// </summary>
	public void ResetCards(List<BlockGroup> cards, DragHandler beginDragCard, DragHandler endDragCard, DragHandler dragCard)
	{
		//기존의 카드 뷰 제거
		foreach (var cardView in _cardViews)
		{
			Destroy(cardView.gameObject);
		}
		_cardViews.Clear();
		//새로운 카드 뷰 생성
		for (int i = 0; i < cards.Count; i++)
		{
			var cardView = Instantiate(_blockGroupViewPrefab, _cardParent.GetChild(i)).GetComponent<BlockGroupView>();
			var randomIndex = UnityEngine.Random.Range(0, _cardThemes.Length);
			cardView.Init(cards[i], _cardThemes[randomIndex]);
			cardView.BeginDrag += beginDragCard + OnBeginDragCard;
			cardView.EndDrag += endDragCard + OnEndDragCard;
			cardView.Dragging += dragCard + OnDragCard;
			_cardViews.Add(cardView);
		}
	}

	/// <summary>
	/// 카드 뷰 제거
	/// </summary>
	public void RemoveCard(BlockGroupView cardView)
	{
		_cardViews.Remove(cardView);
		Destroy(cardView.gameObject);
	}

	/// <summary>
	/// 카드를 집었을 때 호출됨
	/// </summary>
	void OnBeginDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		//카드의 블록 크기를 맵과 일치시킴
		cardView.StartDragging(_mapView.CellSize);
	}

	/// <summary>
	/// 카드를 이동중일 때 호출됨
	/// </summary>
	void OnDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		cardView.Drag(eventData.position + _cardOffset * Screen.height);
	}

	/// <summary>
	/// 카드를 내려놓았을 때 호출됨
	/// </summary>
	void OnEndDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		cardView.StopDragging();
	}
	#endregion
}
