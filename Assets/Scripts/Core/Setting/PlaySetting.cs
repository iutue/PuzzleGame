using UnityEngine;

[CreateAssetMenu(fileName = "PlaySetting", menuName = "Scriptable Objects/Setting/Play")]
public class PlaySetting : ScriptableSetting
{
	/// <summary>
	/// 맵 뷰의 크기
	/// </summary>
	public float MapScale = 0.8f;
}
