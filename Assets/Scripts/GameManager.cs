using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public GameObject setting;
    public List<Text> texts;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void settingMenu()
    {
        setting.SetActive(!setting.activeSelf);
    }

    public static void changeText(int type, float val)
    {
        instance.texts[type].text = val.ToString();
    }
}
