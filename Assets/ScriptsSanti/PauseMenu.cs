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
    bool canOpenMenu = true;
    bool waitTillEnd;
    TMP_Text CountdownText;
    Coroutine CheckAnimatonPlaying;
    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        CountdownText = CountdownAnim.gameObject.GetComponent<TMP_Text>();
    }
    void Update()
    {
        if (gameManager.GetGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CheckAnimatonPlaying != null) return;
                CheckAnimatonPlaying = StartCoroutine(CheckIfShakeIsPlaying());
            }      
        }

    }
    IEnumerator CheckIfShakeIsPlaying()
    {
        Animator CameraAnimator = Camera.main.GetComponent<Animator>();

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = CameraAnimator.GetCurrentAnimatorStateInfo(0);
            bool isPlaying = stateInfo.IsName("ShakeCamera") && stateInfo.normalizedTime < 1f;
            return !isPlaying;
        });
        OpenCloseMenu();
        CheckAnimatonPlaying = null;
    }
    public void OpenCloseMenu()
    {
        if (canOpenMenu)
        {
            Menu.SetActive(true);
            AudioManager.Instance.mixer.SetFloat("MusicVolume", AudioManager.Instance.musicSlider.value - 10f);
            AudioManager.Instance.motorbikeSound.volume = 0f;
            AudioManager.Instance.sfxSource.volume = 0f;
            Time.timeScale = 0;
            gameManager.isGamePaused = true;
            canOpenMenu = false;
            Cursor.visible = true;
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
        AudioManager.Instance.mixer.SetFloat("MusicVolume", AudioManager.Instance.musicSlider.value);
        Cursor.visible = false;
        AudioManager.Instance.motorbikeSound.volume = 1f;
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
                AudioManager.Instance.mixer.SetFloat("MusicVolume", AudioManager.Instance.musicSlider.value);
                AudioManager.Instance.motorbikeSound.volume = 1f;
                AudioManager.Instance.sfxSource.volume = 1f;
                Cursor.visible = false;
                break;
            }
        }
    }
}
