using System.Collections;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    [SerializeField] int ResumeDelay;
    [SerializeField] Animator CountdownAnim;
    GameManagerBeta gameManager;
    bool canOpenMenu = true;
    bool waitTillEnd;
    TMP_Text CountdownText;
    bool gameStarted = false;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        CountdownText = CountdownAnim.gameObject.GetComponent<TMP_Text>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void OnStart()
    {
        gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OpenCloseMenu();
                }
            if (Input.GetKeyDown(KeyCode.H))
            {
                gameManager.Restart();
            }            
        }

    }

    public void OpenCloseMenu()
    {
        if (canOpenMenu)
        {
            Menu.SetActive(true);
            Time.timeScale = 0;
            gameManager.isGamePaused = true;
            canOpenMenu = false;
        }
        else if (!waitTillEnd)
        {
            waitTillEnd = true;
            Menu.SetActive(false);
            StartCoroutine(ResumeGame());
        }
    }
    public void CloseMenuOnRestart()
    {
        Time.timeScale = 1;
        gameManager.isGamePaused = false;
        waitTillEnd = false;
        canOpenMenu = true;
        Menu.SetActive(false);
    }


    IEnumerator ResumeGame()
    {
        int SecondsLeft = ResumeDelay;
        while (true)
        {
            CountdownText.text = SecondsLeft.ToString();
            CountdownAnim.Play("ResumeTextGrowing", 0, 0);
            SecondsLeft--;
            yield return new WaitForSecondsRealtime(1);
            if (SecondsLeft <= 0)
            {
                Time.timeScale = 1;
                gameManager.isGamePaused = false;
                waitTillEnd = false;
                canOpenMenu = true;
                break;
            }
        }
    }
}
