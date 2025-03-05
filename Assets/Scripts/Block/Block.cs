using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 하나의 칸
/// </summary>
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
			if (_type != value)
			{
				//블록의 타입이 바뀜
				TypeChanged?.Invoke(_type, value);
			}
			_type = value;
		}
	}
	/// <summary>
	/// 블록의 종류가 변경됐을 때
	/// </summary>
	public event Action<BlockType, BlockType> TypeChanged;

	public Block(BlockType type, Vector2Int position)
	{
		_type = type;
		Position = position;
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
