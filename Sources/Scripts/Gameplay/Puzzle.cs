using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Puzzle : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{   

    public bool lastBloc = false;
    public bool isPlaced = true;
    public GameObject blocVide;
    public Material unplacedMat;
    public Material lastBlocMat;

    private new Camera camera;
    private Vector3 uiScale = new Vector3(2.5f,2.5f,2.5f);
    private Material placedMat;
    private bool changeMat = false;
    private new AudioSource audio;

    [HideInInspector]
    public Vector3 initialPosition = new Vector3(0f,0f,0f);

    private Inventaire inventaire; 
    private GameObject blocPlace;

    // Start is called before the first frame update
    void Start()
    {   
        audio =  GetComponent<AudioSource>();
        camera = GameObject.Find("CameraISO").GetComponent<Camera>();
        if( !isPlaced ){
            inventaire = GameObject.Find("Inventaire").GetComponent<Inventaire>();
            blocPlace = GameObject.Find("Blocs_Pose");
            changeMat = true;
            placedMat = this.transform.GetChild(1).transform.gameObject.GetComponent<Renderer>().material;
            this.transform.GetChild(1).transform.gameObject.GetComponent<Renderer>().material = unplacedMat;
        }else{
            this.GetComponent<BoxCollider>().enabled = false;
            this.GetComponent<AudioSource>().enabled = false;
            if( lastBloc ){
                this.transform.GetChild(1).transform.gameObject.GetComponent<Renderer>().material = lastBlocMat;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {   
        
    }

    public bool isTheLast(){
        return lastBloc;
    }

    public void OnPointerDown (PointerEventData eventData)
    {   
        if( !isPlaced ){
            //print("OnPointerDown "+this.name);
        }
    }

    public void OnBeginDrag (PointerEventData eventData)
    {       
        if( !isPlaced ){  
            inventaire.drag(transform.gameObject);
            camera.GetComponent<CameraControllerISO>().puzzleOn();
            transform.localScale = new Vector3(4f,4f,4f);
        }
    }

    public void OnDrag (PointerEventData eventData)
    {    
        Vector3 mousePosition = camera.ScreenToWorldPoint(new Vector3(eventData.position.x,eventData.position.y,0f));
        if( !isPlaced ){
            transform.position = camera.ScreenToWorldPoint(new Vector3(eventData.position.x,eventData.position.y,40));
            int layerMask = 1 << 9;
            RaycastHit hit;
            if( Physics.Raycast(mousePosition,camera.transform.forward,out hit, Mathf.Infinity,layerMask)) {
                snaped(hit.transform.position);
            }
            //Debug.DrawRay(transform.position, camera.transform.forward * 1000, Color.yellow);

            //Detection des bords de l'écran
            if( eventData.position.x > Screen.width-3 ){
                camera.GetComponent<CameraControllerISO>().outOfScreen(new Vector2(1f,1f));
            }else if( eventData.position.y > Screen.height-3 ){
                camera.GetComponent<CameraControllerISO>().outOfScreen(new Vector2(-1f,1f));
            }else if( eventData.position.x < 3 ){
                camera.GetComponent<CameraControllerISO>().outOfScreen(new Vector2(-1f,-1f));
            }else if( eventData.position.y < 3 ){
                camera.GetComponent<CameraControllerISO>().outOfScreen(new Vector2(1f,-1f));
            }else{
                camera.GetComponent<CameraControllerISO>().outOfScreen(new Vector2(0f,0f));
            }
        }
    }

    public void OnEndDrag (PointerEventData eventData)
    {   
        if( !isPlaced ){
            Vector3 mousePosition = camera.ScreenToWorldPoint(new Vector3(eventData.position.x,eventData.position.y,0f));
            int layerMask = 1 << 9;
            RaycastHit hit;
            if( Physics.Raycast(mousePosition,camera.transform.forward,out hit, Mathf.Infinity,layerMask) ) {
                droped(hit.transform.gameObject);
            }else{
                inventaire.unDrag(this.gameObject,false);
                camera.GetComponent<CameraControllerISO>().puzzleOff();
                transform.localScale = uiScale;
                transform.localPosition = initialPosition;
            }
        }
    }

    public void droped(GameObject obj){
        audio.pitch = 1f;
        audio.Play();
        Vector3 p = obj.transform.position;
        inventaire.unDrag(this.gameObject,true);
        isPlaced = true;
        transform.parent = blocPlace.transform;
        transform.position = new Vector3(p.x,0f,p.z);
        transform.localScale = new Vector3(1f,1f,1f);
        camera.GetComponent<CameraControllerISO>().puzzleOff();
        blocVide = obj;
        obj.SetActive(false);
    }

    private void snaped(Vector3 p){
        transform.position = new Vector3(p.x,0f,p.z);
        transform.localScale = new Vector3(6.5f,6.5f,6.5f);
    }

    public void createEmptyBloc(){
        GameObject[] blocsVide = GameObject.FindGameObjectsWithTag("BlocVide");
        foreach (GameObject bloc in blocsVide)
        {   
            if( Vector3.Distance(bloc.transform.position,this.transform.position) < 10 ){
                return;
            }
        }
        blocVide = Instantiate(blocVide,transform.position,Quaternion.identity,GameObject.Find("Blocs_Vides").transform);
        blocVide.name = this.name + "_Vide";
    }

    public void trainPassThrow(){
        if( changeMat ){
            GetComponent<BoxCollider>().enabled = false;
            isPlaced = true;
            StartCoroutine("materialAnimation");
        }
    }

    public void returnToInvotory(Transform t){
        audio.pitch = 0.5f;
        audio.Play();
        transform.parent = t;
        transform.localScale = uiScale;
        transform.localPosition = initialPosition;
        isPlaced = false;
        blocVide.SetActive(true);
    }

    IEnumerator materialAnimation(){
        int i = 0;
        float time = 0f;
        while( i < 10 ){
            this.transform.GetChild(1).transform.gameObject.GetComponent<Renderer>().material.Lerp(unplacedMat,placedMat,0.2f+time);
            time = time + Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
}
