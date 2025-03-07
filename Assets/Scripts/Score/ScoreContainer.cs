using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 복수의 점수를 가지는 컨테이너
/// </summary>
[Serializable]
public class ScoreContainer : IEnumerable<Score>
{
	/// <summary>
	/// 모든 점수
	/// </summary>
	[SerializeField]
	List<Score> _elements;
	/// <summary>
	/// 키(점수 이름)에 대응하는 점수
	/// </summary>
	Dictionary<string, Score> _keyLUT = new();
	public Score this[string scoreName] => _keyLUT[scoreName];

	public void Init()
	{
		//모든 점수 초기화
		for (int i = 0; i < _elements.Count; i++)
		{
			Score score = _elements[i];
			score.BaseValueChanged += OnScoreBaseValueChanged;
			score.Reset();
			_keyLUT.TryAdd(score.Type.Key, score);
		}
	}

	/// <summary>
	/// 모든 점수를 기본값으로 재설정
	/// </summary>
	public void ResetAll()
	{
		for (int i = 0; i < _elements.Count; i++)
		{
			_elements[i].Reset();
		}
	}

	/// <summary>
	/// 점수 탐색
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
	/// 점수를 초기값으로 재설정
	/// </summary>
	public void Reset(string scoreName)
	{
		if (TryGet(scoreName, out var score))
		{
			score.Reset();
		}
	}

	/// <summary>
	/// 한 점수의 기준 점수가 변경됐을 때 호출됨
	/// </summary>
	void OnScoreBaseValueChanged(Score score, int oldBaseValue, int newBaseValue)
	{
		switch (score.Type.Group)
		{
			case ScoreType.ScoreGroup.Total:
				if (TryGet("Total", out var totalScore))
				{
					//실제 점수 변화를 총점에 추가
					totalScore.BaseValue += score.CurrentValue - oldBaseValue * score.Multiplier;
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
