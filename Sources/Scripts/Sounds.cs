using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sounds : MonoBehaviour {

    public GameObject volumeSonSlider;
    public GameObject volumeMusiqueSlider;
    private float volumeSon = 0.5f;
    private float volumeMusique = 0.5f; 

    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        volumeMusiqueSlider.GetComponent<Scrollbar>().value = PlayerPrefs.GetFloat("VolumeMusique");
        volumeSonSlider.GetComponent<Scrollbar>().value = PlayerPrefs.GetFloat("VolumeSon");
    }

    // Update is called once per frame
    void Update () {

    }

    public void OnValueChanged () {
        volumeSon = volumeSonSlider.GetComponent<Scrollbar>().value;
        volumeMusique = volumeMusiqueSlider.GetComponent<Scrollbar>().value;
        PlayerPrefs.SetFloat("VolumeSon", volumeSon);
        PlayerPrefs.SetFloat("VolumeMusique", volumeMusique);
        gameManager.soundChange();
    }

}