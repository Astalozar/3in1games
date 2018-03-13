using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
	public Camera mainCamera;
	public TouchInputEvent onTouch = delegate{};
	public SwipeInputEvent onSwipe = delegate {};

	protected Vector3 startTouchPosition;
	protected float touchStartTime;

	void Update() {

		if (Input.GetMouseButtonDown (0)) {
			OnTouchStarted ();
		} else {
			if (Input.GetMouseButton (0)) {
				OnTouchMoved();
			} else {
				if(Input.GetMouseButtonUp(0)) {
					OnTouchEnded ();
				}
			}
		}
	}


	protected void OnTouchStarted() {
		Vector3 worldCoordinates = GetWorldCoordinates (Input.mousePosition);
		onTouch.Invoke (worldCoordinates, TouchPhase.Began);

		startTouchPosition = worldCoordinates;
		touchStartTime = Time.time;
		didSwipe = false;
	}

	protected void OnTouchMoved() {
		if (!didSwipe) {
			Vector3 worldCoordinates = GetWorldCoordinates (Input.mousePosition);
			onTouch.Invoke (worldCoordinates, TouchPhase.Moved);

			Vector3 delta = worldCoordinates - startTouchPosition;
			if (delta.magnitude >= swipeDistance && Time.time - touchStartTime <= swipeMaxTime) {
				Debug.Log ("Swipe");
				Swipe (worldCoordinates);
			}
		}
	}

	protected void OnTouchEnded() {
		if (!didSwipe) {
			Vector3 worldCoordinates = GetWorldCoordinates (Input.mousePosition);
			onTouch.Invoke (worldCoordinates, TouchPhase.Ended);

			Vector3 delta = worldCoordinates - startTouchPosition;
			if (delta.magnitude >= swipeDistance && Time.time - touchStartTime <= swipeMaxTime) {
				Debug.Log ("Swipe");
				Swipe (worldCoordinates);
			}
		}
	}

	protected void Swipe(Vector3 endPosition) {
		Vector3 direction = endPosition - startTouchPosition;
		Vector2Int dir = new Vector2Int ();
		if (Mathf.Abs (direction.x) > Mathf.Abs (direction.y)) {
			dir.y = 0;
			dir.x = direction.x > 0 ? 1 : -1;
		} else {
			dir.x = 0;
			dir.y = direction.y > 0 ? 1 : -1;
		}
		Debug.Log (direction);
		didSwipe = true;

		onSwipe.Invoke (startTouchPosition, dir);
	}

	protected Vector3 GetWorldCoordinates(Vector3 touchPosition) {
		touchPosition.z = -mainCamera.transform.position.z;
		return mainCamera.ScreenToWorldPoint (touchPosition);
	}

	protected bool didSwipe = false;
	protected static float swipeDistance = 1f;
	protected static float swipeMaxTime = 1f;
}

public delegate void TouchInputEvent(Vector3 touchPosition, TouchPhase status);
public delegate void SwipeInputEvent(Vector3 origin, Vector2Int normalizedDirection);
