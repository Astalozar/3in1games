using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSettings : MonoBehaviour {

	protected static ColorSettings _instance;
	protected static ColorSettings instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<ColorSettings> ();
				if (_instance == null) {
					_instance = new GameObject ("ColorSettings").AddComponent<ColorSettings> ();
					_instance.colors = new Color[10];
					for (int i = 0; i < _instance.colors.Length; i++) {
						_instance.colors [i] = new Color (Random.Range (0, 1f), Random.Range (0, 1f), Random.Range (0, 1f));
					}
				}
			}
			return _instance;
		}
	}

	public static int colorCount {
		get {
			return instance.colors.Length;
		}
	}

	public Color[] colors;
	public Color noColor;

	public static Color GetColor(int index) {
		if (index >= 0 && index < instance.colors.Length) {
			return instance.colors [index];
		}
		return instance.noColor;
	}
}
