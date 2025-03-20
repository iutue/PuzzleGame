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
	SlidePanel _panel;
	[SerializeField]
	RectTransform _cardParent;

	/// <summary>
	/// 생성할 카드 뷰
	/// </summary>
	[SerializeField]
	GameObject _cardViewPrefab;
	BlockTheme[] _cardThemes;
	/// <summary>
	/// 생성된 모든 카드 뷰
	/// </summary>
	List<BlockGroupView> _cardViews = new();

	public event DragHandler CardDraggingStarted;
	public event DragHandler CardDragging;
	public event DragHandler CardDraggingStopped;

	public void Init(BlockTheme[] cardThemes, int handCapacity)
	{
		_cardThemes = cardThemes;
		//카드가 들어갈 자리를 미리 확보
		for (int i = 0; i < handCapacity; i++)
		{
			var child = new GameObject("CardSlot").AddComponent<RectTransform>();
			child.transform.SetParent(_cardParent, false);
		}
	}

	/// <summary>
	/// 카드 뷰 추가
	/// </summary>
	public void AddCard(int index, BlockGroup card)
	{
		var cardView = Instantiate(_cardViewPrefab, _cardParent.GetChild(index)).GetComponent<BlockGroupView>();
		var randomIndex = UnityEngine.Random.Range(0, _cardThemes.Length);
		cardView.Init(card, _cardThemes[randomIndex]);
		cardView.DraggingStarted += OnCardDraggingStarted;
		cardView.DraggingStopped += OnCardDraggingStopped;
		cardView.Dragging += OnCardDragging;
		_cardViews.Add(cardView);
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
	/// 모든 카드 뷰 제거
	/// </summary>
	public void ClearCards()
	{
		//기존의 뷰 제거
		foreach (var cardView in _cardViews)
		{
			Destroy(cardView.gameObject);
		}
		_cardViews.Clear();
	}

	/// <summary>
	/// 카드 들기
	/// </summary>
	public void GrabCard(BlockGroupView cardView, Vector2 mapBlockViewSize)
	{
		//카드 블록의 크기를 맵 블록의 크기와 일치시킴
		cardView.ChangeBlockViewSize(mapBlockViewSize);
	}

	/// <summary>
	/// 카드 이동
	/// </summary>
	public void MoveCard(BlockGroupView cardView, Vector2 position)
	{
		//카드를 포인터 위치로 이동
		position = cardView.GetComponent<RectTransform>().InverseTransformPoint(position);
		cardView.SetPosition(position + SettingManager.Instance.Play.CardOffset * Screen.height);
	}

	/// <summary>
	/// 카드 놓기
	/// </summary>
	public void DropCard(BlockGroupView cardView)
	{
		//카드를 원래 위치와 크기로 복구
		cardView.ChangePosition(Vector2.zero);
		cardView.ChangeBlockViewSize(cardView.BlockViewSize);
	}

	public void Open() => _panel.Open();
	public void Close() => _panel.Close();

	#region Callbacks
	void OnCardDraggingStarted(BlockGroupView t, PointerEventData e) => CardDraggingStarted?.Invoke(t, e);
	void OnCardDragging(BlockGroupView t, PointerEventData e) => CardDragging?.Invoke(t, e);
	void OnCardDraggingStopped(BlockGroupView t, PointerEventData e) => CardDraggingStopped?.Invoke(t, e);
	#endregion
}
