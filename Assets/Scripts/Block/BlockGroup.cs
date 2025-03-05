using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 복수의 블록을 가지는 블록 컨테이너
/// </summary>
public class BlockGroup : IEnumerable<Block>
{
	Block[,] _blocks;
	public Block this[int x, int y] => _blocks[x, y];
	public Vector2Int Size { get; private set; }

	/// <summary>
	/// 블록이 배치됐을 때
	/// </summary>
	public event Action BlockSpawned;
	/// <summary>
	/// 블록이 제거됐을 때
	/// </summary>
	public event Action BlockDestroyed;

	public BlockGroup(BlockType[,] blockTypes)
	{
		Size = new Vector2Int(blockTypes.GetLength(0), blockTypes.GetLength(1));
		//블록 초기화
		_blocks = new Block[Size.x, Size.y];
		for (int x = 0; x < Size.x; x++)
		{
			for (int y = 0; y < Size.y; y++)
			{
				var newBlock = new Block(blockTypes[x, y], new Vector2Int(x, y));
				newBlock.TypeChanged += OnBlockTypeChanged;
				_blocks[x, y] = newBlock;
			}
		}
	}

	/// <summary>
	/// 해당 종류의 블록이 있는가
	/// </summary>
	public bool Contains(BlockType type)
	{
		foreach (var block in _blocks)
		{
			if (block.Type == type)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 특정 종류의 블록을 다른 종류로 변환
	/// </summary>
	/// <param name="from">변환할 블록의 종류</param>
	/// <param name="to">변환 후의 종류</param>
	public int Convert(BlockType from, BlockType to)
	{
		int count = 0;
		foreach (var block in _blocks)
		{
			if (block.Type == from)
			{
				block.Type = to;
				count++;
			}
		}
		return count;
	}

	/// <summary>
	/// 다른 블록 그룹과 병합 시도
	/// </summary>
	public bool TryMerge(BlockGroup other, Vector2Int offset)
	{
		for (int otherX = 0; otherX < other.Size.x; otherX++)
		{
			for (int otherY = 0; otherY < other.Size.y; otherY++)
			{
				if (other[otherX, otherY].Type == BlockType.Empty)
				{
					//빈 블록은 무시
					continue;
				}
				Vector2Int position = new Vector2Int(offset.x + otherX, offset.y + otherY);
				if (Size.x <= position.x ||
					Size.y <= position.y)
				{
					//넣을 블록이 범위를 벗어남
					return false;
				}
				if (_blocks[position.x, position.y].Type == BlockType.Block &&
					other[otherX, otherY].Type != BlockType.Empty)
				{
					//이미 블록이 존재함
					return false;
				}
				//블록 병합
				_blocks[position.x, position.y].Type = other[otherX, otherY].Type;
			}
		}
		//모든 블록 병합 완료
		return true;
	}

	//public List<Block> GetCluster(Block centroid)
	//{
	//	var cluster = new List<Block>();
	//	Find(centroid, centroid.Type, cluster);
	//	return cluster;
	//}
	//void Find(Block target, BlockType type, List<Block> foundBlocks)
	//{
	//	if (foundBlocks.Contains(target))
	//	{
	//		return;
	//	}

	//	if (target.Type == type)
	//	{
	//		foundBlocks.Add(target);
	//	}

	//	for (int x = 0; x < target.Position.x - 1; x++)
	//	{
	//		for (int y = 0; x < target.Position.y - 1; y++)
	//		{
	//			if (x < 0 || Size.x <= x ||
	//				y < 0 || Size.y <= y)
	//			{
	//				continue;
	//			}
	//			if (x == target.Position.x && y == target.Position.y)
	//			{
	//				continue;
	//			}
	//			Find(_blocks[x, y], type, foundBlocks);
	//		}
	//	}
	//}

	#region Callbacks
	/// <summary>
	/// 블록의 종류가 변경됐을 때 호출됨
	/// </summary>
	void OnBlockTypeChanged(BlockType previousType, BlockType currentType)
	{
		if (previousType != BlockType.Empty && currentType == BlockType.Block)
		{
			//블록이 배치됨
			BlockSpawned?.Invoke();
		}
		else if (previousType == BlockType.Block && currentType == BlockType.Empty)
		{
			//블록이 제거됨
			BlockDestroyed?.Invoke();
		}
	}
	#endregion
	
	public IEnumerator<Block> GetEnumerator()
	{
		for (int x = 0; x < _blocks.GetLength(0); x++)
		{
			for (int y = 0; y < _blocks.GetLength(1); y++)
			{
				yield return _blocks[x, y];
			}
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
