using System.Collections;
using TMPro;
using UnityEngine;
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

        [Header("UI")]
        public TextMeshProUGUI scoreText;
        public GameObject gameOverPanel;
        public GameObject mainMenuPanel;
        public Transform livesContainer;
        public Transform ingredientsContainer;
        public Animator ChoriPanAnimator;

        [Header("Audio")]
        public AudioSource motorbikeSound;
        public AudioSource music;
        public AudioSource ingredientSound;
        public AudioSource choriSound;

        [Header("Properties")]
        public float gameSpeed = 10f;
        [SerializeField] private float backgroundSpeed = 1f;
        public bool gameOver;
        [SerializeField] private float powerUpDuration = 5f;
        private const float MaxGameSpeed = 30.0f;

        private const float BackgroundLimit = -30.0f;
        private float _timeScoreRepeatRate = 0.5f;

        private int _score;
        private bool _hasAllIngredients;
        private bool isPlayerInvincible;
        public bool isGamePaused;
        private bool gameStarted;
        public Animator animator;

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
            if (gameSpeed >= MaxGameSpeed) return;
            
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
            if (ingredientsContainer.GetChild(0).gameObject.GetComponent<Image>().color == Color.white && ingredient == 0)
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
        public void ActivatePowerUp()
        {
            isPlayerInvincible = true;
            gameSpeed += 5f;
            StartCoroutine(DisablePowerupAfterTime());
        }
        IEnumerator DisablePowerupAfterTime()
        {
            yield return new WaitForSeconds(powerUpDuration);
            isPlayerInvincible = false;
            gameSpeed -= 5f;
        }

        public void GameOver()
        {
            gameOver = true;
            gameOverPanel.SetActive(true);
            Time.timeScale = 0;
            motorbikeSound.Stop();
            music.volume = 0.2f;
            ingredientsContainer.gameObject.SetActive(false);
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
            music.volume = 1f;
            motorbikeSound.Play();
            objectsManager.ClearScreen();
            isPlayerInvincible = false;
            ingredientsContainer.gameObject.SetActive(true);
            player.GetComponent<CircleCollider2D>().enabled = true;
            foreach (Transform child in ingredientsContainer)
            {
                child.gameObject.GetComponent<Image>().color = Color.black;
            }
        }

        public PlayerAlpha GetPlayer => player;

        public float GameSpeed => gameSpeed;
        public bool GetisPlayerInvincible => isPlayerInvincible;
        public bool GetGameState => gameStarted;
    }
}
