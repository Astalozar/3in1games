using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balls {
public class BallsBoard : Board {

		public override void Init ()
		{
			matchFinder = new BallsMatchFinder ();
			matchFinder.board = this;
		}

		public override void StartGame ()
		{
			base.StartGame ();
			inputManager.onSwipe += OnSwipe;
			currentState = BoardState.Idle;
		}

		#region generation 

		public override void GenerateBoard (LevelData data)
		{
			base.GenerateBoard (data);
			for (int x = 0; x < data.width; x++) {
				for (int y = 0; y < data.height; y++) {
					var piece = PiecePool.CreatePiece ();
					piece.transform.SetParent (transform);
					piece.transform.localPosition = GetPiecePosition (new Vector2Int (x, y));
					
					if (levelData.definition != null) {
						piece.Init (levelData.colors [x, y]);
					} else {
						piece.Init (Random.Range (0, ColorSettings.colorCount));
					}
					
					pieces [x, y] = piece;
				}
			}
		}
		
		public override void ClearBoard ()
		{
			base.ClearBoard ();
			for (int x = 0; x < pieces.GetLength (0); x++) {
				for (int y = 0; y < pieces.GetLength (1); y++) {
					PiecePool.DestroyPiece (pieces [x, y]);
				}
			}
			pieces = new Piece[0, 0];
		}
		#endregion



		#region input

		public void OnSwipe(Vector3 position, Vector2Int normalizedDirection) {
			if (currentState == BoardState.Idle) {
				Vector2Int coordinates = GetCoordinates (position);
				if (HasPiece (coordinates) && HasPiece (coordinates + normalizedDirection)) {
//					Debug.Log ("Swap " + GetPiece (coordinates).name + " with " + GetPiece (coordinates + normalizedDirection).name);
					SwapPieces (coordinates, coordinates + normalizedDirection);
				}
			}
		}
		
		public override void OnTouch (Vector3 position, TouchPhase status)
		{
//			if (status == TouchPhase.Ended) {
//				
//			}

//			if (status == TouchPhase.Ended) {
//				if (Input.GetKey (KeyCode.LeftShift)) {
//					Vector2Int coordinates = GetCoordinates (position);
//					Debug.Log (coordinates + " " + GetPiece(coordinates));
//					if (GetPiece (coordinates) != null) {
//						Debug.Log (GetPiece (coordinates).state);
//					}
//				} else {
//					matchFinder.FindMatches ();
//				}
				//			Vector2Int coordinates = GetCoordinates (position);
				//			Debug.Log (coordinates);
				//			if (HasPiece (coordinates)) {
				////				mopiece.spriteRenderer.color = Color.white;
				////				mopiece.MoveTo (GetPiecePosition (coordinates), OnPieceArrived);	
				//				DestroyPieces(new Vector2Int[] {coordinates, coordinates + new Vector2Int(0, 0	), coordinates + new Vector2Int(0, 2)});
				//			}
//			}
		}
		#endregion



		#region matches 

		public override void OnMatchesFound ()
		{
			List<Vector2Int> coords = new List<Vector2Int> ();
			for (int i = 0; i < matchFinder.matches.Count; i++) {
				coords.AddRange (matchFinder.matches [i].coordinates);
			}
			DestroyPieces (coords.ToArray());
		}

		#endregion



		#region movement

		public override void OnPieceArrived (Piece piece)
		{
			base.OnPieceArrived (piece);
//			Debug.Log ("_MOVE " + piece.name);
		}

		public void SwapPieces(Vector2Int from, Vector2Int to) {
			var fromPiece = GetPiece (from);
			var toPiece = GetPiece (to);

			if (fromPiece != null && toPiece != null) {

				currentState = BoardState.Swapping;

				pieces [from.x, from.y] = toPiece;
				pieces [to.x, to.y] = fromPiece;

				piecesInProgress.Add (fromPiece);
				piecesInProgress.Add (toPiece);
				matchFinder.FindMatches ();
				Debug.Log (matchFinder.matches.Count);

				if (matchFinder.matches.Count == 0) {
					pieces [from.x, from.y] = fromPiece;
					pieces [to.x, to.y] = toPiece;

					fromPiece.MoveTo (GetPiecePosition (to), ((Piece obj) => {
						obj.MoveTo (GetPiecePosition (from), OnPieceArrived, AnimationSettings.MoveSpeed);
					}), AnimationSettings.MoveSpeed);

					toPiece.MoveTo (GetPiecePosition (from), ((Piece obj) => {
						obj.MoveTo (GetPiecePosition (to), OnPieceArrived, AnimationSettings.MoveSpeed);
					}), AnimationSettings.MoveSpeed);
				} else {
					fromPiece.MoveTo (GetPiecePosition (to), OnPieceArrived, AnimationSettings.MoveSpeed);
					toPiece.MoveTo (GetPiecePosition (from), OnPieceArrived, AnimationSettings.MoveSpeed);
				}
			}
		}

		public void DropPieces() {
			currentState = BoardState.Falling;

			piecesInProgress = new List<Piece> ();
			for (int x = 0; x < pieces.GetLength (0); x++) {
				int lowY = -1;
				int emptySpaces = 0;
				for (int y = 0; y < pieces.GetLength (1); y++) {
					var piece = GetPiece (x, y);
					if (piece == null) {
						pieces [x, y] = null;
						emptySpaces++;
						if (lowY == -1) {
							lowY = y;
						}
					} else {
						if(lowY >= 0 && lowY < y) {
							MovePiece (new Vector2Int (x, y), new Vector2Int (x, lowY), AnimationSettings.FallSpeed);
							pieces [x, lowY] = piece;
							pieces [x, y] = null;
							int l = 0;
							while (HasPiece (x, l)) {
								if (GetPiece (x, l) == null || GetPiece (x, l).state == PieceState.Destroyed) {
									lowY = l;
									break;
								}
								l++;
							}
						}
					}
				}

				Debug.LogWarning (emptySpaces);
				for(int y = Height; y < Height + emptySpaces; y++) {
					var piece = PiecePool.CreatePiece();
					Debug.Log ("Make piece " + piece.name);
					piece.transform.SetParent (transform);
					piece.transform.localPosition = GetPiecePosition (new Vector2Int (x, y));

					piece.Init(Random.Range(0, ColorSettings.colorCount));
					MovePiece (piece, new Vector2Int (x, y - emptySpaces), AnimationSettings.FallSpeed);
					pieces [x, y - emptySpaces] = piece;
				}
			}
		}

		protected override void OnAllPiecesArrived ()
		{
			Debug.Log ("All " + currentState);
			switch (currentState) {
			case BoardState.Swapping:
				if (matchFinder.matches.Count == 0) {
					currentState = BoardState.Idle;
				} else {
					OnMatchesFound ();
				}
				break;
			case BoardState.Falling:
				matchFinder.FindMatches ();
				if (matchFinder.matches.Count > 0) {
					OnMatchesFound ();
				} else {
					currentState = BoardState.Idle;
				}
				break;
			}
		} 


	#endregion


	#region destruction

		public override void DestroyPieces (Vector2Int[] coordinates)
		{
			if (coordinates.Length > 0) {
				currentState = BoardState.Destroying;
			} else {
				currentState = BoardState.Idle;
			}
			base.DestroyPieces (coordinates);
		}

		protected override void OnPieceDestroyed (Piece piece)
		{
			base.OnPieceDestroyed (piece);
			Debug.Log ("_DESTROY " + piece.name);
		}

	protected override void OnAllPiecesDestroyed ()
		{
			base.OnAllPiecesDestroyed ();
			for (int x = 0; x < Width; x++) {
				for (int y = 0; y < Height; y++) {
					if (GetPiece (x, y) != null && GetPiece (x, y).state == PieceState.Destroyed) {
						pieces [x, y] = null;
					}
				}
			}
			DropPieces ();
		}

	#endregion
	}
}