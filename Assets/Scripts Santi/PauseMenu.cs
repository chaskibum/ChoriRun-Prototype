using System.Collections;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    [SerializeField] int ResumeDelay;
    [SerializeField] Animator ResumeTextAnim;
    GameManagerSanti gameManager;
    bool canOpenMenu = true;
    bool waitTillEnd;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerSanti>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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
            ResumeTextAnim.gameObject.GetComponent<TMP_Text>().text = SecondsLeft.ToString();
            ResumeTextAnim.Play("ResumeTextGrowing", 0, 0);
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
