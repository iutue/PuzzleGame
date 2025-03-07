using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 최대한 많은 블록을 배치, 파괴하는 게임모드
/// </summary>
public class BlastGameMode : GameMode
{
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
				if (Map[x, y].Type == Block.State.Placed)
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
				if (Map[x, y].Type == Block.State.Placed)
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
			block.Type = Block.State.Empty;
		}
		//점수 추가
		Scores["LineClear"].BaseValue += clearCount;
	}

	protected override bool CheckEndCondition()
	{
		//배치할 수 있는 카드를 하나라도 가지고 있는지 검사
		bool hasValidCard = false;
		foreach (var card in Cards)
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
	#endregion
}
