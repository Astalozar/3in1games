using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSettings : MonoBehaviour {
	protected static AnimationSettings _instance;
	protected static AnimationSettings instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<AnimationSettings> ();
				if (_instance == null) {
					_instance = new GameObject ("AnimationSettings").AddComponent<AnimationSettings> ();
				}
			}
			return _instance;
		}
	}


	public float moveSpeed = 5;
	public static float MoveSpeed {
		get {
			return instance.moveSpeed * TimeScale;
		}
	}

	public float fallSpeed = 5; 
	public static float FallSpeed {
		get {
			return instance.fallSpeed * TimeScale;
		}
	}

	public float destroyTime = 0.2f;
	public static float DestroyTime {
		get {
			return instance.destroyTime / TimeScale;
		}
	}

	public float timeScale = 1f;
	public static float TimeScale {
		get {
			return instance.timeScale;
		}
	}
}
