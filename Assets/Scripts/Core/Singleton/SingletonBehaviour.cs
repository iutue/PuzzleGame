using UnityEngine;

/// <summary>
/// 씬에 하나만 존재하는 싱글톤 컴포넌트
/// </summary>
public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
	public static T Instance { get; private set; }

	protected void Awake()
	{
		DontDestroyOnLoad(gameObject);

		if (Instance)
		{
			//중복된 싱글톤 제거
			Destroy(gameObject);
		}
		else
		{
			//사용할 싱글톤으로 등록
			Instance = this as T;
		}
	}
}
