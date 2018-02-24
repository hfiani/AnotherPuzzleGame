using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
	Menu,
	Start,
	Playing,
	Animating,
	End
}

public enum GameMode
{
	None,
	Jigsaw,
	Slide
}

public enum GameOption
{
	Predefined,
	Gallery,
	Camera
}

public class Piece
{
	public int OriginalI { get; set; }
	public int OriginalJ { get; set; }

	public int CurrentI { get; set; }
	public int CurrentJ { get; set; }

	public GameObject GameObject { get; set; }
}

public class Game
{
	#region protected variables
	protected GameObject[] go;
	protected GameObject piece_template;
	protected int ColumnNumber = 4;
	protected int RowNumber = 3;
	protected Texture2D tex;
	protected Piece[,] Matrix;
	#endregion

	#region protected static variables
	protected static GameMode gameMode;
	protected static GameState gameState;
	protected static GameObject Pieces;
	protected static GameObject Frame;
	#endregion

	public static string objectName = "Pieces";
	public static string externalFunctionName = "com.wolfattack.gallery.PuzzleFunctions";

	public Game (GameObject _piece_template, int _ColumnNumber, int _RowNumber)
	{
		piece_template = _piece_template;
		ColumnNumber = _ColumnNumber;
		RowNumber = _RowNumber;

		Matrix = new Piece[RowNumber, ColumnNumber];
	}

	public virtual void Launch ()
	{

	}

	public virtual void Prepare (Texture2D tex)
	{
		Debug.Log ("Game");
	}

	/*public void UpdateState()
	{
		switch (gameState)
		{
			case GameState.Start:
				break;
			case GameState.Playing:
				CheckPieceInput();
				break;
			case GameState.Animating:
				AnimateMovement();
				CheckIfAnimationEnded();
				break;
			case GameState.End:
				if (Input.GetMouseButtonUp(0))
				{
					//reload
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				}
				break;
			default:
				break;
		}
	}*/

	public virtual void UpdateGUIAnimating(int offsetX, int offsetY)
	{
	}

	public virtual void CheckPieceInput()
	{
	}

	public virtual void AnimateMovement()
	{
	}

	public static void Init()
	{
		Pieces = GameObject.FindGameObjectWithTag ("Pieces");
		Frame = GameObject.Find ("Frame");
		Frame.SetActive (false);
		//swapLeft = true;
		gameState = GameState.Menu;
		gameMode = GameMode.None;
	}

	public static GameMode GetGameMode()
	{
		return gameMode;
	}

	public static void SetGameMode(GameMode _gameMode)
	{
		gameMode = _gameMode;
	}

	public static GameState GetGameState()
	{
		return gameState;
	}

	public static void SetGameState(GameState _gameState)
	{
		gameState = _gameState;
	}
}

