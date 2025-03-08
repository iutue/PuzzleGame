using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 앱 버전을 표시하는 텍스트 GUI
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class VersionText : UIBehaviour
{
	protected override void Start()
	{
		GetComponent<TMP_Text>().text = Application.version;
	}
}
