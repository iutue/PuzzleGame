using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "new ScoreType", menuName = "Score/Type")]
public class ScoreType : ScriptableObject
{
	/// <summary>
	/// 코드상에서 점수를 식별하는데 사용하는 이름
	/// </summary>
	[field: SerializeField]
	public string Key { get; private set; }

	/// <summary>
	/// 점수가 속한 그룹
	/// </summary>
	[field: SerializeField]
	public ScoreGroup Group { get; private set; }
	public enum ScoreGroup
	{
		None,
		/// <summary>
		/// 총점 계산에 포함되는 점수
		/// </summary>
		Total,
		/// <summary>
		/// 하나만 만족하면 종료하는 조건
		/// </summary>
		EndCondition_Or,
		/// <summary>
		/// 모두 만족해야 종료하는 조건
		/// </summary>
		EndCondition_And,
		///// <summary>
		///// 하나만 만족하면 승리하는 조건
		///// </summary>
		//WinCondition_Or,
		///// <summary>
		///// 모두 만족해야 승리하는 조건
		///// </summary>
		//WinCondition_And,
		///// <summary>
		///// 하나만 만족하면 패배하는 조건
		///// </summary>
		//LoseCondition_Or,
		///// <summary>
		///// 모두 만족해야 패배하는 조건
		///// </summary>
		//LoseCondition_And,
	}

	/// <summary>
	/// 점수 패널 GUI
	/// </summary>
	[field: SerializeField]
	public GameObject ScorePanelPrefab { get; private set; }

	/// <summary>
	/// 점수와 함께 표시할 이름
	/// </summary>
	[field: SerializeField]
	public LocalizedString DisplayName { get; private set; }
	/// <summary>
	/// 점수와 함께 표시할 이미지
	/// </summary>
	[field: SerializeField]
	public Sprite Icon { get; private set; }
}
