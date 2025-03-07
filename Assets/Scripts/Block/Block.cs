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
	State _currentState;
	public State CurrentState
	{
		get => _currentState;
		set
		{
			State oldState = _currentState;
			if (value != oldState)
			{
				_currentState = value;
				OnStateChanged(oldState, value);
			}
		}
	}
	/// <summary>
	/// 블록의 상태가 변경됐을 때
	/// </summary>
	public event Action<Block, State, State> StateChanged;
	/// <summary>
	/// 블록이 배치됐을 때
	/// </summary>
	public event Action<Block> BlockSpawned;
	/// <summary>
	/// 블록이 제거됐을 때
	/// </summary>
	public event Action<Block> BlockDestroyed;

	public Block(State state, Vector2Int position)
	{
		_currentState = state;
		Position = position;
	}

	/// <summary>
	/// 블록의 상태가 변경됐을 때 호출됨
	/// </summary>
	void OnStateChanged(State oldState, State newState)
	{
		StateChanged?.Invoke(this, oldState, newState);
		if (oldState != State.Placed && newState == State.Placed)
		{
			//블록이 배치됨
			BlockSpawned?.Invoke(this);
		}
		else if (oldState == State.Placed && newState != State.Placed)
		{
			//블록이 제거됨
			BlockDestroyed?.Invoke(this);
		}
	}
}
