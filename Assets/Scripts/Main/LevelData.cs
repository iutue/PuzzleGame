using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
	public StageData[] Stages;
}

[Serializable]
public class StageData
{
	public GameObject Mode;
	public GameObject Map;
}
