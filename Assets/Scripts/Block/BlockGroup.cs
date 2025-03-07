using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 복수의 블록을 2차원으로 보관하는 컨테이너
/// </summary>
public class BlockGroup : IEnumerable<Block>
{
	/// <summary>
	/// 모든 블록
	/// </summary>
	Block[,] _blocks;
	public Block this[int x, int y] => _blocks[x, y];
	/// <summary>
	/// 블록 그룹의 크기, 블록을 배치할 수 있는 가장 먼 위치
	/// </summary>
	public Vector2Int Size { get; private set; }
	public event Action<Block, Block.State, Block.State> TypeChanged;
	public event Action<Block> BlockSpawned;
	public event Action<Block> BlockDestroyed;

	public BlockGroup(Block.State[,] blockTypes)
	{
		Size = new Vector2Int(blockTypes.GetLength(0), blockTypes.GetLength(1));
		//모든 블록 초기화
		_blocks = new Block[Size.x, Size.y];
		for (int x = 0; x < Size.x; x++)
		{
			for (int y = 0; y < Size.y; y++)
			{
				var newBlock = new Block(blockTypes[x, y], new Vector2Int(x, y));
				newBlock.TypeChanged += OnBlockTypeChanged;
				newBlock.BlockSpawned += OnBlockSpawned;
				newBlock.BlockDestroyed += OnBlockDestroyed;
				_blocks[x, y] = newBlock;
			}
		}
	}

	/// <summary>
	/// 해당 상태의 블록이 있는가
	/// </summary>
	public bool Contains(Block.State type)
	{
		for (int x = 0; x < Size.x; x++)
		{
			for (int y = 0; y < Size.y; y++)
			{
				if (_blocks[x, y].Type == type)
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 특정 상태의 모든 블록을 다른 상태로 변환
	/// </summary>
	/// <param name="from">변환할 블록의 상태</param>
	/// <param name="to">변환 후 블록의 상태</param>
	/// <returns>상태가 변환된 블록의 개수</returns>
	public int Convert(Block.State from, Block.State to)
	{
		int count = 0;
		for (int x = 0; x < Size.x; x++)
		{
			for (int y = 0; y < Size.y; y++)
			{
				Block block = _blocks[x, y];
				if (block.Type == from)
				{
					block.Type = to;
					count++;
				}
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
				if (other[otherX, otherY].Type == Block.State.Empty)
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
				if (_blocks[position.x, position.y].Type == Block.State.Placed &&
					other[otherX, otherY].Type != Block.State.Empty)
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

	#region Callbacks
	void OnBlockTypeChanged(Block block, Block.State oldType, Block.State newType)
	{
		TypeChanged?.Invoke(block, oldType, newType);
	}

	void OnBlockSpawned(Block block)
	{
		BlockSpawned?.Invoke(block);
	}

	void OnBlockDestroyed(Block block)
	{
		BlockDestroyed?.Invoke(block);
	}
	#endregion

	public IEnumerator<Block> GetEnumerator()
	{
		for (int x = 0; x < Size.x; x++)
		{
			for (int y = 0; y < Size.y; y++)
			{
				yield return _blocks[x, y];
			}
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
