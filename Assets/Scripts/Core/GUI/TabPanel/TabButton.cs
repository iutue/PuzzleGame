using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 탭을 열 때 사용하는 버튼 GUI
/// </summary>
[RequireComponent(typeof(Button))]
public class TabButton : UIBehaviour
{
	/// <summary>
	/// 패널에서 이 탭의 번호
	/// </summary>
	int _index;
	Action<int> _clicked;
	/// <summary>
	/// 선택됐을 때만 활성화할 이미지
	/// </summary>
	[field: SerializeField]
	public Image Highlight { get; private set; }

	public void Init(int index, Action<int> clicked)
	{
		_index = index;
		_clicked = clicked;
		GetComponent<Button>().onClick.AddListener(new UnityAction(OnClicked));
	}

	/// <summary>
	/// 탭 버튼이 클릭됐을 때 호출됨
	/// </summary>
	void OnClicked() => _clicked(_index);
}
