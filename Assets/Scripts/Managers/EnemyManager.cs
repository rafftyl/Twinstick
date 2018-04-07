using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A singleton governing enemy waves
/// </summary>
public class EnemyManager : Singleton<EnemyManager>
{

	/// <summary>
	/// A helper struct used for choosing enemies to spawn 
	/// </summary>
	[System.Serializable]
	public class EnemyProbabilityPair
    {
		public EnemyAI enemy;
		public float probability;
	}

	/// <summary>
	/// Enemy wave.
	/// </summary>
	[System.Serializable]
	public class EnemyWave
	{
		/// <summary>
		/// Array of enemies with their corresponding spawn probabilities 
		/// </summary>
		public EnemyProbabilityPair[] enemyData;

		/// <summary>
		/// The enemy count in this wave.
		/// </summary>
		public int enemyCount;

		/// <summary>
		/// Time needed to spawn all the enemies in this wave
		/// </summary>
		public float spawnDuration;
	}


	public Transform[] spawnPoints;
	public EnemyWave[] enemyWaveData;

	/// <summary>
	/// Collisions with these layers are checked before spawning an enemy 
	/// </summary>
	[Tooltip("Collisions with these layers are checked before spawning an enemy")]
	public LayerMask collisionCheckLayers;

	/// <summary>
	/// Radius of collision check
	/// </summary>
	[Tooltip("Radius of collision check")]
	public float collisionCheckRadius;

	int currentWaveIndex = -1;
	float spawnTimestep;

	int enemiesSpawned = 0;
	int enemiesKilled = 0;
	List<EnemyAI> spawnedEnemies = new List<EnemyAI>();

	public EnemyWave CurrentWave
    {
		get
        {
			if (currentWaveIndex < enemyWaveData.Length)
            {
				return enemyWaveData [currentWaveIndex];
			}
            else
            {
				return null;
			}
		}
	}

	public int EnemyCount
    {
		get
        {
			return spawnedEnemies.Count;
		}
	}

	public int WaveNumber
    {
		get
        {
			return currentWaveIndex + 1;
		}
	}

	public List<EnemyAI> SpawnedEnemies
    {
		get
        {
			return spawnedEnemies;
		}
	}

	protected override void Awake()
    {
		base.Awake ();
		if (spawnPoints.Length > 0 && enemyWaveData.Length > 0)
        {
			StartNextWave ();
		}
	}

	void StartNextWave()
    {
		currentWaveIndex++;
		enemiesKilled = 0; 
		enemiesSpawned = 0;
		spawnedEnemies.Clear ();
		spawnTimestep = CurrentWave.spawnDuration /((float) CurrentWave.enemyCount);
		StartCoroutine (SpawnCoroutine());
	}

	void OnEnable()
    {
		GameEvents.OnEnemyKilled += OnEnemyKilled;	
	}

	void OnDisable()
    {
		GameEvents.OnEnemyKilled -= OnEnemyKilled;
	}

	/// <summary>
	/// Checks the state of the game after an  enemy is killed and starts another wave/ends the game when necessary
	/// </summary>
	/// <param name="enemy">Enemy killed.</param>
	void OnEnemyKilled(EnemyAI enemy)
    {
		enemiesKilled++;
		//Debug.Log(mEnemiesKilled.ToString() +  " killed of " + CurrentWave.enemyCount);
		spawnedEnemies.Remove (enemy);
		if (enemiesKilled == CurrentWave.enemyCount)
        {
			if (currentWaveIndex < enemyWaveData.Length - 1)
            {
				StartNextWave ();
			}
            else
            {
				GameController.Instance.WinGame ();
			}
		}
	}

	/// <summary>
	/// Removes all the enemies from the level 
	/// </summary>
	void ClearEnemies()
    {
		foreach (EnemyAI enemy in spawnedEnemies)
        {
			Destroy (enemy.gameObject);
		}
		spawnedEnemies.Clear ();
	}

	IEnumerator SpawnCoroutine()
    {
		//while there are enemies left to spawn
		while (CurrentWave != null && enemiesSpawned < CurrentWave.enemyCount)
        {			
			yield return new WaitForSeconds (spawnTimestep); //wait for a while

			//randomly pick a spawn point
			Transform spawnPoint = spawnPoints [Random.Range (0, spawnPoints.Length)];

			//pick a number from 0-1 range and iterate through the enemy list until their accumulated probability is less than this number
			float rand = Random.Range (0, 1f);
			float accumProbability = 0;
			foreach (EnemyProbabilityPair enemyData in CurrentWave.enemyData)
            {
				if (enemyData.probability + accumProbability > rand)
                {
					//if there's anything around the spawn point, don't spawn
					if (!Physics.CheckSphere (spawnPoint.position, collisionCheckRadius, collisionCheckLayers))
                    {
						EnemyAI enemy = Instantiate<EnemyAI> (enemyData.enemy);
						enemy.transform.position = spawnPoint.position;
						enemy.transform.rotation = spawnPoint.rotation;
						spawnedEnemies.Add (enemy);
						enemiesSpawned++;
						GameEvents.EnemySpawned (enemy);
					}
					break;					
				}
				accumProbability += enemyData.probability;
			}
		}
	}

	#if UNITY_EDITOR
	void OnValidate()
    {
		//renormalize  probabilities for each wave
		foreach (EnemyWave waveData in enemyWaveData)
        {
			float sum = 0;
			foreach (EnemyProbabilityPair pair in waveData.enemyData)
            {
				if (pair.probability < 0)
                {
					pair.probability = 0;
				}
				sum += pair.probability;
			}

			if (sum > 0)
            {
				foreach (EnemyProbabilityPair pair in waveData.enemyData)
                {
					pair.probability /= sum;
				}
			}
		}
	}
	#endif
}

