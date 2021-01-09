using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   

    public int nbLevel = 3;

    public AudioSource musiqueSource;
    public AudioSource[] trainSource;
    public GameObject canvasPause;

    private GameObject train;
    private List<AudioSource> puzzleSources;
    private CameraControllerISO cameraIso;
    private bool inGame = false;

    [HideInInspector]
    public bool pause = false;
    [HideInInspector]
    public bool end = false;

    // Start is called before the first frame update
    void Start()
    {
        if( !(SceneManager.GetActiveScene().name == "Menu") ){
            cameraIso = GameObject.Find("CameraISO").GetComponent<CameraControllerISO>();
            train = GameObject.Find("Train");
            gameDifficulty();
            puzzleSources = new List<AudioSource>();
            GameObject[] puzzles = GameObject.FindGameObjectsWithTag("RailBloc");
            foreach(GameObject puzzle in puzzles){
                if( !puzzle.GetComponent<Puzzle>().isPlaced ){
                    puzzleSources.Add(puzzle.GetComponent<AudioSource>());
                }
            }
            inGame =  true;
        }
        soundChange();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void soundChange(){
        float audioVolume = 1;
        if(inGame){
            audioVolume = PlayerPrefs.GetFloat("VolumeSon",1);
            foreach(AudioSource puzzleSource in puzzleSources){
                puzzleSource.volume = audioVolume;
            }
            foreach(AudioSource audio in trainSource ){
                audio.volume = PlayerPrefs.GetFloat("VolumeSon",1);
            }
        }
        audioVolume = PlayerPrefs.GetFloat("VolumeMusique",1);
        musiqueSource.volume = audioVolume;
    }

    public void OnPause(){
        if( !end ){
                pause = !pause;
            if( pause ){
                canvasPause.SetActive(true);
            }else
            {
                canvasPause.GetComponent<MenuPause>().defautMenu();        
            }
            train.GetComponent<Train>().playPause(pause);
        }   
    }

    public void endGame(){
        cameraIso.endGame();
        end = true;
        canvasPause.GetComponent<MenuPause>().defautMenu();  
        pause =  false;
    }

    public void reloadLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void nextLevel(){
        int levelScene =  SceneManager.GetActiveScene().buildIndex;
        levelScene += 1;
        if( levelScene > nbLevel || SceneManager.sceneCountInBuildSettings < (levelScene+1) ){
            levelScene = 0;
        }

        SceneManager.LoadScene(levelScene);
    }

    private void gameDifficulty(){
        int difficulty = PlayerPrefs.GetInt("difficulte",1);
        switch (difficulty)
        {
            case 0:
                train.GetComponent<Train>().speed = 5;
                break;
            case 1:
                train.GetComponent<Train>().speed = 10;
                break;
            case 2:
                train.GetComponent<Train>().speed = 18;
                break;
          default:
              train.GetComponent<Train>().speed = 10;
              break;
        }
    }
}
