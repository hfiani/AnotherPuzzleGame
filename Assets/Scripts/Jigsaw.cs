using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class Jigsaw : MonoBehaviour
{
	#region private variables
	/*private Vector3 screenPositionToAnimate;
	private int toAnimateI, toAnimateJ;
	private GameState gameState;
	private GameMode gameMode;
	private GameObject[] go;
	private GameObject Pieces;
	private GameObject Frame;
	private float vertStep;
	private float horiStep;
	private float extraVertStep;
	private float extraHoriStep;
	//jigsaw only
	private Vector3[] snappingPositions;
	private Piece PieceToAnimateJigsaw;
	// slide only
	private Piece PieceToAnimateSlide;
	private Piece PieceToSwapSlide;
	private bool swapLeft = true;
	private bool swapEnabled = false;
	private Piece[,] MatrixJigsaw;
	private Piece[,] MatrixSlide;*/
	private Game game;
	private bool modeSelected = false;
	#endregion

	#region private serialized variables
	[SerializeField] private int ColumnNumber = 4;
	[SerializeField] private int RowNumber = 3;
	[SerializeField] private GameObject piece_template_jigsaw;
	[SerializeField] private GameObject piece_template_slide;
	#endregion

	#region private static variables
	#endregion

    // Use this for initialization
    void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (Utilities.unityClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"))
			{
				Utilities.currentActivity = Utilities.unityClass.GetStatic<AndroidJavaObject> ("currentActivity");
			}
			using (Utilities.puzzleFunctionsClass = new AndroidJavaClass (Game.externalFunctionName))
			{
				if (Utilities.puzzleFunctionsClass != null)
				{
					Utilities.puzzleFunctionsClass.CallStatic ("setInfos", Utilities.currentActivity, name, "onPhotoPick");
				}
			}
			Utilities.ChangeOrientation (Orientation.HORIZONTAL);
		}

		Game.objectName = name;

		Game.Init ();
    }

	private void PredefinedPic()
	{
		Texture2D [] textures = Resources.LoadAll<Texture2D> ("Textures/Anime");
		Texture2D tex = textures [UnityEngine.Random.Range (0, textures.Length)];
        
		if (game != null)
		{
			game.Prepare (tex);
		}
	}

	public void onPhotoPick(string photoPath)
	{
		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists (photoPath))
		{
			fileData = File.ReadAllBytes (photoPath);
			tex = new Texture2D (1, 1);
			tex.LoadImage (fileData);

			if (tex != null && game != null)
			{
				game.Prepare (tex);
			}
			else
			{
				Screen.orientation = Utilities.lastOrientation;
			}
		}
		else
		{
			Screen.orientation = Utilities.lastOrientation;
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				Application.Quit ();
			}
		}

		switch (Game.GetGameState())
        {
            case GameState.Start:
                break;
			case GameState.Playing:
				if (game != null)
				{
					game.CheckPieceInput ();
				}
                break;
			case GameState.Animating:
				if (game != null)
				{
					game.AnimateMovement();
				}
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
    }

    /// <summary>
    /// boring UI, waiting for uGUI framework :)
    /// </summary>
    void OnGUI()
	{
		int offsetX = 0;
		int offsetY = 0;
		if (Utilities.consideredOrientation == Orientation.HORIZONTAL)
		{
			offsetX = 0;
			offsetY = 50;
		}
		else if (Utilities.consideredOrientation == Orientation.VERTICAL)
		{
			offsetX = 100;
			offsetY = 0;
		}

		if (GUI.Button (new Rect (Screen.width - 100, Screen.height - 50, 100, 50), "Back"))
		{
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}

		switch (Game.GetGameState())
		{
			case GameState.Menu:
				int order = -1;
				if (order++ >= -1 && Game.GetGameMode() == GameMode.None && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Jigsaw"))
				{
					/*gameState = GameState.Start;
					gameMode = GameMode.Jigsaw;*/
					Game.SetGameState (GameState.Start);
					game = new JigsawGame (piece_template_jigsaw, ColumnNumber, RowNumber);
				}
				if (order++ >= -1 && Game.GetGameMode() == GameMode.None && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Slide"))
				{
					/*gameState = GameState.Start;
					gameMode = GameMode.Slide;*/
					Game.SetGameState (GameState.Start);
					game = new SlideGame (piece_template_slide, ColumnNumber, RowNumber);
				}
				break;
			case GameState.Start:
				order = -1;
				if (order++ >= -1 && modeSelected && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Start"))
				{
					/*switch(gameMode)
					{
						case GameMode.Jigsaw:
							ShuffleJigsaw ();
							break;
						case GameMode.Slide:
							go [go.Length - 1].GetComponent<SpriteRenderer> ().enabled = false;
							ShuffleSlide ();
							break;
					}*/
					if (game != null)
					{
						game.Launch ();
					}

					Game.SetGameState(GameState.Playing);
				}
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Predefined"))
				{
					PredefinedPic();
					modeSelected = true;
				}
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Gallery"))
				{
					Utilities.lastOrientation = Screen.orientation;
					if (Application.platform == RuntimePlatform.Android)
					{
						Debug.Log ("OnGUI before call");
						using (Utilities.puzzleFunctionsClass = new AndroidJavaClass (Game.externalFunctionName))
						{
							if (Utilities.puzzleFunctionsClass != null)
							{
								Utilities.puzzleFunctionsClass.CallStatic ("setInfos", Utilities.currentActivity, name, "onPhotoPick");
								Utilities.puzzleFunctionsClass.CallStatic ("openGallery");
							}
						}
					}
				}
				if (order++ >= -1 && GUI.Button (new Rect (order * offsetX, 50 + order * offsetY, 100, 50), "Camera"))
				{
					//cameraStart ();
					using (Utilities.puzzleFunctionsClass = new AndroidJavaClass (Game.externalFunctionName))
					{
						if (Utilities.puzzleFunctionsClass != null)
						{
							Utilities.puzzleFunctionsClass.CallStatic ("setInfos", Utilities.currentActivity, name, "onPhotoPick");
							Utilities.puzzleFunctionsClass.CallStatic ("openCamera");
						}
					}

					Utilities.lastOrientation = Screen.orientation;
					Utilities.ChangeOrientation (Orientation.HORIZONTAL);
				}
				break;
			case GameState.Playing:
			case GameState.Animating:
				if (GUI.Button (new Rect (0, 50, 100, 50), "Reset"))
				{
					modeSelected = false;

					SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
				}
				if (game != null)
				{
					game.UpdateGUIAnimating (offsetX, offsetY);
				}
				break;
            case GameState.End:
                GUI.Label(new Rect(0, 0, 100, 100), "Congrats, tap to start over!");
                break;
            default:
                break;
        }
	}
}
