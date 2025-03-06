using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Block
{
	/// <summary>
	/// 그룹 내에서 블록의 위치
	/// </summary>
	public readonly Vector2Int Position;

	/// <summary>
	/// 현재 블록의 종류
	/// </summary>
	BlockType _type;
	public BlockType Type
	{
		get => _type;
		set
		{
			BlockType oldType = _type;
			if (value != oldType)
			{
				_type = value;
				OnTypeChanged(oldType, value);
			}
		}
	}
	/// <summary>
	/// 블록의 종류가 변경됐을 때
	/// </summary>
	public event Action<Block, BlockType, BlockType> TypeChanged;
	/// <summary>
	/// 블록이 배치됐을 때
	/// </summary>
	public event Action<Block> BlockSpawned;
	/// <summary>
	/// 블록이 제거됐을 때
	/// </summary>
	public event Action<Block> BlockDestroyed;

	public Block(BlockType type, Vector2Int position)
	{
		_type = type;
		Position = position;
	}

	/// <summary>
	/// 블록의 종류가 변경됐을 때 호출됨
	/// </summary>
	void OnTypeChanged(BlockType oldType, BlockType newType)
	{
		TypeChanged?.Invoke(this, oldType, newType);
		if (oldType != BlockType.Block && newType == BlockType.Block)
		{
			//블록이 배치됨
			BlockSpawned?.Invoke(this);
		}
		else if (oldType == BlockType.Block && newType != BlockType.Block)
		{
			//블록이 제거됨
			BlockDestroyed?.Invoke(this);
		}
	}
}

/// <summary>
/// 블록의 종류
/// </summary>
public enum BlockType
{
	/// <summary>
	/// 블록을 배치할 수 있는 빈 공간
	/// </summary>
	Empty,
	/// <summary>
	/// 예약된 블록
	/// </summary>
	Ghost,
	/// <summary>
	/// 배치된 블록
	/// </summary>
	Block
}
