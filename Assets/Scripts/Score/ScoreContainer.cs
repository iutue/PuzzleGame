using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ScoreContainer : IEnumerable<Score>
{
	[SerializeField]
	List<Score> _elements;
	/// <summary>
	/// 키(점수 이름)에 대응하는 점수
	/// </summary>
	Dictionary<string, Score> _keyLUT = new();
	public Score this[string scoreName] => _keyLUT[scoreName];

	public void Init()
	{
		//점수 초기화
		for (int i = 0; i < _elements.Count; i++)
		{
			Score score = _elements[i];
			score.Changed += OnScoreChanged;
			score.Reset();
			_keyLUT.TryAdd(score.Type.Key, score);
		}
	}

	/// <summary>
	/// 모든 점수를 기본값으로 초기화
	/// </summary>
	public void ResetAll()
	{
		for (int i = 0; i < _elements.Count; i++)
		{
			_elements[i].Reset();
		}
	}

	/// <summary>
	/// 점수 획득
	/// </summary>
	public bool TryGet(string scoreName, out Score score)
	{
		return _keyLUT.TryGetValue(scoreName, out score);
	}
	/// <summary>
	/// 점수 설정
	/// </summary>
	public void Set(string scoreName, int newBaseValue)
	{
		if (TryGet(scoreName, out var score))
		{
			score.BaseValue = newBaseValue;
		}
	}
	/// <summary>
	/// 점수를 초기값으로 설정
	/// </summary>
	public void Reset(string scoreName)
	{
		if (TryGet(scoreName, out var score))
		{
			score.Reset();
		}
	}

	public void Add(string scoreName, int value)
	{
		if (TryGet(scoreName, out var score))
		{
			score.BaseValue += value;
		}
	}

	void OnScoreChanged(Score score, int oldValue, int newValue)
	{
		switch (score.Type.Group)
		{
			case ScoreType.ScoreGroup.Total:
				if (TryGet("Total", out var totalScore))
				{
					//실제 점수 변화를 총점에 추가
					totalScore.BaseValue += score.CurrentValue - oldValue * score.Multiplier;
				}
				break;

			case ScoreType.ScoreGroup.EndCondition_Or:
				break;

			case ScoreType.ScoreGroup.EndCondition_And:
				break;

			default:
				break;
		}
	}

	public IEnumerator<Score> GetEnumerator() => _elements.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
