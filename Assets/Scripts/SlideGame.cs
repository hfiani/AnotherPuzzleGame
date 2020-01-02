using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlideGame : Game
{
	private Vector3 screenPositionToAnimate;
	private int toAnimateI, toAnimateJ;
	private float vertStep;
	private float horiStep;
	private float extraVertStep;
	private float extraHoriStep;
	private Piece PieceToAnimate;
	private Piece PieceToSwap;
	private bool swapLeft = true;
	private bool swapEnabled = false;
	private float AnimSpeed = 60f;

	public SlideGame (GameObject _piece_template, int _ColumnNumber, int _RowNumber)
		:base (_piece_template, _ColumnNumber, _RowNumber)
	{
		gameMode = GameMode.Jigsaw;
	}

	public override void Prepare(Texture2D tex)
	{
		this.tex = tex;
		SliceTexture ();
		OneDToTwoD ();
	}

	public override void Launch() 
	{
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				int random_i = UnityEngine.Random.Range(0, RowNumber);
				int random_j = UnityEngine.Random.Range(0, ColumnNumber);
				//swap'em
				Swap(i, j, random_i, random_j, true);
			}
		}
		go [go.Length - 1].GetComponent<SpriteRenderer> ().enabled = false;
	}

	public override void UpdateGUIAnimating(int offsetX, int offsetY)
	{
		if (swapLeft && GUI.Button (new Rect (offsetX, 50 + offsetY, 100, 50), (swapEnabled?"Swap NOW":"Click to Swap!")))
		{
			swapEnabled = !swapEnabled;
		}
		if (swapLeft && swapEnabled)
		{
			GUI.Label (new Rect (0, 0, 100, 100), "Swap any two of your choice!");
		}
		else
		{
			PieceToSwap = null;
		}
	}

	public override void CheckPieceInput()
	{
		if (Input.GetMouseButtonUp (0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction);

			//check if a piece was hit
			if (hit.collider != null)
			{
				string name = hit.collider.gameObject.name;
				string [] parts = name.Split ('-');
				int iPart = int.Parse (parts [1]);
				int jPart = int.Parse (parts [2]);

				int iFound = -1, jFound = -1;
				//find which one was hit, in our 2D array
				//there must be a better way than this one
				for (int i = 0; i < RowNumber; i++)
				{
					if (iFound != -1)
						break;
					for (int j = 0; j < ColumnNumber; j++)
					{
						if (iFound != -1)
							break;
						if (Matrix [i, j].OriginalI == iPart
							&& Matrix [i, j].OriginalJ == jPart)
						{
							iFound = i;
							jFound = j;
						}
					}
				}

				if (swapEnabled)
				{
					if (swapLeft)
					{
						if (PieceToSwap == null)
						{
							PieceToSwap = Matrix [iFound, jFound];
						}
						else
						{
							Swap (PieceToSwap.CurrentI, PieceToSwap.CurrentJ, iFound, jFound, true);
							CheckForVictory ();
							PieceToSwap = null;
							swapLeft = false;
							swapEnabled = false;
						}
					}
				}
				else
				{
					//check for the null piece, taking into account the game bounds
					bool pieceFound = false;
					if (iFound > 0 && !Matrix [iFound - 1, jFound].GameObject.GetComponent<SpriteRenderer> ().enabled)
					{
						pieceFound = true;
						toAnimateI = iFound - 1;
						toAnimateJ = jFound;
					}
					else if (jFound > 0 && !Matrix [iFound, jFound - 1].GameObject.GetComponent<SpriteRenderer> ().enabled)
					{
						pieceFound = true;
						toAnimateI = iFound;
						toAnimateJ = jFound - 1;
					}
					else if (iFound < RowNumber - 1 && !Matrix [iFound + 1, jFound].GameObject.GetComponent<SpriteRenderer> ().enabled)
					{
						pieceFound = true;
						toAnimateI = iFound + 1;
						toAnimateJ = jFound;
					}
					else if (jFound < ColumnNumber - 1 && !Matrix [iFound, jFound + 1].GameObject.GetComponent<SpriteRenderer> ().enabled)
					{
						pieceFound = true;
						toAnimateI = iFound;
						toAnimateJ = jFound + 1;
					}

					if (pieceFound)
					{
						//get the coordinates of the empty object
						screenPositionToAnimate = Matrix [toAnimateI, toAnimateJ].GameObject.transform.localPosition;
						Matrix [toAnimateI, toAnimateJ].GameObject.transform.localPosition = Matrix [iFound, jFound].GameObject.transform.localPosition;
						PieceToAnimate = Matrix [iFound, jFound];
						gameState = GameState.Animating;
					}
				}
			}
		}
	}

	public override void AnimateMovement()
	{
		//animate it
		//Lerp could also be used, but I prefer the MoveTowards approach :)
		PieceToAnimate.GameObject.transform.localPosition = Vector2.MoveTowards 
			(
				PieceToAnimate.GameObject.transform.localPosition, 
				screenPositionToAnimate, Time.deltaTime * AnimSpeed
			);
		if(Vector2.Distance(PieceToAnimate.GameObject.transform.localPosition, screenPositionToAnimate) < 0.1f)
		{
			PieceToAnimate.GameObject.transform.localPosition = screenPositionToAnimate;
			//make sure they swap, exchange positions and stuff
			Swap(PieceToAnimate.CurrentI, PieceToAnimate.CurrentJ, toAnimateI, toAnimateJ, false);
			//set the required properties
			Matrix[PieceToAnimate.CurrentI, PieceToAnimate.CurrentI].CurrentJ = PieceToAnimate.CurrentI;
			Matrix[PieceToAnimate.CurrentI, PieceToAnimate.CurrentJ].CurrentJ = PieceToAnimate.CurrentJ;
			Matrix[toAnimateI, toAnimateJ].CurrentI = toAnimateI;
			Matrix[toAnimateI, toAnimateJ].CurrentJ = toAnimateJ;
			gameState = GameState.Playing;
			//check if the use has won
			CheckForVictory();
		}
	}

	private void SliceTexture()
	{
		if (go != null)
		{
			foreach(GameObject go_o in go)
			{
				GameObject.Destroy (go_o);
			}
		}
		go = new GameObject[RowNumber * ColumnNumber];

		float tex_width = tex.width - 50f;
		float tex_height = tex.height - 50f;
		Pieces.transform.localScale = new Vector3(tex_width / tex.width, tex_height / tex.height, 0);

		vertStep = tex.height / RowNumber;
		horiStep = tex.width / ColumnNumber;

		extraVertStep = 0;
		extraHoriStep = 0;
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				go [i * ColumnNumber + j] = GameObject.Instantiate (piece_template, Pieces.transform);
				Sprite s = Sprite.Create (tex, new Rect (j * horiStep, (RowNumber - 1 - i) * vertStep, horiStep + extraHoriStep, vertStep + extraVertStep), new Vector2 (0.5f, 0.5f));

				SpriteRenderer sr = go [i * ColumnNumber + j].GetComponent<SpriteRenderer> ();
				sr.sprite = s;
				sr.sortingOrder = i * ColumnNumber + j + 1;

				go [i * ColumnNumber + j].name = "piece-" + i + "-" + j;
			}
		}
	}

	private void OneDToTwoD()
	{
		float maxVertPoints = 10;
		float maxHorizPoints = 14;

		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if ((float)tex.width <= (float)tex.height * 1.2f)
			{
				maxVertPoints = 8;
				maxHorizPoints = 6;
			}
		}
		float vertStepWorld = maxVertPoints / RowNumber;
		float horiStepWorld = maxHorizPoints / ColumnNumber;

		float vertStepLocal = go[0].GetComponent<SpriteRenderer>().sprite.bounds.size.y;
		float horiStepLocal = go[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

		float vertRatio = vertStepWorld / vertStepLocal;
		float horiRatio = horiStepWorld / horiStepLocal;
		float ratio  = Mathf.Min (vertRatio, horiRatio) * 0.8f;

		int width_frame = (int)(tex.width * ratio * 1.015f);
		int height_frame = (int)(tex.height * ratio * 1.015f);
		Texture2D tex2 = GameObject.Instantiate(tex) as Texture2D;
		tex2.Resize(width_frame, height_frame);
		Frame.GetComponent<SpriteRenderer> ().sprite = Sprite.Create (tex2, new Rect (0, 0, tex2.width, tex2.height), new Vector2 (0.5f, 0.5f));
		Frame.SetActive (true);

		Matrix = new Piece[RowNumber, ColumnNumber];
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				go [i * ColumnNumber + j].transform.localScale = new Vector3 (go [i * ColumnNumber + j].transform.localScale.x * ratio, go [i * ColumnNumber + j].transform.localScale.x * ratio, 1);

				Vector3 point = new Vector3(
					horiStepLocal * (j - (float)(ColumnNumber) / 2 + 0.5f) * ratio,
					vertStepLocal * (RowNumber - 0.5f - i - (float)(RowNumber) / 2) * ratio);
				go [i * ColumnNumber + j].transform.localPosition = point;

				//place relevant information
				Matrix[i, j] = new Piece();
				Matrix[i, j].GameObject = go[i * ColumnNumber + j];
				Matrix[i, j].OriginalI = i;
				Matrix[i, j].OriginalJ = j;
				//add a box collider the the raycast to work properly
				if (Matrix[i, j].GameObject.GetComponent<BoxCollider2D>() == null)
					Matrix[i, j].GameObject.AddComponent<BoxCollider2D>();
			}
		}
		GC.Collect ();

		if (Application.platform == RuntimePlatform.Android ||
			Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if ((float)tex.width <= (float)tex.height * 1.2f)
			{
				Utilities.ChangeOrientation (Orientation.VERTICAL);
			}
			else
			{
				Utilities.ChangeOrientation (Orientation.HORIZONTAL);
			}
		}
	}

	private void Swap(int i, int j, int random_i, int random_j, bool changeposition)
	{
		//temp piece, necessary for swapping
		Piece temp = Matrix[i, j];
		Matrix[i, j] = Matrix[random_i, random_j];
		Matrix[random_i, random_j] = temp;

		if (changeposition)
		{
			Vector3 temppos = Matrix [i, j].GameObject.transform.localPosition;
			Matrix [i, j].GameObject.transform.localPosition = Matrix [random_i, random_j].GameObject.transform.localPosition;
			Matrix [random_i, random_j].GameObject.transform.localPosition = temppos;
		}

		//set the required properties
		Matrix[i, j].CurrentI = i;
		Matrix[i, j].CurrentJ = j;
		Matrix[random_i, random_j].CurrentI = random_i;
		Matrix[random_i, random_j].CurrentJ = random_j;
	}

	private void CheckForVictory()
	{
		//dual loop to check the object's properties
		for (int i = 0; i < RowNumber; i++)
		{
			for (int j = 0; j < ColumnNumber; j++)
			{
				if (Matrix [i, j].CurrentI != Matrix [i, j].OriginalI ||
					Matrix [i, j].CurrentJ != Matrix [i, j].OriginalJ)
				{
					return;
				}
			}
		}

		//if we did not return, then we've won!
		gameState = GameState.End;

		go [go.Length - 1].GetComponent<SpriteRenderer> ().enabled = true;
	}
}

