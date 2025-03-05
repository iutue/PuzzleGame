using System;
using System.Collections.Generic;
using System.Linq;

public class BlastGameMode : GameMode
{
	protected override void UpdateMap()
	{
		TryLineClear();
	}

	/// <summary>
	/// 맵의 모든 라인을 찾아서 클리어
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
				if (Map[x, y].Type == BlockType.Block)
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
				if (Map[x, y].Type == BlockType.Block)
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

		//감지된 모든 라인 클리어
		foreach (var block in blocksToDestroy)
		{
			block.Type = BlockType.Empty;
		}

		ScoreTable["LineClear"].BaseValue += clearCount;
	}

	protected override bool CheckEndCondition()
	{
		//배치할 수 있는 카드를 하나라도 가지고 있는지 검사
		bool canPlaceCard = false;
		foreach (var card in Cards)
		{
			if (TryPlaceCard(card))
			{
				//배치 가능한 카드가 하나라도 있음
				canPlaceCard = true;
				break;
			}
		}
		Map.Convert(BlockType.Ghost, BlockType.Empty);

		return !canPlaceCard;
	}
	/// <summary>
	/// 카드를 배치할 수 있는 공간이 있는가
	/// </summary>
	bool TryPlaceCard(BlockGroup card)
	{
		foreach (var mapBlock in Map)
		{
			if (Map.TryMerge(card, mapBlock.Position))
			{
				//카드를 맵에 배치 가능
				return true;
			}
		}
		//카드를 맵에 배치할 수 없음
		return false;
	}

	protected override void OnMapBlockSpawned()
	{
		base.OnMapBlockSpawned();
		ScoreTable["+Block"].BaseValue += 1;
	}
	protected override void OnMapBlockDestroyed()
	{
		base.OnMapBlockDestroyed();
		ScoreTable["-Block"].BaseValue += 1;
	}
}
