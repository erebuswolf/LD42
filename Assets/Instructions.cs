using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour {

    [SerializeField] private AudioSource paperNoise;


    // Use this for initialization
    void Start () {
    }

    public void ShowSheet() {
        paperNoise.Play();
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
        if (Input.anyKey) {
            paperNoise.Play();
            this.gameObject.SetActive(false);
        }
    }
}
