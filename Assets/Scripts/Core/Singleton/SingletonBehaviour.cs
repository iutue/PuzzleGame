using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
	public static T Instance { get; private set; }

	protected void Awake()
	{
		if (transform.root == transform)
		{
			DontDestroyOnLoad(gameObject);
		}

		if (Instance)
		{
			//중복된 매니저 제거
			Destroy(gameObject);
		}
		else
		{
			//새로운 싱글톤으로 선택
			Instance = this as T;
		}
	}
}
