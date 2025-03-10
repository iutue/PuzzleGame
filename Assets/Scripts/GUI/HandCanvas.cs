using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static BlockGroupView;

/// <summary>
/// 자신이 가지고 있는 카드를 보여주는 창
/// </summary>
[RequireComponent(typeof(Canvas))]
public class HandCanvas : UIBehaviour
{
	//TODO GameModeSetting을 만들어 DrawCount, TimeLimit 등 저장하기
	/// <summary>
	/// 카드를 집었을 때 포인터와 카드의 상대 위치, 화면 크기에 비례함
	/// </summary>
	[SerializeField]
	Vector2 _cardOffset;
	/// <summary>
	/// 카드에 무작위로 적용할 테마
	/// </summary>
	[SerializeField]
	BlockTheme[] _themes;

	[SerializeField]
	SlidePanel _cards;
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

	//TODO drawCount는 ModeSetting에서 가져오기
	public void Init(int drawCount, DragHandler beginDragCard, DragHandler endDragCard, DragHandler dragCard)
	{
		_beginDragCard = beginDragCard;
		_endDragCard = endDragCard;
		_dragCard = dragCard;
		//카드가 들어갈 자리를 미리 확보
		for (int i = 0; i < drawCount; i++)
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
		//새로운 뷰 생성
		for (int i = 0; i < cards.Count; i++)
		{
			var cardView = Instantiate(_cardViewPrefab, _cardParent.GetChild(i)).GetComponent<BlockGroupView>();
			var randomIndex = UnityEngine.Random.Range(0, _themes.Length);
			cardView.Init(cards[i], _themes[randomIndex]);
			cardView.BeginDrag += _beginDragCard;
			cardView.EndDrag += _endDragCard;
			cardView.Dragging += _dragCard;
			_cardViews.Add(cardView);
		}
	}

	public void Open()
	{
		_cards.Open();
	}
	public void Close()
	{
		_cards.Close();
	}

	#region Callbacks
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
	public void OnBeginDragCard(BlockGroupView cardView, Vector2 blockViewSize)
	{
		//카드 블록의 크기를 맵 블록의 크기와 일치시킴
		cardView.StartDragging(blockViewSize);
	}

	/// <summary>
	/// 카드를 이동중일 때 호출됨
	/// </summary>
	public void OnDragCard(BlockGroupView cardView, Vector2 position)
	{
		cardView.Drag(position + _cardOffset * Screen.height);
	}

	/// <summary>
	/// 카드를 내려놓았을 때 호출됨
	/// </summary>
	public void OnEndDragCard(BlockGroupView cardView)
	{
		cardView.StopDragging();
	}
	#endregion
}
