

public static class Singletone<T> where T : new()
{
	static T _instance;
	public static T Instance => _instance ??= new T();
}
