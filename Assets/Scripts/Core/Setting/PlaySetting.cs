using UnityEngine;

[CreateAssetMenu(fileName = "PlaySetting", menuName = "Scriptable Objects/Setting/Play")]
public class PlaySetting : ScriptableSetting
{
	/// <summary>
	/// 맵 뷰의 크기
	/// </summary>
	public float MapScale = 0.8f;

	/// <summary>
	/// 카드를 집었을 때 포인터와 카드의 상대 위치
	/// </summary>
	public Vector2 CardOffset = new Vector2(0f, 0.3f);
}
