using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DifferentSplash : MonoBehaviour {
    public Image Overlay;
    public string NextScene;
    public float ShowTime = 1;

    IEnumerator SplashRun() {
        yield return new WaitForSeconds(.1f);
        float start = Time.time;
        while (Time.time - start <= .5) {
            Color c = Overlay.color;
            c.a = 1-(Time.time - start) / .5f;
            Overlay.color = c;
            yield return null;
        }

        Color c1 = Overlay.color;
        c1.a = 0;
        Overlay.color = c1;

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
    void Start() {
        StartCoroutine(SplashRun());
    }

    // Update is called once per frame
    void Update() {

    }
}
