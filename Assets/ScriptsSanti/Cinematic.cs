using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Cinematic : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float CameraSpeed;
    [SerializeField] float CameraSizeStart = 3.5f;
    [SerializeField] float CameraSizeEnd = 8f;
    [SerializeField] int SecondsPerFrame;
    [SerializeField] CanvasGroup FramesCanvasGroup;

    [Header("Cameras/Animators")]
    [SerializeField] Animator FadeAnimator;
    [SerializeField] Camera CinematicCamera;
    [SerializeField] Camera MainCamera;
    [Header("Transforms/GameObjects")]
    [SerializeField] Transform StartPosCamera;
    [SerializeField] GameObject CloseText;
    [SerializeField] List<Transform> CameraPosition;
    [SerializeField] List<GameObject> MovieFrames;
    int Index;
    bool CinematicStarted;
    GameManagerBeta gameManager;
    Coroutine UpdateCameraPositionCoroutine;
    Coroutine UpdateCameraSizeCoroutine;

    float cooldown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManagerBeta>();
        CinematicCamera.orthographicSize = CameraSizeStart;
        gameManager.GetOnQuitButtonEvent?.AddListener(OnQuit);
    }

    // Update is called once per frame
    void Update()
    {
        if (CinematicStarted)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Index < CameraPosition.Count)
                {
                    NextFrame();
                    cooldown = 0;
                }
                else
                {
                    CloseText.SetActive(false);
                    FadeAnimator.SetBool("Fade", false);
                    StartCoroutine(EndCinematic());
                    CinematicStarted = false;
                }
            }
        }
    }

    public void StartCinematic()
    {
        FadeAnimator.SetBool("Fade", true);
        AnimatorStateInfo stateInfo = FadeAnimator.GetCurrentAnimatorStateInfo(0);
        Invoke("SetupCinematic", stateInfo.length);
        StartCoroutine(PassFrame());
    }
    void NextFrame()
    {
        if (UpdateCameraPositionCoroutine != null) StopCoroutine(UpdateCameraPositionCoroutine);
        UpdateCameraPositionCoroutine = StartCoroutine(UpdateCameraPosition(Index));
        if (Index < MovieFrames.Count && MovieFrames[Index] != null) UpdateMovieFrames(Index);
        Index++;
    }

    void SetupCinematic()
    {
        CinematicCamera.gameObject.SetActive(true);
        MainCamera.enabled = false;
        CinematicStarted = true;
        UpdateCameraPositionCoroutine = StartCoroutine(UpdateCameraPosition(Index));
        UpdateMovieFrames(Index);
        Index++;
    }
    void StartGame()
    {
        Animator cameraAnimator = MainCamera.GetComponent<Animator>();

        cameraAnimator.SetBool("GameStarted", true);
        AnimatorStateInfo stateInfo = cameraAnimator.GetCurrentAnimatorStateInfo(0);
        Invoke("InvokeStartEvent", stateInfo.length - 0.3f);
    }
    void InvokeStartEvent()
    {
        gameManager.GetOnstartEvent?.Invoke();
    }
    void UpdateMovieFrames(int index)
    {
        MovieFrames[index]?.SetActive(true);
        StartCoroutine(UpdateAlphaSize(index));
    }
    IEnumerator UpdateCameraPosition(int index)
    {
        if (index == CameraPosition.Count - 1 && UpdateCameraSizeCoroutine == null)
        {
            UpdateCameraSizeCoroutine = StartCoroutine(UpdateCameraSize());
            CloseText.SetActive(true);
        }
        while (Vector3.Distance(CinematicCamera.transform.position, CameraPosition[index].position) > 0.01f)
        {
            CinematicCamera.transform.position = Vector3.Lerp(CinematicCamera.transform.position, CameraPosition[index].position, CameraSpeed * Time.deltaTime);

            yield return null;
        }
    }
    IEnumerator UpdateCameraSize()
    {
        while (!Mathf.Approximately(CinematicCamera.orthographicSize, CameraSizeEnd))
        {
            CinematicCamera.orthographicSize = Mathf.Lerp(CinematicCamera.orthographicSize, CameraSizeEnd, 7 * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator PassFrame()
    {
        while (Index < CameraPosition.Count)
        {
            cooldown += Time.deltaTime;
            if (cooldown > SecondsPerFrame)
            {
                NextFrame();
                cooldown = 0;
            }
            yield return null;
        }
    }

    IEnumerator EndCinematic()
    {
        while (!Mathf.Approximately(FramesCanvasGroup.alpha, 0))
        {
            FramesCanvasGroup.alpha = Mathf.MoveTowards(FramesCanvasGroup.alpha, 0, 2 * Time.deltaTime);
            yield return null;
        }
        MainCamera.enabled = true;
        CinematicCamera.gameObject.SetActive(false);
        StartGame();
    }
    IEnumerator UpdateAlphaSize(int index)
    {
        Image image = MovieFrames[index].GetComponent<Image>();
        while (!Mathf.Approximately(image.color.a, 1f))
        {
            image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, 1), 5 * Time.deltaTime);
            yield return null;
        }
    }

    void OnQuit()
    {
        StopAllCoroutines();
        UpdateCameraSizeCoroutine = null;
        UpdateCameraPositionCoroutine = null;
        foreach (GameObject gameObject in MovieFrames)
        {
            Image image = gameObject.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            Debug.Log("Alpha en 0");
        }
        Index = 0;
        CinematicCamera.transform.position = StartPosCamera.position;
        CinematicCamera.orthographicSize = CameraSizeStart;
        FramesCanvasGroup.alpha = 1;
    }
}
