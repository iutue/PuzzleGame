using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 레벨로 이동할 수 있는 버튼
/// </summary>
[RequireComponent(typeof(Button))]
public class LevelButton : UIBehaviour
{
	/// <summary>
	/// 불러올 레벨 씬의 이름
	/// </summary>
	[SerializeField]
	string _levelName;

	/// <summary>
	/// 레벨의 모드
	/// </summary>
	[SerializeField]
	TMP_Text _levelModeText;
	/// <summary>
	/// 레벨의 맵 크기
	/// </summary>
	[SerializeField]
	TMP_Text _levelSizeText;

	protected override void Start()
	{
		base.Start();
		GetComponent<Button>().onClick.AddListener(new UnityAction(OnButtonClicked));

		//레벨 정보 표시
		var levelInfos = _levelName.Split('_');
		if (levelInfos.Length >= 2)
		{
			if (_levelModeText) _levelModeText.text = levelInfos[0];
			if (_levelSizeText) _levelSizeText.text = levelInfos[1];
		}
	}

	/// <summary>
	/// 버튼이 클릭됐을 때 호출됨
	/// </summary>
	void OnButtonClicked()
	{
		TransitionManager.Instance.LoadSceneAsync(_levelName);
	}
}
