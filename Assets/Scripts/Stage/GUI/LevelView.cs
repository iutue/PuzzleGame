using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelView : UIBehaviour
{
	LevelData _owner;

	[SerializeField]
	TMP_Text _titleText;
	[SerializeField]
	Image _titleBackground;
	[SerializeField]
	RectTransform _stageButtonParent;
	[SerializeField]
	GameObject _stageButtonPrefab;

	public void Init(LevelData data)
	{
		_owner = data;
		_titleText.text = data.Title.GetLocalizedString();
		_titleBackground.sprite = data.TitleBackground;
		//스테이지 버튼 생성
		for (int i = 0; i < data.Stages.Length; i++)
		{
			var stageView = Instantiate(_stageButtonPrefab, _stageButtonParent, false).GetComponent<StageView>();
			stageView.Init(data.Stages[i], OnStageButtonClicked);
		}
	}

	void OnStageButtonClicked(StageData stage)
	{
		MatchManager.Instance.LoadStageAsync(null, _owner, _owner.Stages[stage.Index]);
	}
}
