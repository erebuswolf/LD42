using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {
    public Image Logo;
    public Image Overlay;
    public string NextScene;
    public float ShowTime = 1;
    IEnumerator SplashRun() {
        yield return new WaitForSeconds(.1f);
        float start = Time.time;
        while (Time.time - start <= .5) {
            Color c = Logo.color;
            c.a = (Time.time - start) / .5f;
            Logo.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(ShowTime);
        start = Time.time;
        while (Time.time - start <= .5) {
            Color c = Overlay.color;
            c.a = (Time.time - start) / .5f;
            Overlay.color = c;
            yield return null;
        }
        Color c2 = Overlay.color;
        c2.a = 1;
        Overlay.color = c2;
        yield return null;
        SceneManager.LoadScene(NextScene);
    }
	// Use this for initialization
	void Start () {
        Color c = Logo.color;
        c.a = 0;
        Logo.color = c;
        StartCoroutine(SplashRun());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
