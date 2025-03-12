using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static BlockGroupView;

/// <summary>
/// 자신이 가지고 있는 카드를 보여주는 창
/// </summary>
[RequireComponent(typeof(Canvas))]
public class HandView : UIBehaviour
{
	[SerializeField]
	PlaySetting _playSetting;
	GameState _gameModeSetting;

	[SerializeField]
	SlidePanel _panel;
	[SerializeField]
	RectTransform _cardParent;

	/// <summary>
	/// 생성할 카드 뷰
	/// </summary>
	[SerializeField]
	GameObject _cardViewPrefab;
	/// <summary>
	/// 생성된 모든 카드 뷰
	/// </summary>
	List<BlockGroupView> _cardViews = new();

	DragHandler _beginDragCard;
	DragHandler _endDragCard;
	DragHandler _dragCard;

	public void Init(GameState gameModeSetting, DragHandler beginDragCard, DragHandler endDragCard, DragHandler dragCard)
	{
		_gameModeSetting = gameModeSetting;
		_beginDragCard = beginDragCard;
		_endDragCard = endDragCard;
		_dragCard = dragCard;
		//카드가 들어갈 자리를 미리 확보
		for (int i = 0; i < _gameModeSetting.HandCapacity; i++)
		{
			var child = new GameObject("CardSlot").AddComponent<RectTransform>();
			child.transform.SetParent(_cardParent, false);
		}
	}

	/// <summary>
	/// 모든 카드 뷰 초기화
	/// </summary>
	public void ResetCards(List<BlockGroup> cards)
	{
		//기존의 뷰 제거
		foreach (var cardView in _cardViews)
		{
			Destroy(cardView.gameObject);
		}
		_cardViews.Clear();
	}

	public void Open()
	{
		_panel.Open();
	}
	public void Close()
	{
		_panel.Close();
	}

	#region Callbacks
	/// <summary>
	/// 카드가 추가됐을 때 호출됨
	/// </summary>
	public void OnCardAdded(int index, BlockGroup card)
	{
		//카드 뷰 추가
		var cardView = Instantiate(_cardViewPrefab, _cardParent.GetChild(index)).GetComponent<BlockGroupView>();
		var randomIndex = UnityEngine.Random.Range(0, _gameModeSetting.CardThemes.Length);
		cardView.Init(card, _gameModeSetting.CardThemes[randomIndex]);
		cardView.BeginDrag += _beginDragCard;
		cardView.EndDrag += _endDragCard;
		cardView.Dragging += _dragCard;
		_cardViews.Add(cardView);
	}

	/// <summary>
	/// 카드가 제거됐을 때 호출됨
	/// </summary>
	public void OnCardRemoved(BlockGroupView cardView)
	{
		//카드 뷰 제거
		_cardViews.Remove(cardView);
		Destroy(cardView.gameObject);
	}

	/// <summary>
	/// 플레이어가 카드를 집었을 때 호출됨
	/// </summary>
	public void OnBeginDragCard(BlockGroupView cardView, Vector2 mapBlockViewSize)
	{
		//카드 블록의 크기를 맵 블록의 크기와 일치시킴
		cardView.ChangeBlockViewSize(mapBlockViewSize);
	}

	/// <summary>
	/// 플레이어가 카드를 이동중일 때 호출됨
	/// </summary>
	public void OnDragCard(BlockGroupView cardView, Vector2 position)
	{
		//카드를 포인터 위치로 이동
		position = cardView.GetComponent<RectTransform>().InverseTransformPoint(position);
		cardView.SetPosition(position + _playSetting.CardOffset * Screen.height);
	}

	/// <summary>
	/// 플레이어가 카드를 내려놓았을 때 호출됨
	/// </summary>
	public void OnEndDragCard(BlockGroupView cardView)
	{
		//카드를 원래 위치와 크기로 복구
		cardView.ChangePosition(Vector2.zero);
		cardView.ChangeBlockViewSize(cardView.BlockViewSize);
	}
	#endregion
}
