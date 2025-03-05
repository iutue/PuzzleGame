using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Localization;

[Serializable]
public class Score
{
	/// <summary>
	/// 점수의 종류
	/// </summary>
	[field: SerializeField]
	public ScoreType Type
	{
		get;
#if UNITY_EDITOR
		set;
#endif
	}

	/// <summary>
	/// 기준 점수를 변경할 때 어떤 점수를 선택하는가
	/// </summary>
	[SerializeField]
	BaseValueType _baseValueType = BaseValueType.New;
	public enum BaseValueType
	{
		/// <summary>
		/// 새로운 점수
		/// </summary>
		New,
		/// <summary>
		/// 최소 점수
		/// </summary>
		Min,
		/// <summary>
		/// 최대 점수
		/// </summary>
		Max
	}

	/// <summary>
	/// 점수 배율
	/// </summary>
	[SerializeField]
	int _multiplier = 1;

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
			_baseValue = _baseValueType switch
			{
				BaseValueType.New => value,
				BaseValueType.Min => Mathf.Min(oldBaseValue, value),
				BaseValueType.Max => Mathf.Max(oldBaseValue, value),
				_ => 0
			};
			Changed?.Invoke(oldBaseValue, _baseValue);
		}
	}
	/// <summary>
	/// 실제 점수
	/// </summary>
	public int CurrentValue => BaseValue * _multiplier;

	/// <summary>
	/// 점수가 변경됐을 때
	/// </summary>
	public event Action<int, int> Changed;

	/// <summary>
	/// 점수를 초기값으로 설정
	/// </summary>
	public void Reset()
	{
		int oldBaseValue = _baseValue;
		_baseValue = InitialValue;
		Changed?.Invoke(oldBaseValue, _baseValue);
	}
}
