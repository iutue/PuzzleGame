using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 카드를 사용하는 게임 모드
/// </summary>
public abstract class CardGameMode : GameMode
{
	/// <summary>
	/// 내 턴에 뽑을 카드 수
	/// </summary>
	[Header("Card"), Range(1, 5), SerializeField]
	int _drawCount;
	/// <summary>
	/// 패의 최대 카드 수
	/// </summary>
	[Range(1, 10), SerializeField]
	int _handCapacity;
	/// <summary>
	/// 패의 모든 카드
	/// </summary>
	protected List<BlockGroup> _hand = new();
	protected IReadOnlyList<BlockGroup> Hand => _hand;

	/// <summary>
	/// 선택된 카드
	/// </summary>
	BlockGroup _selectedCard;
	[SerializeField]
	HandView _handView;
	/// <summary>
	/// 카드에 적용할 테마
	/// </summary>
	[SerializeField]
	BlockTheme[] _cardThemes;

	protected override void OnEnable()
	{
		base.OnEnable();
		_handView.CardDraggingStarted += OnCardDraggingStarted;
		_handView.CardDragging += OnCardDragging;
		_handView.CardDraggingStopped += OnCardDraggingStopped;
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		_handView.CardDraggingStarted -= OnCardDraggingStarted;
		_handView.CardDragging -= OnCardDragging;
		_handView.CardDraggingStopped -= OnCardDraggingStopped;
	}

	#region Game
	public override void InitGame(GameMap map)
	{
		base.InitGame(map);
		_handView.Init(_cardThemes, _handCapacity);
	}

	protected override void ResetGame()
	{
		base.ResetGame();
		ResetCards();
	}

	public override void StartGame()
	{
		base.StartGame();
		DrawCards();
		_handView.Open();
	}
	#endregion

	#region Card
	/// <summary>
	/// 패 버리기
	/// </summary>
	protected virtual void ResetCards()
	{
		//카드 초기화
		_hand.Clear();
		_handView.ClearCards();
	}

	/// <summary>
	/// 카드 뽑기
	/// </summary>
	void DrawCards()
	{
		for (int i = 0; i < _drawCount; i++)
		{
			if (Hand.Count >= _handCapacity)
			{
				//더 이상 카드를 뽑을 수 없음
				break;
			}
			var newCard = CreateCard();
			AddCard(newCard);
		}
	}

	/// <summary>
	/// 새로운 카드 생성
	/// </summary>
	protected virtual BlockGroup CreateCard()
	{
		//램덤한 모양의 카드 생성
		int randomIndex = UnityEngine.Random.Range(0, BlockGroupTemplates.Templates.Count);
		var randomTemplate = BlockGroupTemplates.Templates[randomIndex];
		return new BlockGroup(randomTemplate);
	}

	/// <summary>
	/// 패에 카드 추가
	/// </summary>
	protected virtual void AddCard(BlockGroup card)
	{
		_hand.Add(card);
		_handView.AddCard(Hand.Count - 1, card);
	}

	/// <summary>
	/// 패의 카드 제거
	/// </summary>
	protected virtual void RemoveCard(BlockGroupView target)
	{
		_hand.Remove(target.OwnerBlockGroup);
		_handView.RemoveCard(target);

		if (Hand.Count == 0)
		{
			//패에 카드가 없음
			OnHandEmpty();
		}
	}

	/// <summary>
	/// 맵에 카드 배치 시도
	/// </summary>
	bool TryCardPlacement()
	{
		//감지된 블록
		Block currentBlock;
		if (Origin)
		{
			currentBlock = Origin.OwnerBlock;
		}
		else
		{
			currentBlock = null;
		}
		//이미 배치를 시도한 대상인가
		if (currentBlock == SelectedBlock)
		{
			return true;
		}
		SelectedBlock = currentBlock;

		//배치 시도
		Map.Blocks.Convert(Block.State.Preview, Block.State.Empty);
		if (currentBlock == null)
		{
			//감지된 블록이 없으면 이전의 배치 흔적만 제거
			return true;
		}
		else
		{
			//새로운 블록에 카드 배치 시도
			return Map.Blocks.TryMerge(_selectedCard, currentBlock.Position);
		}
	}

	/// <summary>
	/// 카드 배치 확정
	/// </summary>
	void ConfirmCardPlacement(BlockGroupView cardView)
	{
		//카드 사용
		Map.Blocks.Convert(Block.State.Preview, Block.State.Placed);
		RemoveCard(cardView);
		//게임 업데이트
		UpdateGame();
	}

	/// <summary>
	/// 맵에 카드를 배치할 수 있는 공간이 있는가
	/// </summary>
	bool TryPlaceCard(BlockGroup card)
	{
		foreach (var mapBlock in Map.Blocks)
		{
			if (Map.Blocks.TryMerge(card, mapBlock.Position))
			{
				//맵에 배치 가능
				return true;
			}
		}
		//맵에 배치할 공간이 없음
		return false;
	}
	#endregion

	#region Callbakcs
	protected override void OnMapBlockSpawned(Block block)
	{
		base.OnMapBlockSpawned(block);
		Scores["+Block"].BaseValue += 1;
	}

	protected override void OnMapBlockDestroyed(Block block)
	{
		base.OnMapBlockDestroyed(block);
		Scores["-Block"].BaseValue += 1;
	}

	/// <summary>
	/// 카드를 집었을 때 호출됨
	/// </summary>
	void OnCardDraggingStarted(BlockGroupView cardView, PointerEventData eventData)
	{
		_selectedCard = cardView.OwnerBlockGroup;
		_handView.GrabCard(cardView, Map.MapView.BlockViewSize);
	}

	/// <summary>
	/// 카드를 이동중일 때 호출됨
	/// </summary>
	void OnCardDragging(BlockGroupView cardView, PointerEventData eventData)
	{
		Origin = Map.MapView.GetMapBlockAt(cardView.OriginBlockPosition);
		//카드 배치 시도
		if (TryCardPlacement())
		{
			//배치 성공
		}
		else
		{
			//배치 실패
			Map.Blocks.Convert(Block.State.Preview, Block.State.Empty);
		}
		_handView.MoveCard(cardView, eventData.position);
	}

	/// <summary>
	/// 카드를 내려놓았을 때 호출됨
	/// </summary>
	void OnCardDraggingStopped(BlockGroupView cardView, PointerEventData eventData)
	{
		//맵에 배치된 카드가 있으면
		//TODO[개선] 맵이 아닌 카드로 배치 성공 여부 저장하기
		if (Map.Blocks.Contains(Block.State.Preview))
		{
			//카드 배치 확정
			ConfirmCardPlacement(cardView);
		}
		_handView.DropCard(cardView);
	}

	/// <summary>
	/// 패에 카드가 하나도 없을 때 호출됨
	/// </summary>
	void OnHandEmpty()
	{
		//새로운 카드 뽑기
		DrawCards();
	}
	#endregion
}
