using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventaire : MonoBehaviour
{

    public Vector3[] slots;

    private List<GameObject> blocs = new List<GameObject>();
    private GameObject fond;
    private new Camera camera;

    // Start is called before the first frame update
    void Start()
    {   
        camera = GameObject.Find("CameraISO").GetComponent<Camera>();
        GameObject tmp = GameObject.Find("Blocs_A_poser");
        int nbChild = tmp.transform.childCount;
        for( int i = 0; i < nbChild; i++){
            tmp.transform.GetChild(0).transform.parent = this.transform;
        }
        Destroy(tmp);
        fond = transform.GetChild(0).gameObject;
        for(int i = 1; i < transform.childCount; i++){
            blocs.Add(transform.GetChild(i).gameObject);
        }
        int j = 0;
        foreach (GameObject bloc in blocs)
        {   
            bloc.GetComponent<Puzzle>().createEmptyBloc();
            bloc.transform.localPosition = slots[j];
            bloc.GetComponent<Puzzle>().initialPosition = slots[j];
            bloc.transform.localScale = new Vector3(2.5f,2.5f,2.5f);
            j++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void drag(GameObject obj){
        fond.SetActive(false);
        foreach (GameObject bloc in blocs)
        {   
            if( bloc != obj ){
                bloc.SetActive(false);
            }
        }
    }

    public void unDrag(GameObject obj,bool placed){
        fond.SetActive(true);
        if( placed ){
            blocs.Remove(obj);
        }
        foreach (GameObject bloc in blocs)
        {   
            bloc.SetActive(true);
        }
    }

    public void OnClick(){
        Vector3 mousePosition = camera.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x,Mouse.current.position.ReadValue().y,0f));
        RaycastHit hit;
        int layerMask = 1 << 10;
        Debug.DrawRay(mousePosition, camera.transform.forward * 1000, Color.yellow);
        if( Physics.Raycast(mousePosition,camera.transform.forward,out hit, Mathf.Infinity,layerMask)) {
           hit.transform.gameObject.GetComponent<Puzzle>().returnToInvotory(transform);
           blocs.Add(hit.transform.gameObject);
        }
    }

}
