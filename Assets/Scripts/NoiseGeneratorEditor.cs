using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects, CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		NoiseGenerator noiseGen = (NoiseGenerator)target;

		/*
		// Width and height of the texture in pixels.
		int pixWidth = EditorGUILayout.IntField("Pixel Width", noiseGen.pixWidth);
		int pixHeight = EditorGUILayout.IntField("Pixel Height", noiseGen.pixWidth);
		float scale = EditorGUILayout.FloatField("Scale", noiseGen.scale);

		// The origin of the sampled area in the plane.
		float xOrg = EditorGUILayout.FloatField("X Offset", noiseGen.xOrg);
		float yOrg = EditorGUILayout.FloatField("Y Offset", noiseGen.yOrg); 

		bool autoUpdate = EditorGUILayout.Toggle("Auto Update", noiseGen.autoUpdate); ;
		*/
		if (noiseGen.autoUpdate)
			noiseGen.CalcNoise();
		if (GUILayout.Button("Generate")) {
			noiseGen.InitSetup();
			noiseGen.CalcNoise();
		}
	}
}
/*
public class NoiseGeneratorEditor : MonoBehaviour {

}
*/
