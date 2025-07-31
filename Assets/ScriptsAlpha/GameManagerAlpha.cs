using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ScriptsAlpha
{
    public class GameManagerAlpha : MonoBehaviour
    {
        private static readonly int GameStarted = Animator.StringToHash("GameStarted");
        [SerializeField] private Transform background;
        [SerializeField] private Transform background2;

        public ObjectsManagerAlpha objectsManager;

        public PlayerAlpha player;

        [Header("UI")] public TextMeshProUGUI scoreText;
        public GameObject gameOverPanel;
        public GameObject mainMenuPanel;
        public Transform livesContainer;
        public Transform ingredientsContainer;
        public Animator ChoriPanAnimator;

        [Header("Audio")] public AudioSource motorbikeSound;
        public AudioSource ingredientSound;
        public AudioSource choriSound;
        public AudioMixer mixer;
        public Slider musicSlider;
        public Slider sfxSlider;
        public AudioSource startLoop;
        public AudioSource mainMenuSong;
        public AudioSource startSong;
        public AudioSource songLoop;
        public AudioSource melodyLoop;
        public AudioSource fastMelodyLoop;

        [Header("Properties")] public float gameSpeed = 10f;
        [SerializeField] private float backgroundSpeed = 1f;
        public bool gameOver;
        [SerializeField] private float powerUpDuration = 5f;
        private const float MaxGameSpeed = 35.0f;

        private const float BackgroundLimit = -30.0f;
        private float _timeScoreRepeatRate = 0.5f;

        private int _score;
        private bool _hasAllIngredients;
        private bool isPlayerInvincible;
        public bool isGamePaused;
        private bool gameStarted;
        public Animator animator;
        
        public bool fastLoopActivated = false;

        public void StartGame()
        {
            objectsManager.StartGame();
            InvokeRepeating(nameof(IncreaseSpeed), 0, 1);
            InvokeRepeating(nameof(TimeScore), 0, _timeScoreRepeatRate);
            livesContainer.gameObject.SetActive(true);
            mainMenuPanel.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(true);
            gameStarted = true;
            animator.SetBool(GameStarted, true);
            Restart();
        }

        public void BackToMenu()
        {
            gameStarted = false;
            player.GetComponent<CircleCollider2D>().enabled = false;
            objectsManager.StopGame();
            animator.SetBool(GameStarted, false);
            CancelInvoke();
            StopAllCoroutines();
            livesContainer.gameObject.SetActive(false);
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            Invoke("TurnMainMenuOn", stateInfo.length);
            ingredientsContainer.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(false);
            motorbikeSound.Stop();
            BackToMenuMusic();
        }

        void TurnMainMenuOn()
        {
            mainMenuPanel.SetActive(true);
        }

        private void Update()
        {
            background.position -= transform.right * (gameSpeed * Time.deltaTime);

            if (background.position.x < BackgroundLimit)
                background.position = new Vector3(0, background.position.y, background.position.z);

            background2.position -= transform.right * (backgroundSpeed * Time.deltaTime);

            if (background2.position.x < BackgroundLimit)
                background2.position = new Vector3(0, background2.position.y, background2.position.z);
        }

        public void IncreaseSpeed()
        {
            if (fastLoopActivated) return;
            
            if (gameSpeed >= MaxGameSpeed)
            {
                FastLoop();
                motorbikeSound.volume = 1.5f;
                return;
            }

            motorbikeSound.pitch += 0.02f;

            gameSpeed += 0.2f;
        }

        public void TimeScore()
        {
            if (gameSpeed < MaxGameSpeed)
            {
                _score++;
                _timeScoreRepeatRate = 0.5f;
            }
            else
            {
                _score++;
                _timeScoreRepeatRate = 0.2f;
            }

            scoreText.text = _score.ToString();
        }

        public void AddScore(int points = 1)
        {
            _score += points;
            scoreText.text = _score.ToString();
        }

        public void HpFeedback(int hp)
        {
            livesContainer.GetChild(hp).gameObject.SetActive(false);
        }

        public void ChoriFeedback(int ingredient)
        {
            if (ingredientsContainer.GetChild(0).gameObject.GetComponent<Image>().color == Color.white &&
                ingredient == 0)
                ingredientsContainer.GetChild(4).gameObject.GetComponent<Image>().color = Color.white;
            ingredientsContainer.GetChild(ingredient).gameObject.GetComponent<Image>().color = Color.white;
            ingredientSound.Play();

            foreach (Transform child in ingredientsContainer)
            {
                if (child.gameObject.GetComponent<Image>().color == Color.black)
                {
                    _hasAllIngredients = false;
                    break;
                }

                _hasAllIngredients = true;
            }

            if (_hasAllIngredients)
            {
                MakeChori();
                _hasAllIngredients = false;
            }
        }

        private void MakeChori()
        {
            foreach (Transform child in ingredientsContainer)
            {
                child.gameObject.GetComponent<Image>().color = Color.black;
            }

            ChoriPanAnimator.Play("ChoriPanCompleted", 0, 0);
            AnimatorStateInfo stateInfo = ChoriPanAnimator.GetCurrentAnimatorStateInfo(0);
            Invoke("AddScoreAfterAnim", stateInfo.length);
        }

        private void AddScoreAfterAnim()
        {
            AddScore(250);
            choriSound.Play();
        }

        /*public void ActivatePowerUp()
        {
            isPlayerInvincible = true;
            gameSpeed += 5f;
            StartCoroutine(DisablePowerupAfterTime());
            player.PlayPowerUpFeedback();
            motorbikeSound.pitch += 0.5f;
        }

        IEnumerator DisablePowerupAfterTime()
        {
            yield return new WaitForSeconds(powerUpDuration);
            isPlayerInvincible = false;
            gameSpeed -= 5f;
            motorbikeSound.pitch -= 0.5f;
        }*/

        public void GameOver()
        {
            gameOver = true;
            gameOverPanel.SetActive(true);
            Time.timeScale = 0;
            motorbikeSound.Stop();
            ingredientsContainer.gameObject.SetActive(false);
            StopMusic();
        }

        public void Restart()
        {
            player.hp = 2;
            _score = 0;
            scoreText.text = _score.ToString();
            player.ResetPosition();
            gameOverPanel.SetActive(false);
            livesContainer.GetChild(0).gameObject.SetActive(true);
            livesContainer.GetChild(1).gameObject.SetActive(true);
            Time.timeScale = 1;
            gameSpeed = 10f;
            motorbikeSound.Play();
            objectsManager.ClearScreen();
            isPlayerInvincible = false;
            ingredientsContainer.gameObject.SetActive(true);
            player.GetComponent<CircleCollider2D>().enabled = true;
            foreach (Transform child in ingredientsContainer)
            {
                child.gameObject.GetComponent<Image>().color = Color.black;
            }

            motorbikeSound.pitch = 1f;
            StopMusic();
            CueMusic();
        }

        public PlayerAlpha GetPlayer => player;

        public float GameSpeed => gameSpeed;

        public bool GetisPlayerInvincible => isPlayerInvincible;

        public bool GetGameState => gameStarted;

        public void ExitGame()
        {
            Application.Quit();
        }
        
        #region Audio

        public void ChangeMusicVolume()
        {
            mixer.SetFloat("MusicVolume", musicSlider.value);
        }

        public void ChangeSfxVolume()
        {
            mixer.SetFloat("SFXVolume", sfxSlider.value);
        }

        IEnumerator FadeMusic(AudioSource audioSource, bool fadeIn)
        {
            if (fadeIn)
            {
                while (audioSource.volume < 1)
                {
                    audioSource.volume += 0.02f;
                    yield return new WaitForSeconds(0.05f);
                }
            }
            else
            {
                while (audioSource.volume > 0)
                {
                    audioSource.volume -= 0.02f;
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        public void StartSong()
        {
            startSong.volume = 1f;
            startSong.Play();
            startLoop.Stop();
        }

        public void StartSongLoop()
        {
            songLoop.volume = 1f;
            melodyLoop.volume = 1f;
            songLoop.Play();
            melodyLoop.Play();
            fastMelodyLoop.Play();
        }

        public void FastLoop()
        {
            StartCoroutine(FadeMusic(melodyLoop, false));
            StartCoroutine(FadeMusic(fastMelodyLoop, true));
            fastLoopActivated = true;
        }

        public void CueMusic()
        {
            if (!startLoop.isPlaying) startLoop.Play();
            
            StartCoroutine(FadeMusic(startLoop, true));
            StartCoroutine(FadeMusic(mainMenuSong, false));
            Invoke("StartSong", (startLoop.clip.length - startLoop.time));
            Invoke("StartSongLoop", startSong.clip.length + (startLoop.clip.length - startLoop.time));
        }

        public void StopMusic()
        {
            CancelInvoke("StartSongLoop");
            CancelInvoke("StartSong");
            startLoop.Stop();
            mainMenuSong.Stop();
            startSong.Stop();
            songLoop.Stop();
            melodyLoop.Stop();
            fastMelodyLoop.Stop();
        }

        public void BackToMenuMusic()
        {
            mainMenuSong.Play();
            StartCoroutine(FadeMusic(mainMenuSong, true));
            StartCoroutine(FadeMusic(startLoop, false));
            StartCoroutine(FadeMusic(startSong, false));
            StartCoroutine(FadeMusic(songLoop, false));
            StartCoroutine(FadeMusic(melodyLoop, false));
            StartCoroutine(FadeMusic(fastMelodyLoop, false));
        }
        
        #endregion
    }
}
