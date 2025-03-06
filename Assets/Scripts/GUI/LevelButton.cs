using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : UIBehaviour
{
	/// <summary>
	/// 불러올 레벨 씬의 이름
	/// </summary>
	[SerializeField]
	string _levelName;

	protected override void Start()
	{
		base.Start();
		GetComponent<Button>().onClick.AddListener(new UnityAction(OnButtonClicked));
	}

	/// <summary>
	/// 버튼이 클릭됐을 때 호출됨
	/// </summary>
	void OnButtonClicked()
	{
		TransitionManager.Instance.LoadSceneAsync(_levelName);
	}
}
