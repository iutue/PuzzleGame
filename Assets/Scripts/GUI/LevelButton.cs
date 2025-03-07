using TMPro;
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

	[SerializeField]
	TMP_Text _levelModeText;
	[SerializeField]
	TMP_Text _levelSizeText;

	protected override void Start()
	{
		base.Start();
		GetComponent<Button>().onClick.AddListener(new UnityAction(OnButtonClicked));

		//레벨 정보 표시
		var levelSpecs = _levelName.Split('_');
		if (_levelModeText) _levelModeText.text = levelSpecs[0];
		if (_levelSizeText) _levelSizeText.text = levelSpecs[1];
	}

	/// <summary>
	/// 버튼이 클릭됐을 때 호출됨
	/// </summary>
	void OnButtonClicked()
	{
		TransitionManager.Instance.LoadSceneAsync(_levelName);
	}
}
