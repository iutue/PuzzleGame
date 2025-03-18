using System;
using UnityEngine;

public class GameMap : MonoBehaviour
{
	/// <summary>
	/// 맵의 크기
	/// </summary>
	[SerializeField]
	Vector2Int _initialMapSize;
	/// <summary>
	/// 현재 맵
	/// </summary>
	public BlockGroup Blocks { get; private set; }
	public event Action<Block> MapBlockSpawned;
	public event Action<Block> MapBlockDestroyed;

	[SerializeField]
	public MapView MapView;
	/// <summary>
	/// 맵에 적용할 테마
	/// </summary>
	[SerializeField]
	BlockTheme[] _mapThemes;

	public void Init()
	{
		MapView.Init(_mapThemes);
	}

	public void ResetMap()
	{
		var mapTemplate = new Block.State[_initialMapSize.x, _initialMapSize.y];
		Blocks = new BlockGroup(mapTemplate);
		Blocks.BlockSpawned += OnMapBlockSpawned;
		Blocks.BlockDestroyed += OnMapBlockDestroyed;
		MapView.ResetMap(Blocks);
	}

	public void Open()
	{
		MapView.Open();
	}
	public void Close()
	{
		MapView.Close();
	}

	#region Callbacks
	/// <summary>
	/// 맵에 블록이 배치됐을 때 호출됨
	/// </summary>
	protected virtual void OnMapBlockSpawned(Block block)
	{
		MapBlockSpawned?.Invoke(block);
	}

	/// <summary>
	/// 맵의 블록이 제거됐을 때 호출됨
	/// </summary>
	protected virtual void OnMapBlockDestroyed(Block block)
	{
		MapBlockDestroyed?.Invoke(block);
	}
	#endregion
}
