using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// A singleton class used to reset or end the game 
/// </summary>
public class GameController : Singleton<GameController>
{
	/// <summary>
	/// Ground layers - used to detect mouse position on the ground
	/// </summary>
	[Tooltip("Ground layers - used to detect mouse position on the ground")]
	public LayerMask groundLayers;
	public float groundRaycastDist = 100;

	/// <summary>
	/// The screen displayed after winning the game
	/// </summary>
	public UIWinScreen winScreen;

	/// <summary>
	/// Player's character
	/// </summary>
	public Player player;

	/// <summary>
	/// Resets the game.
	/// </summary>
	public void ResetGame()
    {
		SceneManager.LoadScene (0);
		Time.timeScale = 1f;
	}

	/// <summary>
	/// Displays the win screen.
	/// </summary>
	public void WinGame()
    {
		Time.timeScale = 0.1f;
		winScreen.gameObject.SetActive (true);
	}
}
