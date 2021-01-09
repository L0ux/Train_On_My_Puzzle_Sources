using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    public GameObject menuOptions;
    public GameObject menuPause;
    public GameObject canvasPause;

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //ATTRIBUER ECRAN DE VICTOIRE
    }

    public void afficheOptions(){
        menuOptions.SetActive(true);
        menuPause.SetActive(false);
    }

    public void affichePause(){
        menuOptions.SetActive(false);
        menuPause.SetActive(true);
    }

    public void reprendre(){
        gameManager.OnPause();
    }

    public void defautMenu(){
        menuOptions.SetActive(false);
        menuPause.SetActive(true);
        canvasPause.SetActive(false);
    }

    public void recommencer(){
        defautMenu();
        gameManager.reloadLevel();
    }

    public void nivSuivant(){
        defautMenu();
        gameManager.nextLevel();
    }

    public void ending(GameObject ending){
        canvasPause.SetActive(true);
        menuPause = ending;
        menuPause.SetActive(true);
    }

    public void quit(){
        if(gameManager != null) gameManager.OnPause();
        SceneManager.LoadScene("Menu");
    }
}
