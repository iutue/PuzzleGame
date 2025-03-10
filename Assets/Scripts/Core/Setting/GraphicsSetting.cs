using UnityEngine;

[CreateAssetMenu(fileName = "GraphicsSetting", menuName = "Scriptable Objects/Setting/Graphics")]
public class GraphicsSetting : ScriptableSetting
{
	/// <summary>
	/// 목표 프레임률
	/// </summary>
	public int TargetFrameRate = 60;
}
