using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {


	public LevelData levelData;
	protected Vector2 worldSize;
	public int Height {
		get {
			return pieces.GetLength(1);
		}
	}

	public int Width { 
		get {
			return pieces.GetLength (0);
		}
	}
	protected Piece[,] pieces;


	public SpriteRenderer border;
	public Transform mask;

	public InputManager inputManager;
	public MatchFinder matchFinder;

	#region initialization
	void Start() {
		Init ();
		StartGame ();
	}

	public virtual void Init() {
		matchFinder = new MatchFinder ();
		matchFinder.board = this;
	}

	public virtual void StartGame() {
		GenerateBoard (levelData);
		inputManager.onTouch += OnTouch;
	}

	public virtual void Release() {
		inputManager.onTouch -= OnTouch;
	}

	#endregion



	#region coordinates

	public virtual void GenerateBoard(LevelData data) {
		if (data.definition != null) {
			data.ParseDefinition ();
		}

		pieces = new Piece[data.width, data.height];
		worldSize = new Vector2 (levelData.width * Piece.size, levelData.height * Piece.size);


		border.size = new Vector2 (data.width * 5 + 1, data.height * 5 + 1);
		mask.localScale = new Vector3 (data.width * 5, data.height * 5);
		Camera.main.orthographicSize = data.width;
	}


	public virtual void ClearBoard() {

	}


	public Vector2Int GetCoordinates(Vector3 worldPosition) {
		worldPosition = transform.InverseTransformPoint (worldPosition);
		worldPosition += (Vector3)worldSize / 2f;
		Vector2Int coordinates = new Vector2Int((int)(worldPosition.x / Piece.size), (int)(worldPosition.y / Piece.size));
		return coordinates;
	}

	public Vector3 GetPiecePosition(Vector2Int coordinates) {
		Vector3 position = -worldSize / 2f;
		position += new Vector3 ((coordinates.x + 0.5f) * Piece.size, (coordinates.y + 0.5f) * Piece.size, 0);
		return position;
	}

	public bool HasPiece(Vector2Int coordinates) {
		return HasPiece (coordinates.x, coordinates.y);
	}

	public bool HasPiece(int x, int y) {
		return x >= 0 && x < pieces.GetLength (0) && y >= 0 && y < pieces.GetLength (1);
	}

	public Piece GetPiece(Vector2Int coordinates) {
		return GetPiece (coordinates.x, coordinates.y);
	}

	public Piece GetPiece(int x, int y) {
		return pieces [x, y];
	}

	#endregion



	#region states 

	protected BoardState currentState = BoardState.Disabled;

	public virtual void OnMatchesFound() {

	}


	#endregion

	#region input

	public virtual void OnTouch(Vector3 position, TouchPhase status) {

	}

	#endregion



	protected List<Piece> piecesInProgress = new List<Piece>();
	#region movement 

	protected virtual void MovePiece(Vector2Int from, Vector2Int to, float speed) {
		if(HasPiece(from)) {
			var piece = GetPiece (from);
			MovePiece(piece, to, speed);
		}
	}

	protected virtual void MovePiece(Piece piece, Vector2Int destination, float speed) {
		if (piece != null) {
			if (!piecesInProgress.Contains (piece)) {
				piecesInProgress.Add (piece);
				piece.MoveTo (GetPiecePosition (destination), OnPieceArrived, speed);
			} else {
				Debug.Log (piecesInProgress.Count);
			}
		}
	}


	public virtual void OnPieceArrived(Piece piece) {
		if (piecesInProgress.Contains (piece)) {
			piecesInProgress.Remove (piece);
		}

		if (piecesInProgress.Count == 0) {
			OnAllPiecesArrived ();
		}
	}

	protected virtual void OnAllPiecesArrived() {
	}

	#endregion




	#region destruction

	public virtual void DestroyPieces(Vector2Int[] coordinates) {
		piecesInProgress = new List<Piece> ();
		for (int i = 0; i < coordinates.Length; i++) {
			DestroyPiece (coordinates [i], 0);
		}
	}

	protected virtual void DestroyPiece(Vector2Int coordinates, float delay) {
		if (HasPiece (coordinates)) {
			var piece = GetPiece (coordinates);
			if (piece != null && (piece.state != PieceState.Destroying && piece.state != PieceState.Destroyed)) {
				if (!piecesInProgress.Contains (piece)) {
					piecesInProgress.Add (piece);
					piece.Release (OnPieceDestroyed, delay);
				}
			}
		}
	}

	protected virtual void OnPieceDestroyed(Piece piece) {
		if(piecesInProgress.Contains(piece)) {
			piecesInProgress.Remove (piece);
		}

		if (piecesInProgress.Count == 0) {
			OnAllPiecesDestroyed ();
		}
	}

	protected virtual void OnAllPiecesDestroyed() {

	}




	#endregion






}

public enum BoardState {
	Disabled,
	Idle,
	Swapping,
	Matching,
	Destroying,
	Falling,
}

[System.Serializable]
public class LevelData {
	public int width;
	public int height;

	public int totalPieces {
		get {
			return width * height;
		}
	}

	public TextAsset definition;
	public int[,] colors { get; protected set; }
	public void ParseDefinition() {
		string txt = definition.text;
		string[] lines = txt.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
		for (int y = lines.Length - 1; y >= 0; y--) {
			string[] line = lines [lines.Length - y - 1].Split (new char[] { ',' });
			if (colors == null) {
				colors = new int[line.Length, lines.Length];
			}
			for (int x = 0; x < line.Length; x++) {
				colors [x, y] = int.Parse (line [x]);
			}
		}
		width = colors.GetLength (0);
		height = colors.GetLength (1);
	}
}

