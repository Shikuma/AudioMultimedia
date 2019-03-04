using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
	// Width and height of the texture in pixels.
	public int pixWidth = 256;
	public int pixHeight = 256;
	public float scale = 20f, delay;

	// The origin of the sampled area in the plane.
	public float xOrg;
	public float yOrg;
	public float incValue;

	public bool autoUpdate, roundColorValues, generateAudio;

	public GameObject audioPrefab;	

	private Texture2D noiseTex;
	private Color[] pix;
	private Renderer rend;

	float[,] pixels;
	float sinusAudioWaveIntensity = 0.0f;
	float squareAudioWaveIntensity = 0.0f;
	float sawAudioWaveIntensity = 0.0f;
	ProceduralAudioController PAC;
	int i = 0;
	float t = 0.0f, pixelDelay;

	public enum filters {
		bilinear,
		point,
		trilinear
	}

	public filters texFilter;

	void Start() {
		PAC = audioPrefab.GetComponent<ProceduralAudioController>();
		pixelDelay = delay;
		InitSetup();
		//StartCoroutine(Simulate());
	}

	void Update() {
		CalcNoise();
		if (generateAudio) {
			sinusAudioWaveIntensity = Mathf.Lerp(pixels[noiseTex.width - 1, i], pixels[noiseTex.width - 1, i + 1], t);
			squareAudioWaveIntensity = Mathf.Lerp(pixels[noiseTex.width - 2, i], pixels[noiseTex.width - 2, i + 1], t);
			sawAudioWaveIntensity = Mathf.Lerp(pixels[noiseTex.width - 3, i], pixels[noiseTex.width - 3, i + 1], t);
			

			if (t < 1) {
				t += Time.deltaTime / pixelDelay;
			}
			else {
				if (i + 2 >= noiseTex.height) {
					xOrg += incValue;
					i = 0;
				}
				t = 0.0f;
				i++;
			}

			Debug.DrawRay(new Vector3(15.5f, 0, 15.5f - i), Vector3.up, Color.red);
			Debug.DrawRay(new Vector3(14.5f, 0, 15.5f - i), Vector3.up, Color.blue);
			Debug.DrawRay(new Vector3(13.5f, 0, 15.5f - i), Vector3.up, Color.yellow);


			PAC.sinusAudioWaveIntensity = sinusAudioWaveIntensity;
			PAC.squareAudioWaveIntensity = squareAudioWaveIntensity;
			PAC.sawAudioWaveIntensity = sawAudioWaveIntensity;
		}
	}

	public void InitSetup() {
		rend = GetComponent<Renderer>();

		// Set up the texture and a Color array to hold pixels during processing.
		noiseTex = new Texture2D(pixWidth, pixHeight);
		pix = new Color[noiseTex.width * noiseTex.height];
		pixels = new float[noiseTex.width, noiseTex.height];

		//Set the filter mode for the texture in case we want a different filter mode
		switch (texFilter) {
			case filters.bilinear:
				noiseTex.filterMode = FilterMode.Bilinear;
				break;
			case filters.point:
				noiseTex.filterMode = FilterMode.Point;
				break;
			case filters.trilinear:
				noiseTex.filterMode = FilterMode.Trilinear;
				break;
			default:
				break;
		}

		rend.sharedMaterial.mainTexture = noiseTex;
	}

	public void CalcNoise() {

		if (autoUpdate) {
			InitSetup();
		}

		// For each pixel in the texture...
		float y = 0.0F;

		while (y < noiseTex.height) {
			float x = 0.0F;
			while (x < noiseTex.width) {
				float xCoord = xOrg + x / noiseTex.width * scale;
				float yCoord = yOrg + y / noiseTex.height * scale;
				float sample = Mathf.PerlinNoise(xCoord, yCoord);
				if (roundColorValues) {
					if (sample > .5f) sample = 1;
					else sample = 0;
				}
				pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
				pixels[(int)x, (int)y] = sample;
				x++;
			}
			y++;
		}

		// Copy the pixel data to the texture and load it into the GPU.
		noiseTex.SetPixels(pix);
		noiseTex.Apply();
	}

	public IEnumerator Simulate() {
		yield return new WaitForSeconds(delay);
		xOrg += incValue;
		//yOrg += incValue;
		if (generateAudio) 
			//UpdateMusic();
		StartCoroutine(Simulate());
	}

	public void UpdateMusic() {
		float[] column1 = new float[noiseTex.height];
		float[] column2 = new float[noiseTex.height];
		float[] column3 = new float[noiseTex.height];
		for (int i = 0; i < noiseTex.height; i++) {
			column1[i] = pixels[noiseTex.width - 1, i];
			column2[i] = pixels[noiseTex.width - 2, i];
			column3[i] = pixels[noiseTex.width - 3, i];
		}

		

		
		float sinusAudioWaveIntensity = 0.0f;
		float squareAudioWaveIntensity = 0.0f;
		float sawAudioWaveIntensity = 0.0f;


		/*Exp 1
		 * Divded the total number of notes from the taken column by 3
		 * There are 3 waves we are using to adjust the tone of the audio
		 * 32x32 will leave 2 remainder -- right now those do nothing.
		for (int i = 0; i < 3; i++) {
			for (int j = 0 + ((noiseTex.height / 3) * i); j < (noiseTex.height / 3) + (noiseTex.height / 3) * i; j++) {
				if(i == 0) 
					sinusAudioWaveIntensity += thisColumn[j];
					
				else if(i == 1) 
					squareAudioWaveIntensity += thisColumn[j];
				
				else if(i == 2) 
					sawAudioWaveIntensity += thisColumn[j];
			}
		}

		sinusAudioWaveIntensity /= (noiseTex.height / 3);
		squareAudioWaveIntensity /= (noiseTex.height / 3);
		sawAudioWaveIntensity /= (noiseTex.height / 3);
		*/

		/*Exp 2
		 * Set the wave values directly from the first 3 pixel values
		
		sinusAudioWaveIntensity = thisColumn[0];
		squareAudioWaveIntensity = thisColumn[1];
		sawAudioWaveIntensity = thisColumn[2];
		
		 */

		/* Exp 3
		 * Get the last 3 columns
		 * 1 column = 1 wave
		 * set delay to 10 seconds
		 * Lerp through each column at a speed of (delay / noiseTex.height) <- Speed per pixel comes to a total of 10 seconds after each pixel has been lerped through
		 * Offset by offset*3
		 */

		Debug.Log(sinusAudioWaveIntensity);
		Debug.Log(squareAudioWaveIntensity);
		Debug.Log(sawAudioWaveIntensity);

		PAC.sinusAudioWaveIntensity = sinusAudioWaveIntensity;
		PAC.squareAudioWaveIntensity = squareAudioWaveIntensity;
		PAC.sawAudioWaveIntensity = sawAudioWaveIntensity;
	}
	
}
