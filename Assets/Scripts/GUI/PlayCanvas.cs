using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static BlockGroupView;

/// <summary>
/// 매치에 필수적인 GUI 모음
/// </summary>
[RequireComponent(typeof(Canvas))]
public class PlayCanvas : UIBehaviour
{
	/// <summary>
	/// 탑바 패널
	/// </summary>
	[SerializeField]
	TopBarCanvas _topBarCanvas;
	/// <summary>
	/// 맵 패널
	/// </summary>
	[field: SerializeField]
	public MapCanvas MapCanvas { get; private set; }

	/// <summary>
	/// 카드 패널
	/// </summary>
	[Space]
	[SerializeField]
	//TODO CardsCanvas로 관리하기
	SlidePanel _cardsCanvas;
	[SerializeField]
	GameObject _blockGroupViewPrefab;
	/// <summary>
	/// 모든 카드 뷰를 가지는 부모
	/// </summary>
	[SerializeField]
	RectTransform _cardParent;
	List<BlockGroupView> _cardViews = new();
	/// <summary>
	/// 카드를 집었을 때 포인터와 카드의 상대 위치, 화면 크기에 비례함
	/// </summary>
	[SerializeField]
	Vector2 _cardOffset;
	/// <summary>
	/// 카드에 무작위로 적용할 테마
	/// </summary>
	[SerializeField]
	BlockTheme[] _cardThemes;

	//TODO PlayCanvas 내 자식 캔버스로 옮기기
	/// <summary>
	/// 결과 패널
	/// </summary>
	[Space]
	[SerializeField]
	ResultCanvas _resultCanvas;

	#region Game
	/// <summary>
	/// 게임 초기화 후 호출됨
	/// </summary>
	public void OnGameInitialized(int drawCount, ScoreContainer scoreTable,
		Action backButtonClicked, Action retryButtonClicked, Action nextButtonClicked)
	{
		//탑바
		_topBarCanvas.Init(scoreTable["Total"], backButtonClicked, retryButtonClicked);
		//맵
		MapCanvas.Init();
		//카드
		//_cards.Init();
		for (int i = 0; i < drawCount; i++)
		{
			//카드가 들어갈 자리를 미리 확보
			var child = new GameObject("CardSlot").AddComponent<RectTransform>();
			child.transform.SetParent(_cardParent, false);
		}
		//결과
		_resultCanvas.Init(scoreTable, backButtonClicked, retryButtonClicked, nextButtonClicked);
	}

	/// <summary>
	/// 게임 시작 후 호출됨
	/// </summary>
	public void OnGameStarted()
	{
		//결과 패널은 닫고
		_resultCanvas.Close();
		//필수 패널은 열기
		_topBarCanvas.Open();
		MapCanvas.Open();
		_cardsCanvas.Open();
	}

	/// <summary>
	/// 게임 종료 후 호출됨
	/// </summary>
	public void OnGameEnded()
	{
		_topBarCanvas.Close();
		_resultCanvas.Open();
	}

	/// <summary>
	/// 게임 리셋 후 호출됨
	/// </summary>
	public void OnGameReset(BlockGroup map)
	{
		MapCanvas.ResetMap(map);
		//_cards.ResetCards();
	}
	#endregion

	#region Card
	/// <summary>
	/// 모든 카드 뷰 초기화
	/// </summary>
	public void ResetCards(List<BlockGroup> cards, DragHandler beginDragCard, DragHandler endDragCard, DragHandler dragCard)
	{
		//기존의 뷰 제거
		foreach (var cardView in _cardViews)
		{
			Destroy(cardView.gameObject);
		}
		_cardViews.Clear();
		//새로운 뷰 생성
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
		_cardsCanvas.SetState(SlidePanel.State.Show);
		_cardsCanvas.Open();
	}

	/// <summary>
	/// 주어진 카드 뷰 제거
	/// </summary>
	public void OnCardRemoved(BlockGroupView cardView)
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
		cardView.StartDragging(MapCanvas.CellSize);
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
