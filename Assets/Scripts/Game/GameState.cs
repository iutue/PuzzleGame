using UnityEngine;

/// <summary>
/// 게임의 상태
/// </summary>
//TODO[추가] Json으로 직렬화해서 게임 상태 저장 기능 추가하기
[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/GameState")]
public class GameState : ScriptableObject
{
	[Header("Match")]
	public ScoreContainer Scores;
	/// <summary>
	/// 한 턴의 제한 시간
	/// </summary>
	[Range(0, 60)]
	public int TimeLimit;

	[Header("Map")]
	/// <summary>
	/// 맵에 적용할 테마
	/// </summary>
	public BlockTheme MapTheme;

	[Header("Card")]
	/// <summary>
	/// 한 플레이어가 최대로 가질 수 있는 카드 수
	/// </summary>
	[Range(1, 10)]
	public int HandCapacity;
	/// <summary>
	/// 카드를 한 번 뽑을 때 가져올 카드 수
	/// </summary>
	[Range(0, 5)]
	public int DrawCount;
	/// <summary>
	/// 카드에 무작위로 적용할 테마
	/// </summary>
	public BlockTheme[] CardThemes;
}
