using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class ScoreTable : IEnumerable<Score>
{
	[SerializeField]
	List<Score> _scores;
	/// <summary>
	/// 점수의 이름에 대응하는 점수 인스턴스
	/// </summary>
	Dictionary<string, Score> _nameLUT;
	public Score this[string scoreName] => _nameLUT[scoreName];

	public void Init()
	{
		_nameLUT = new Dictionary<string, Score>();
		for (int i = _scores.Count - 1; i >= 0; i--)
		{
			ScoreType type = _scores[i].Type;
			if (!type || _nameLUT.ContainsKey(type.Name))
			{
				//유효하지 않은 원소 제거
				_scores.RemoveAt(i);
				continue;
			}
			else
			{
				_nameLUT.Add(type.Name, _scores[i]);
			}
		}

		//모든 점수 초기화
		ResetAll();
	}

	/// <summary>
	/// 모든 점수 초기화
	/// </summary>
	public void ResetAll()
	{
		for (int i = 0; i < _scores.Count; i++)
		{
			_scores[i].Reset();
		}
	}

	public Score Get(string scoreName)
	{
		return _nameLUT[scoreName];
	}
	public void Set(string scoreName, int newBaseValue)
	{
		_nameLUT[scoreName].BaseValue = newBaseValue;
	}
	/// <summary>
	/// 점수를 초기값으로 설정
	/// </summary>
	public void Reset(string scoreName)
	{
		_nameLUT[scoreName].Reset();
	}

	public IEnumerator<Score> GetEnumerator() => _scores.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
