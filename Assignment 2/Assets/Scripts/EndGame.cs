using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    public Button mouse;
    public Button cat;

    public GameObject diagram;
    public GameObject question;

    public GameObject right;
    public GameObject wrong;

    // Start is called before the first frame update
    void Awake()
    {
        // add a listener to the host button
        mouse.onClick.AddListener(() =>
        {
            // player is correct
            correct();
        });

        // add a listener to the client button
        cat.onClick.AddListener(() =>
        {
            // player is incorrect
            incorrect();
        });
    }

    public void correct()
    {
        mouse.gameObject.SetActive(false);
        cat.gameObject.SetActive(false);
        diagram.SetActive(false);
        question.SetActive(false);

        right.SetActive(true);
    }
    public void incorrect()
    {
        mouse.gameObject.SetActive(false);
        cat.gameObject.SetActive(false);
        diagram.SetActive(false);
        question.SetActive(false);

        wrong.SetActive(true);
    }
}
