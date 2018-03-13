using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour {

	public static float size = 1f;
	public SpriteRenderer spriteRenderer;
	public int color;

	public virtual void Clear() {
		transform.DOKill ();
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
	}

	protected PieceState _state;
	public PieceState state {
		get {
			return _state;
		}
		protected set {
			_state = value;
		}
	}


	public virtual void Init(int color) {
		this.color = color;
		spriteRenderer.color = ColorSettings.GetColor (color);
		this.state = PieceState.Idle;
	}


	protected Coroutine movementCoroutine;

	public virtual void MoveTo(Vector3 destination, System.Action<Piece> onComplete, float speed) {
		if (movementCoroutine != null) {
			StopCoroutine (movementCoroutine);
		}
		movementCoroutine = StartCoroutine (MoveToCoroutine (destination, onComplete, speed));
	}

	protected virtual IEnumerator MoveToCoroutine(Vector3 destination, System.Action<Piece> onComplete, float speed, float delay = 0, float acceleration = 0) { 
		if (delay > 0) {
			yield return new WaitForSeconds (delay);
		}

		bool didArrive = false;
		while (!didArrive) {
			Vector3 newPosition = Vector3.MoveTowards (transform.localPosition, destination, Time.deltaTime * speed);
			transform.localPosition = newPosition;
			speed += acceleration * Time.deltaTime;

			didArrive = Vector3.Distance (transform.localPosition, destination) < minDistance;
			if (!didArrive) {
				yield return new WaitForEndOfFrame ();
			}
		}
		transform.localPosition = destination;
		state = PieceState.Idle;
		onComplete.Invoke (this);
	}


	public virtual void Release(System.Action<Piece> onComplete, float delay = 0) {
		if (state == PieceState.Idle) {
			StartCoroutine (DestroyCoroutine (onComplete, delay));
		} else {
			onComplete (this);
		}
	}

	protected virtual IEnumerator DestroyCoroutine(System.Action<Piece> onComplete, float delay = 0) {
		if (delay > 0) {
			yield return new WaitForSeconds (delay);
		}

		state = PieceState.Destroyed;
		transform.DOScale(Vector3.one * 0.5f, AnimationSettings.DestroyTime).SetEase(Ease.InBack);

		yield return new WaitForSeconds (AnimationSettings.DestroyTime * destructionTimePercentage);

		onComplete.Invoke (this);

		yield return new WaitForSeconds (AnimationSettings.DestroyTime * (1 - destructionTimePercentage));

		PiecePool.DestroyPiece (this);
	}

	protected static float minDistance = 0.0001f;
	protected static float destructionTimePercentage = 0.8f;
}

public enum PieceState {
	Idle,
	Swapping,
	Matching,
	Destroying,
	Destroyed,
	Falling,
}