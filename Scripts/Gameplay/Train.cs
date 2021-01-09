using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{   

    public static int nbPoints = 13;

    private GameObject[] wayPoint;
    
    private int currentWayPoint = 0;
    private GameObject[] blocs;
    private GameObject currentBlock = null;
    private List<GameObject> visitedBlocs;
    private bool lastBloc = false;
    private Vector3 position;
    private bool end = false;
    private bool pause = false;
    private bool start = false;
    private Vector3 direction = new Vector3(0f,0f,0f);
    private float startSpeed = 0f;
    private GameManager gameManager;
    
    public  AudioSource engineAudio;
    public  AudioSource fxAudio;
    
    public GameObject train;
    public GameObject firstBloc;
    public float speed = 1.0f;

    public ParticleSystem[] brakeParticles;
    public ParticleSystem whiteSteam;
    public ParticleSystem blackSteam;
    public AudioClip horn;
    public AudioClip steamEngine;
    public AudioClip brakeSong;
    public GameObject win;
    public GameObject lose;
    public MenuPause menuPauseScript;

    // Start is called before the first frame update
    void Start()
    {   
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        brakeParticlePlay(false);
        whiteSteam.Stop();
        blackSteam.Stop();
        blocs = GameObject.FindGameObjectsWithTag("RailBloc");
        visitedBlocs = new List<GameObject>();
        wayPoint = new GameObject[nbPoints];
        position = train.transform.position;
        currentBlock = firstBloc;
        getwayPoint();
        currentWayPoint = 8;
    }

    // Update is called once per frame
    void Update()
    {     

        if(!start){
            return;
        }

        if( !end && !pause ){

            position = train.transform.position;

            if( startSpeed < speed ){
                startSpeed = startSpeed + speed / 100f;
                if( engineAudio.pitch < 1){
                    engineAudio.pitch += 0.001f;
                }
            }

            steamEffect();

            if( currentBlock != null ){
                direction = Vector3.Normalize(wayPoint[currentWayPoint].transform.position-position);
                train.transform.position = Vector3.MoveTowards(position,wayPoint[currentWayPoint].transform.position,Time.deltaTime * startSpeed);
                position = train.transform.position;
                train.transform.rotation = Quaternion.LookRotation(direction);
                if( Vector3.Distance(position,wayPoint[currentWayPoint].transform.position) < 1){
                    currentWayPoint++;
                }
            }

            if( currentWayPoint == nbPoints && !(visitedBlocs.Count == blocs.Length)){
                getCurrentBloc();
            }


            //Crash du train
            if( currentBlock == null && !lastBloc){
                fxAudio.clip = brakeSong;
                fxAudio.Play();
                brakeParticlePlay(true);
                StartCoroutine("crashAnimation");
                end = true;
                gameManager.endGame();
            }

            //Fin du circuit
            if(lastBloc){
                end = true;
                StartCoroutine("endAnimation");
                gameManager.endGame();
            }
        }
    }

    void getwayPoint(){
        GameObject points = currentBlock.transform.GetChild(0).GetChild(0).gameObject;
        Vector3 firstPosition = points.transform.GetChild(0).gameObject.transform.position;
        Vector3 lastPosition = points.transform.GetChild(nbPoints-1).gameObject.transform.position;
        float distanceFirst = Vector3.Distance(position,firstPosition);
        float distanceLast = Vector3.Distance(position,lastPosition);

        if( distanceFirst < distanceLast ){
            for(int i = 0; i < nbPoints; i++){
                wayPoint[i] = points.transform.GetChild(i).gameObject;
            }
        }else{
            int j = nbPoints-1;
            for(int i = 0; i < nbPoints; i++){
                wayPoint[i] = points.transform.GetChild(j).gameObject;
                j--;
            }
        }
        currentWayPoint = 0;
    }

    void getCurrentBloc(){
        if(currentBlock != null ){
            visitedBlocs.Add(currentBlock);
        }  

        currentBlock = null;

        float distance = 10000;
        float tmpDistance = 0;

        foreach(GameObject bloc in blocs){
            tmpDistance = Vector3.Distance(firstPointOfBloc(bloc),position);
            if( tmpDistance < distance && !visitedBlocs.Contains(bloc) && tmpDistance < 20 && bloc.GetComponent<Puzzle>().isPlaced){
                distance = tmpDistance;
                currentBlock = bloc;
            }
        }

        if( currentBlock != null ){
            currentBlock.GetComponent<Puzzle>().trainPassThrow();
            getwayPoint();
            lastBloc = currentBlock.GetComponent<Puzzle>().isTheLast();
        }

    }

    Vector3 firstPointOfBloc(GameObject bloc){
        GameObject points = bloc.transform.GetChild(0).GetChild(0).gameObject;
        Vector3 firstPosition = points.transform.GetChild(0).gameObject.transform.position;
        Vector3 lastPosition = points.transform.GetChild(nbPoints-1).gameObject.transform.position;

        if( Vector3.Distance(position,firstPosition) < Vector3.Distance(position,lastPosition) ){
            return firstPosition;
        }else{
            return lastPosition;
        }

    }

    IEnumerator crashAnimation(){
        Vector3 position = train.transform.position;
        while( Vector3.Distance(position,train.transform.position) < 10 && speed > 0.5 ){
            train.transform.position = Vector3.MoveTowards(train.transform.position,train.transform.position+direction,Time.deltaTime * speed);
            speed = speed - ( speed*2 / 100f );
            engineAudio.volume = engineAudio.volume - 0.01f;
            yield return new WaitForSeconds(0.01f);   
        }
        engineAudio.Stop();
        brakeParticlePlay(false);
        blackSteam.Stop();
        whiteSteam.Play();
        menuPauseScript.ending(lose);
    }

    IEnumerator endAnimation(){
        while( currentWayPoint < 2 ){
            direction = Vector3.Normalize(wayPoint[currentWayPoint].transform.position-position);
            train.transform.position = Vector3.MoveTowards(position,wayPoint[currentWayPoint].transform.position,Time.deltaTime * startSpeed);
            position = train.transform.position;
            train.transform.rotation = Quaternion.LookRotation(direction);
            if( Vector3.Distance(position,wayPoint[currentWayPoint].transform.position) < 1){
                currentWayPoint++;
            }
            yield return new WaitForSeconds(0.01f); 
        }
        position = train.transform.position;
        while( Vector3.Distance(position,train.transform.position) < 20 ){
            train.transform.position = Vector3.MoveTowards(train.transform.position,train.transform.position+direction,Time.deltaTime * speed);
            if( speed > 3 ){
                speed = speed - ( (speed) / 100f );
                engineAudio.pitch = engineAudio.pitch - 0.001f;
                engineAudio.volume = engineAudio.volume - 0.001f;
            } 
            yield return new WaitForSeconds(0.01f);
        }
        while( engineAudio.volume > 0 ){
            engineAudio.volume = engineAudio.volume - 0.005f;
            yield return new WaitForSeconds(0.01f); 
        }
        engineAudio.Stop();
        menuPauseScript.ending(win);
    }

    private void brakeParticlePlay(bool b){
        if(b){
            foreach(ParticleSystem brake in brakeParticles){
                brake.Play();
            }
        }else{
            foreach(ParticleSystem brake in brakeParticles){
                brake.Stop();
            }
        }
    }

    private void steamEffect(){
        float random = Random.Range(0f,3000f);
        if( random < 1 ){
            whiteSteam.Stop();
            blackSteam.Play();
            fxAudio.clip = horn;
            fxAudio.Play();
        }
        if(  !whiteSteam.isEmitting && !blackSteam.isEmitting){
            whiteSteam.Play();
        }

    }

    public void playPause(bool p){
        pause = p;
        if( pause ){
            Time.timeScale = 0;
            engineAudio.Stop();
        }else{
            Time.timeScale = 1;
            engineAudio.Play();
        }
    }

    public void startTrain(){
        engineAudio.clip = steamEngine;
        engineAudio.Play();
        engineAudio.pitch = 0.8f;
        start = true;
    }   

}
