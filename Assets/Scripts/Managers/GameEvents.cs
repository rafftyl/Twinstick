using UnityEngine;
using System.Collections;

//a static class aggregating various game  events
public static class GameEvents
{
	public delegate void EnemyEvent(EnemyAI enemy);
	public static event EnemyEvent OnEnemyHit;
	public static event EnemyEvent OnEnemyKilled;
	public static event EnemyEvent OnEnemySpawned;

	public delegate void PlayerEvent(Player player);
	public static event PlayerEvent OnPlayerHit;
	public static event PlayerEvent OnPlayerKilled;
	public static event PlayerEvent OnPlayerInventoryChanged;

	public static void PlayerHit(Player player)
    {
		if (OnPlayerHit != null)
        {
			OnPlayerHit (player);
		}
	}

	public static void PlayerKilled(Player player)
    {
		if (OnPlayerKilled != null)
        {
			OnPlayerKilled (player);
		}
	}

	public static void PlayerInventoryChanged(Player player)
    {
		if (OnPlayerInventoryChanged != null)
        {
			OnPlayerInventoryChanged (player);
		}
	}

	public static void EnemyHit(EnemyAI enemy)
    {
		if (OnEnemyHit != null)
        {
			OnEnemyHit (enemy);
		}
	}

	public static void EnemyKilled(EnemyAI enemy)
    {
		if (OnEnemyKilled != null)
        {
			OnEnemyKilled (enemy);
		}
	}

	public static void EnemySpawned(EnemyAI enemy)
    {
		if (OnEnemySpawned != null)
        {
			OnEnemySpawned (enemy);
		}
	}
}
