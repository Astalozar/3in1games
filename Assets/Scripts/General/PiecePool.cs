using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PiecePool : MonoBehaviour {

	protected static PiecePool instance;
	protected static PiecePool Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<PiecePool> ();
			}
			return instance;
		}
	}

	protected static int uniqueId = 0;


	protected List<Piece> activePieces = new List<Piece> ();
	protected List<Piece> disabledPieces = new List<Piece> ();

	public GameObject piecePrefab;


	protected virtual Piece CreatePieceInstance() {
		Piece piece = null;
		if (disabledPieces.Count > 0) {
			piece = disabledPieces [0];
			disabledPieces.RemoveAt (0);
			piece.gameObject.SetActive (true);
		} else {
			piece = (Instantiate (Instance.piecePrefab) as GameObject).GetComponent<Piece> ();
			piece.name = "Piece " + uniqueId++;
		}

		activePieces.Add (piece);
		return piece;
	}

	protected virtual void DestroyPieceInstance(Piece piece) {
		piece.Clear ();
		if (Instance.activePieces.Contains (piece)) {
			Instance.activePieces.Remove (piece);
		}
		Instance.disabledPieces.Add (piece);
		piece.transform.SetParent (Instance.transform);

		piece.gameObject.SetActive (false);
	}

	public static Piece CreatePiece() {
		return Instance.CreatePieceInstance ();
	}

	public static void DestroyPiece(Piece piece) {
		Instance.DestroyPieceInstance (piece);
	}
}

