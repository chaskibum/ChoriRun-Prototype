using UnityEngine;

namespace ScriptsAlpha
{
    public class ObjectsManagerAlpha : MonoBehaviour
    {
        public ObstacleAlpha obstaclePrefab;
        public PowerupAlpha powerupPrefab;
        public IngredientAlpha ingredientPrefab;
        public BadIngredientAlpha badIngredientPrefab;
        public Transform objectSpawner;
        public Transform despawnTransform;

        private GameManagerAlpha _gameManager;

        private void Awake()
        {
            _gameManager = FindAnyObjectByType<GameManagerAlpha>();
        }

        public void StartGame()
        {
            InvokeRepeating(nameof(SpawnObstacle), 2.0f, 1f);
            InvokeRepeating(nameof(SpawnIngredient), 2.5f, 1f);
            InvokeRepeating(nameof(SpawnPowerup), 1.3f, 20f);
            InvokeRepeating(nameof(SpawnBadIngredient), 1f, 10.7f);
        }

        public void StopGame()
        {
            CancelInvoke();
        }

        private void Update()
        {
            foreach (Transform obst in transform)
            {
                obst.position -= obst.right * (_gameManager.GameSpeed * Time.deltaTime);

                if (obst.position.x < despawnTransform.position.x)
                    Destroy(obst.gameObject);
            }
        }

        void SpawnObstacle()
        {
            ObstacleAlpha obstacle = Instantiate(obstaclePrefab, transform.position, Quaternion.identity);
            if (!obstacle) return;
            obstacle.transform.parent = transform;
            obstacle.transform.position = objectSpawner.GetChild(Random.Range(0, 3)).position;
        }

        void SpawnIngredient()
        {
            IngredientAlpha ingredient = Instantiate(ingredientPrefab, transform.position, Quaternion.identity);
            if (!ingredient) return;
            ingredient.transform.parent = transform;
            ingredient.transform.position = objectSpawner.GetChild(Random.Range(0, 3)).position;
        }
        void SpawnPowerup()
        {
            PowerupAlpha powerup = Instantiate(powerupPrefab, transform.position, Quaternion.identity);
            if (!powerup) return;
            powerup.transform.parent = transform;
            powerup.transform.position = objectSpawner.GetChild(Random.Range(0, 3)).position;
        }
        void SpawnBadIngredient()
        {
            BadIngredientAlpha badIngredient = Instantiate(badIngredientPrefab, transform.position, Quaternion.identity);
            if (!badIngredient) return;
            badIngredient.transform.parent = transform;
            badIngredient.transform.position = objectSpawner.GetChild(Random.Range(0, 3)).position;
        }

        public void ClearScreen()
        {
            foreach (Transform obst in transform)
            {
                Destroy(obst.gameObject);
            }
        }
    }
}