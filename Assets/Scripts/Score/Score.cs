using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Score
{
	/// <summary>
	/// 점수의 종류
	/// </summary>
	[field: SerializeField]
	public ScoreType Type { get; private set; }

	/// <summary>
	/// 기준 점수를 변경할 때 어떤 점수를 선택하는가
	/// </summary>
	[SerializeField]
	ValueUpdateMethod _valueUpdateMethod = ValueUpdateMethod.New;
	public enum ValueUpdateMethod
	{
		/// <summary>
		/// 새로운 점수
		/// </summary>
		New,
		/// <summary>
		/// 가장 낮은 점수
		/// </summary>
		Min,
		/// <summary>
		/// 가장 높은 점수
		/// </summary>
		Max
	}

	/// <summary>
	/// 점수 배율
	/// </summary>
	[field: SerializeField]
	public int Multiplier { get; private set; } = 1;

	/// <summary>
	/// 초기 점수
	/// </summary>
	[field: SerializeField]
	public int InitialValue { get; private set; }
	/// <summary>
	/// 기준 점수
	/// </summary>
	[SerializeField]
	int _baseValue;
	public int BaseValue
	{
		get => _baseValue;
		set
		{
			int oldBaseValue = _baseValue;
			_baseValue = _valueUpdateMethod switch
			{
				ValueUpdateMethod.New => value,
				ValueUpdateMethod.Min => Mathf.Min(oldBaseValue, value),
				ValueUpdateMethod.Max => Mathf.Max(oldBaseValue, value),
				_ => throw new ArgumentOutOfRangeException()
			};
			if (oldBaseValue != _baseValue)
			{
				Changed(this, oldBaseValue, _baseValue);
			}
		}
	}
	/// <summary>
	/// 실제 점수
	/// </summary>
	public int CurrentValue => BaseValue * Multiplier;

	/// <summary>
	/// 점수가 변경됐을 때
	/// </summary>
	public event Action<Score, int, int> Changed;

	/// <summary>
	/// 점수를 초기값으로 설정
	/// </summary>
	public void Reset()
	{
		int oldBaseValue = _baseValue;
		_baseValue = InitialValue;
		if (oldBaseValue != _baseValue)
		{
			Changed(this, oldBaseValue, _baseValue);
		}
	}
}
