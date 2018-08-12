using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalsSheet : MonoBehaviour {

    [SerializeField] private List<GameObject> checkmarks;

    [SerializeField] private AudioSource CrayonNoise;
    [SerializeField] private AudioSource paperNoise;

    private bool[] checkedMarks;

    bool animating;

    bool inited;
    // Use this for initialization
    void Start () {
        init();
    }

    void init() {
        if (inited) {
            return;
        }
        inited = true;
        checkedMarks = new bool[checkmarks.Count];
    }

	// Update is called once per frame
	void Update () {
        if (Input.anyKey && !animating) {
            paperNoise.Play();
            this.gameObject.SetActive(false);
        }
	}

    public void ShowSheet() {
        paperNoise.Play();
        this.gameObject.SetActive(true);
    }

    public void SetGoal(int goal) {
        init();
        if (goal >=0 && goal < checkmarks.Count && !checkedMarks[goal]) {
            this.gameObject.SetActive(true);
            checkedMarks[goal] = true;
            paperNoise.Play();
            StartCoroutine(CheckBox(goal));
        }
    }
    
    public IEnumerator CheckBox(int i) {
        animating = true;
        yield return new WaitForSeconds(.3f);
        CrayonNoise.Play();
        yield return new WaitForSeconds(.2f);
        checkmarks[i].SetActive(true);
        animating = false;
    }
}
