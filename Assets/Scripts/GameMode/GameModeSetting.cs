using UnityEngine;

[CreateAssetMenu(fileName = "GameModeSetting", menuName = "Scriptable Objects/Setting/GameMode")]
public class GameModeSetting : ScriptableSetting
{
	/// <summary>
	/// 한 턴의 제한 시간
	/// </summary>
	[Range(0, 60)]
	public int TimeLimit;

	/// <summary>
	/// 맵의 크기
	/// </summary>
	public Vector2Int MapSize;
	/// <summary>
	/// 맵에 적용할 테마
	/// </summary>
	public BlockTheme MapTheme;

	/// <summary>
	/// 한 플레이어가 최대로 가질 수 있는 카드 수
	/// </summary>
	[SerializeField, Range(1, 10)]
	public int HandCapacity;
	/// <summary>
	/// 카드를 한 번 뽑을 때 가져올 카드 수
	/// </summary>
	[SerializeField, Range(0, 5)]
	public int DrawCount;
	/// <summary>
	/// 카드에 무작위로 적용할 테마
	/// </summary>
	public BlockTheme[] CardThemes;
}
