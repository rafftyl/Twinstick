using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIWinScreen : MonoBehaviour, IPointerClickHandler
{
	public void OnPointerClick(PointerEventData data)
    {
		GameController.Instance.ResetGame ();
	}
}
