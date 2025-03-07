using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 한 블록의 정보를 가지는 하나의 칸
/// </summary>
public class Block
{
	/// <summary>
	/// 블록 그룹 내에서 블록의 위치
	/// </summary>
	public readonly Vector2Int Position;
	
	/// <summary>
	/// 블록의 상태
	/// </summary>
	public enum State
	{
		/// <summary>
		/// 블록 없음
		/// </summary>
		Empty,
		/// <summary>
		/// 블록 배치 예약
		/// </summary>
		Preview,
		/// <summary>
		/// 블록 배치됨
		/// </summary>
		Placed
	}
	/// <summary>
	/// 현재 블록의 상태
	/// </summary>
	State _type;
	public State Type
	{
		get => _type;
		set
		{
			State oldType = _type;
			if (value != oldType)
			{
				_type = value;
				OnTypeChanged(oldType, value);
			}
		}
	}
	/// <summary>
	/// 블록의 상태가 변경됐을 때
	/// </summary>
	public event Action<Block, State, State> TypeChanged;
	/// <summary>
	/// 블록이 배치됐을 때
	/// </summary>
	public event Action<Block> BlockSpawned;
	/// <summary>
	/// 블록이 제거됐을 때
	/// </summary>
	public event Action<Block> BlockDestroyed;

	public Block(State type, Vector2Int position)
	{
		_type = type;
		Position = position;
	}

	/// <summary>
	/// 블록의 상태가 변경됐을 때 호출됨
	/// </summary>
	void OnTypeChanged(State oldType, State newType)
	{
		TypeChanged?.Invoke(this, oldType, newType);
		if (oldType != State.Placed && newType == State.Placed)
		{
			//블록이 배치됨
			BlockSpawned?.Invoke(this);
		}
		else if (oldType == State.Placed && newType != State.Placed)
		{
			//블록이 제거됨
			BlockDestroyed?.Invoke(this);
		}
	}
}
