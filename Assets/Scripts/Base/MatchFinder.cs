using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder {
	protected static int minMatchSize = 3;

	public Board board;
	public List<Match> matches { get; protected set; }

	public virtual void FindMatches() {
		matches = new List<Match> ();

		board.OnMatchesFound ();
	}

}


public class Match {


	public Match(int color) {
		this.coordinates = new List<Vector2Int> ();
		this.color = color;
	}

	public List<Vector2Int> coordinates;
	public int color { get; protected set; }

	public void Add(Vector2Int newCoordinates) {
//		if (coordinates == null) {
//			coordinates = new List<Vector2Int> ();
//		}
		if (!coordinates.Contains (newCoordinates)) {
			coordinates.Add (newCoordinates);
		}
	}

	public void Merge(Match target) {
		for (int i = 0; i < target.coordinates.Count; i++) {
			if (!coordinates.Contains (target.coordinates [i])) {
				coordinates.Add (target.coordinates [i]);
			}
		}
		target.coordinates.Clear ();
	}

	public int Size {
		get {
			return coordinates.Count;
		}
	}


}