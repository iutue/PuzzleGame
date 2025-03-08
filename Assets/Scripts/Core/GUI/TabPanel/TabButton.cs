using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TabButton : UIBehaviour
{
	/// <summary>
	/// 이 탭의 번호
	/// </summary>
	int _index;
	Action<int> _clicked;

	public void Init(int index, Action<int> clicked)
	{
		_index = index;
		_clicked = clicked;
		GetComponent<Button>().onClick.AddListener(new UnityAction(OnClicked));
	}

	/// <summary>
	/// 탭이 클릭됐을 때 호출됨
	/// </summary>
	void OnClicked() => _clicked(_index);
}
