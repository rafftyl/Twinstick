using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// User interface element displaying player's health
/// </summary>
public class UIHealthBar : MonoBehaviour
{
	public Slider slider;
	bool isInit  = false;
	void OnEnable()
    {
		GameEvents.OnPlayerHit += OnPlayerHit;
	}

	void OnDisable()
    {
		GameEvents.OnPlayerHit -= OnPlayerHit;
	}

	void OnPlayerHit(Player player)
    {
		if (!isInit)
        {
			slider.maxValue = player.maxHealth;
			slider.wholeNumbers = true;
			isInit = false;
		}

		slider.value = player.CurrentHealth;
	}
}
