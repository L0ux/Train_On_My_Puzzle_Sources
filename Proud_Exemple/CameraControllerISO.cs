using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraControllerISO : MonoBehaviour
{   
    public float speed = 10;
    public GameObject focusObject;
    public float minimumZoom = 3f;
    public float baseZoom;
    public AudioClip level1Song;
    public Image cadena;

    private new AudioSource audio; 

    private Vector3 targetPosition;
    private new Camera camera;
    private Vector3 direction;
    private Vector3 mouseDirection;
    private float zoom;
    private Vector3 offSet;
    private bool isLocked = true;
    private bool itsWasLocked = false;
    private bool puzzleMod = false;
    private bool end = false;
    private bool start = true;

    // Start is called before the first frame update
    void Start()
    {   
        playAudioLevel();
        camera = this.GetComponent<Camera>(); 
        offSet = targetPosition - transform.position;
        camera.orthographicSize = baseZoom;
        StartCoroutine(focusFinalBloc());
    }

    // Update is called once per frame
    void Update()
    {       
        if(start){
            return;
        }

        if( !end  ){
            if( isLocked ){
                LockPosition();
            }else{
                checkBounds();
                transform.position = Vector3.Lerp(transform.position,transform.position+direction+mouseDirection,camera.orthographicSize/speed*2);
            }
            if( !((camera.orthographicSize + zoom) < minimumZoom) ){
                camera.orthographicSize += zoom;
            }   
        }else{
            LockPosition();
        }
    }

    //Appuie sur la touche Move
    public void OnMove(InputValue val){
        direction = new Vector3(val.Get<Vector2>().x*0.7f,val.Get<Vector2>().y,val.Get<Vector2>().x*0.7f);
    }

    //Centre la caméra sur l'objet focus
    private void LockPosition(){
        targetPosition = focusObject.GetComponent<Transform>().position;
        Vector3 desiredPosition = targetPosition - offSet;
        Vector3 smoothPosition = Vector3.Lerp(transform.position,desiredPosition,0.125f);
        transform.position = smoothPosition;
    }

    //Appuie sur la touche Lock
    public void OnLock(){
        if( puzzleMod ){
            puzzleOff();
        }else{
            isLocked = !isLocked;
        }
        updateCadenaIcone();
    }

    //Appuie sur la touche Zoom
    public void OnZoom(InputValue val){
        zoom = val.Get<float>() * Time.deltaTime * speed * 2;
    }

    public Vector3 getfocusObjectPosition(){
        return focusObject.GetComponent<Transform>().position;
    }

    public void puzzleOn(){
        itsWasLocked = isLocked;
        isLocked = false;
        puzzleMod = true;
        updateCadenaIcone();
    }

    public void puzzleOff(){
        isLocked = itsWasLocked;
        puzzleMod = false;
        updateCadenaIcone();
    }

    public void outOfScreen(Vector2 dir){
        mouseDirection = new Vector3(dir.x,0,dir.y);
    }

    private void playAudioLevel(){
        string sceneName = SceneManager.GetActiveScene().name;
        if( sceneName == "Niveau1" ){
            GetComponent<AudioSource>().clip = level1Song;
            GetComponent<AudioSource>().Play();
        }
    }

    public void endGame(){
        end = true;
    }

    IEnumerator focusFinalBloc(){
        GameObject[] blocs = GameObject.FindGameObjectsWithTag("RailBloc");
        Vector3 desiredPosition = new Vector3(0f,0f,0f);
        foreach(GameObject bloc in blocs ){
            if( bloc.GetComponent<Puzzle>().lastBloc ){
                desiredPosition = bloc.transform.position - offSet;
                break;
            }
        }
        yield return new WaitForSeconds(1f);
        isLocked = false;
        while( Vector3.Distance(transform.position,desiredPosition) > 2  ){
            transform.position = Vector3.Lerp(transform.position,desiredPosition,speed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(2f);
        isLocked = true;
        start = false;
        focusObject.GetComponentInParent<Train>().startTrain();
    }

    private void checkBounds(){
        if( Mouse.current.position.ReadValue().x > Screen.width-3 ){
                outOfScreen(new Vector2(1f,1f));
            }else if( Mouse.current.position.ReadValue().y > Screen.height-3 ){
                outOfScreen(new Vector2(-1.8f,1.8f));
            }else if( Mouse.current.position.ReadValue().x < 3 ){
                outOfScreen(new Vector2(-1f,-1f));
            }else if( Mouse.current.position.ReadValue().y < 3 ){
                outOfScreen(new Vector2(1.8f,-1.8f));
            }else{
                outOfScreen(new Vector2(0f,0f));
            }
    }

    private void updateCadenaIcone(){
        cadena.enabled = isLocked;
    }
}
