using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Menus : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject menuJouer;
    public GameObject menuPrincipal;
    public GameObject menuOptions;
    public GameObject menuTuto;
    public GameObject dropDown;
    public GameObject PageUne;
    public GameObject PageDeux;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void afficheMenuPrincipal(){
        menuPrincipal.SetActive(true);
        menuJouer.SetActive(false);
        menuOptions.SetActive(false);
        menuTuto.SetActive(false);
    }

    public void afficheOptions(){
        menuOptions.SetActive(true);
        menuJouer.SetActive(false);
        menuPrincipal.SetActive(false);
        menuTuto.SetActive(false);
    }
    
    public void afficheJouer(){
        menuJouer.SetActive(true);
        menuPrincipal.SetActive(false);
        menuOptions.SetActive(false);
        menuTuto.SetActive(false);
    }

    public void afficheTuto(){
        menuJouer.SetActive(false);
        menuPrincipal.SetActive(false);
        menuOptions.SetActive(false);
        menuTuto.SetActive(true);
        PageUne.SetActive(true);
        PageDeux.SetActive(false);
    }

    public void afficheTuto2(){
        menuJouer.SetActive(false);
        menuPrincipal.SetActive(false);
        menuOptions.SetActive(false);
        menuTuto.SetActive(true);
        PageUne.SetActive(false);
        PageDeux.SetActive(true);
    }

    public void OnDifficultyChanged () {
        PlayerPrefs.SetInt("difficulte",dropDown.GetComponent<Dropdown>().value);
    }
    
    public void quit(){
        Application.Quit();
    }

    public void playNiveau1(){
        SceneManager.LoadScene("Niveau1");
    }

    public void playNiveau2(){
        
    }

    public void playNiveau3(){
        
    }

    public void playProcedural(){
        
    }
}
