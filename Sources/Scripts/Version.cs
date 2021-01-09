using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour
{   

    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "Version " + Application.version;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
