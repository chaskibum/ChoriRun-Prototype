using System.Collections;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] int ResumeDelay;
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] Animator CountdownAnim;
    GameManagerBeta gameManager;
    AudioManager audioManager;
    bool canOpenMenu = true;
    bool waitTillEnd;
    TMP_Text CountdownText;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        audioManager = FindFirstObjectByType<AudioManager>();
        CountdownText = CountdownAnim.gameObject.GetComponent<TMP_Text>();
    }
    void Update()
    {
        if (gameManager.GetGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OpenCloseMenu();
                }
            if (Input.GetKeyDown(KeyCode.H))
            {
                gameManager.GetOnRestartEvent.Invoke();
            }            
        }

    }

    public void OpenCloseMenu()
    {
        if (canOpenMenu)
        {
            Menu.SetActive(true);
            // audioManager.StopMusic();
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
        GameOverPanel.SetActive(false);
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
                // audioManager.CueMusic();
                break;
            }
        }
    }
}
