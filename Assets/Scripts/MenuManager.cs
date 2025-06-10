using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject instructionsMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    
    private readonly string _level = "Level";

    private void Awake()
    {
        instructionsMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(_level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
