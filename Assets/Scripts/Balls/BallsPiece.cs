using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balls {
	public class BallsPiece : Piece {

		protected new static float destructionTimePercentage = 2f;
		 
		public override void Init (int color)
		{
			base.Init (color);
		}
	}
}
