using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balls {
	public class BallsMatchFinder : MatchFinder {

		public override void FindMatches ()
		{
			matches = new List<Match> ();
			//Horizontal
			Match activeMatch = null;
			for (int y = 0; y < board.Height; y++) {
				activeMatch = null;
				for(int x = 0; x < board.Width; x++) {
					var p = board.GetPiece (x, y);
					if (activeMatch != null) {
						if (p.color != activeMatch.color) {
							if (activeMatch.Size >= minMatchSize) {
								matches.Add (activeMatch);
							}
							activeMatch = new Match (p.color);
							activeMatch.Add (new Vector2Int (x, y));
						} else {
							activeMatch.Add (new Vector2Int (x, y));
						}
					} else {
						activeMatch = new Match (p.color);
						activeMatch.Add (new Vector2Int (x, y));
					}
				}

				if (activeMatch.Size >= minMatchSize) {
					matches.Add (activeMatch);
				}
			}

			//Vertical
			for (int x = 0; x < board.Width; x++) {
				activeMatch = null;
				for(int y = 0; y < board.Height; y++) {
					var p = board.GetPiece (x, y);
					if (activeMatch != null) {
						if (p.color != activeMatch.color) {
							if (activeMatch.Size >= minMatchSize) {
								matches.Add (activeMatch);
							}
							activeMatch = new Match (p.color);
							activeMatch.Add (new Vector2Int (x, y));
						} else {
							activeMatch.Add (new Vector2Int (x, y));
						}
					} else {
						activeMatch = new Match (p.color);
						activeMatch.Add (new Vector2Int (x, y));
					}
				}

				if (activeMatch.Size >= minMatchSize) {
					matches.Add (activeMatch);
				}
			}
		}
	}
}