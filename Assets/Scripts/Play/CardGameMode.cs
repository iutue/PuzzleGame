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

	#region Game
	protected override void InitGame()
	{
		base.InitGame();
		_handView.Init(_cardThemes, _handCapacity, OnBeginDragCard, OnEndDragCard, OnDragCard);
	}

	protected override void ResetGame()
	{
		base.ResetGame();
		ResetCards();
	}

	protected override void StartGame()
	{
		base.StartGame();
		DrawCards();
		_handView.Open();
	}
	#endregion

	#region Map
	protected override void UpdateMap()
	{
		TryLineClear();
	}

	/// <summary>
	/// 맵에 존재하는 모든 블록 라인을 찾아서 제거
	/// </summary>
	void TryLineClear()
	{
		//라인 클리어 횟수
		int clearCount = 0;
		//제거할 블록들
		List<Block> blocksToDestroy = new List<Block>(Map.Size.x * Map.Size.y);
		//제거할 블록 후보들
		List<Block> candidates = new List<Block>(Map.Size.x + Map.Size.y);

		//세로 클리어
		for (int x = 0; x < Map.Size.x; x++)
		{
			candidates.Clear();
			for (int y = 0; y < Map.Size.y; y++)
			{
				if (Map[x, y].CurrentState == Block.State.Placed)
				{
					candidates.Add(Map[x, y]);
				}
				else
				{
					candidates.Clear();
					break;
				}
			}
			if (candidates.Count > 0)
			{
				clearCount++;
				blocksToDestroy.AddRange(candidates);
			}
		}
		//가로 클리어
		for (int y = 0; y < Map.Size.y; y++)
		{
			candidates.Clear();
			for (int x = 0; x < Map.Size.x; x++)
			{
				if (Map[x, y].CurrentState == Block.State.Placed)
				{
					candidates.Add(Map[x, y]);
				}
				else
				{
					candidates.Clear();
					break;
				}
			}
			if (candidates.Count > 0)
			{
				clearCount++;
				blocksToDestroy.AddRange(candidates);
			}
		}
		//감지된 모든 라인의 블록 제거
		foreach (var block in blocksToDestroy)
		{
			block.CurrentState = Block.State.Empty;
		}
		//점수 추가
		Scores["LineClear"].BaseValue += clearCount;
	}

	protected override bool CheckEndCondition()
	{
		//배치할 수 있는 카드를 하나라도 가지고 있는지 검사
		bool hasValidCard = false;
		foreach (var card in Hand)
		{
			if (TryPlaceCard(card))
			{
				//배치 가능한 카드가 하나라도 있음
				hasValidCard = true;
				break;
			}
		}
		//검사 흔적 제거
		Map.Convert(Block.State.Preview, Block.State.Empty);

		//배치 가능한 카드가 하나도 없으면 게임 종료
		return !hasValidCard;
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
		_handView.ResetCards(_hand);
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
		_handView.OnCardAdded(Hand.Count - 1, card);
	}

	/// <summary>
	/// 패의 카드 제거
	/// </summary>
	protected virtual void RemoveCard(BlockGroupView target)
	{
		_hand.Remove(target.OwnerBlockGroup);
		_handView.OnCardRemoved(target);

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
		Map.Convert(Block.State.Preview, Block.State.Empty);
		if (currentBlock == null)
		{
			//감지된 블록이 없으면 이전의 배치 흔적만 제거
			return true;
		}
		else
		{
			//새로운 블록에 카드 배치 시도
			return Map.TryMerge(_selectedCard, currentBlock.Position);
		}
	}

	/// <summary>
	/// 카드 배치 확정
	/// </summary>
	void ConfirmCardPlacement(BlockGroupView cardView)
	{
		//카드 사용
		Map.Convert(Block.State.Preview, Block.State.Placed);
		RemoveCard(cardView);
		//게임 업데이트
		UpdateGame();
	}

	/// <summary>
	/// 맵에 카드를 배치할 수 있는 공간이 있는가
	/// </summary>
	bool TryPlaceCard(BlockGroup card)
	{
		foreach (var mapBlock in Map)
		{
			if (Map.TryMerge(card, mapBlock.Position))
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
	protected virtual void OnBeginDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		_selectedCard = cardView.OwnerBlockGroup;
		_handView.OnBeginDragCard(cardView, MapView.BlockViewSize);
	}

	/// <summary>
	/// 카드를 이동중일 때 호출됨
	/// </summary>
	protected virtual void OnDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		Origin = MapView.GetMapBlockAt(cardView.OriginBlockPosition);
		//카드 배치 시도
		if (TryCardPlacement())
		{
			//배치 성공
		}
		else
		{
			//배치 실패
			Map.Convert(Block.State.Preview, Block.State.Empty);
		}
		_handView.OnDragCard(cardView, eventData.position);
	}

	/// <summary>
	/// 카드를 내려놓았을 때 호출됨
	/// </summary>
	protected virtual void OnEndDragCard(BlockGroupView cardView, PointerEventData eventData)
	{
		//맵에 배치된 카드가 있으면
		//TODO[개선] 맵이 아닌 카드로 배치 성공 여부 저장하기
		if (Map.Contains(Block.State.Preview))
		{
			//카드 배치 확정
			ConfirmCardPlacement(cardView);
		}
		_handView.OnEndDragCard(cardView);
	}

	/// <summary>
	/// 패에 카드가 하나도 없을 때 호출됨
	/// </summary>
	protected virtual void OnHandEmpty()
	{
		//새로운 카드 뽑기
		DrawCards();
	}
	#endregion
}
