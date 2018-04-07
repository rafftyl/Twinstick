using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// User interface wave counter.
/// </summary>
public class UIWaveCounter : MonoBehaviour
{
	public Text waveCounter;
	public Text enemyCounter;

	void OnEnable()
    {
		GameEvents.OnEnemySpawned += OnEnemyEvent;
		GameEvents.OnEnemyKilled += OnEnemyEvent;
	}

	void OnDisable()
    {
		GameEvents.OnEnemySpawned -= OnEnemyEvent;
		GameEvents.OnEnemyKilled -= OnEnemyEvent;
	}

	void Start()
    {
		SetTexts (1, 0);
	}

	void SetTexts(int waveNum, int enemyCount)
    {
		waveCounter.text = "Wave: " + waveNum;
		enemyCounter.text = "Enemies remaining: " + enemyCount;
	}

	void OnEnemyEvent(EnemyAI enemy)
    {
		SetTexts (EnemyManager.Instance.WaveNumber, EnemyManager.Instance.EnemyCount);
	}

}
