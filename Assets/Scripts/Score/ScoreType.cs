using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "new ScoreType", menuName = "Score/Type")]
public class ScoreType : ScriptableObject
{
	/// <summary>
	/// 코드상에서의 이름
	/// </summary>
	[field: SerializeField]
	public string Name { get; private set; }

	/// <summary>
	/// 점수와 함께 표시할 이름
	/// </summary>
	public LocalizedString DisplayName;

	/// <summary>
	/// 점수와 함께 표시할 이미지
	/// </summary>
	[field: SerializeField]
	public Sprite Icon { get; private set; }
}
