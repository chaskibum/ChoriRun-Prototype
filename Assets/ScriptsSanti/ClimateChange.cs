using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ClimateChange : MonoBehaviour
{
    [Header("Variables")]

    [SerializeField] Light2D GlobalLight;
    [SerializeField] TMP_Text ScoreText;
    [SerializeField] TMP_Text GameOverText;


    [Header("Properties")]
    [SerializeField] int ScoreToChangeDayTime;
    int targetScore;

    [SerializeField] float TargetIntensity;
    [SerializeField] float GlobalTargetIntensity;
    [SerializeField] bool DayTime;
    [Header("Colors")]
    [SerializeField] Color NightColor;
    [SerializeField] Color DayColor;
    [SerializeField] Color NightTextColor;
    
    [SerializeField] List<GameObject> DayBackgroundVisual;
    [SerializeField] List<GameObject> CloudsVisual;
    [SerializeField] List<Light2D> Lights;

    GameManagerBeta gameManagerBeta;

    Coroutine DayBackgroundCoroutine;
    Coroutine CloudsCoroutine;
    Coroutine LightsCoroutine;
    Coroutine TextColorCoroutine;
    Coroutine TextColorCoroutine2;



    void Awake()
    {
        gameManagerBeta = FindFirstObjectByType<GameManagerBeta>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerBeta.GetOnRestartEvent.AddListener(OnRestart);
        gameManagerBeta.GetOnQuitButtonEvent.AddListener(OnRestart);
        gameManagerBeta.GetOnstartEvent.AddListener(OnStart);
        gameManagerBeta.GetOnQuitButtonEvent.AddListener(OnQuit);
        // StartCoroutine(ChangeDayTimeBasedOnScore());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ChangeDayTime();
        }
    }
    
    /*
    IEnumerator ChangeDayTimeBasedOnScore()
    {
        while (true)
        {
            targetScore += ScoreToChangeDayTime;
            yield return new WaitUntil(() => gameManagerBeta.GetScore >= targetScore);
            ChangeDayTime();
        }
    }*/
    
    public void ChangeDayTime()
    {
        StopCoroutines();
        DayTime = !DayTime;
        if (!DayTime)
        {
            MakeDay();
        }
        else
        {
            MakeNight();
        }
    }



    void MakeNight()
    {
        DayBackgroundCoroutine = StartCoroutine(IncreaseDecreaseAlpha(DayBackgroundVisual, false));
        CloudsCoroutine = StartCoroutine(IncreaseDecreaseAlpha(CloudsVisual, false));
        LightsCoroutine = StartCoroutine(IncreaseDecreaseLights(Lights, GlobalLight, NightColor, true));
        TextColorCoroutine = StartCoroutine(ChangeTextColor(NightTextColor, ScoreText));
        TextColorCoroutine2 = StartCoroutine(ChangeTextColor(NightTextColor, GameOverText));
    }

    void MakeDay()
    {
        DayBackgroundCoroutine = StartCoroutine(IncreaseDecreaseAlpha(DayBackgroundVisual, true));
        CloudsCoroutine = StartCoroutine(IncreaseDecreaseAlpha(CloudsVisual, true));
        LightsCoroutine = StartCoroutine(IncreaseDecreaseLights(Lights, GlobalLight, DayColor, false));
        TextColorCoroutine = StartCoroutine(ChangeTextColor(Color.black, ScoreText));
        TextColorCoroutine2 = StartCoroutine(ChangeTextColor(Color.black, GameOverText));
    }

    void StopCoroutines()
    {
        if(DayBackgroundCoroutine != null) StopCoroutine(DayBackgroundCoroutine);
        if(CloudsCoroutine != null) StopCoroutine(CloudsCoroutine);
        if(LightsCoroutine != null) StopCoroutine(LightsCoroutine);
        if(TextColorCoroutine != null) StopCoroutine(TextColorCoroutine);
    }

    IEnumerator IncreaseDecreaseAlpha(List<GameObject> ListOfObjects, bool Increase)
    {
        Color color;

        float Target;

        bool AnyVisible = true;
        bool IncreaseOrDecrease()
        {

            if (Increase)
            {
                Target = 1;
                return color.a < Target;
            }
            else
            {
                Target = 0;
                return color.a > Target;
            }

        }
        while (AnyVisible)
        {
            AnyVisible = false;
            foreach (GameObject Object in ListOfObjects)
            {
                color = Object.GetComponent<SpriteRenderer>().color;
                if (IncreaseOrDecrease())
                {
                    color.a = Mathf.MoveTowards(color.a, Target, Time.deltaTime);
                    Object.GetComponent<SpriteRenderer>().color = color;
                    AnyVisible = true;
                }
            }
            yield return null;
        }
    }
    IEnumerator ChangeTextColor(Color TextColor, TMP_Text text)
    {
        Color color = text.color;

        while (true)
        {
            color = Color.Lerp(color, TextColor, Time.deltaTime * 2);
            text.color = color;
            if (color == TextColor) break;
            yield return null;
        }
    }
    IEnumerator IncreaseDecreaseLights(List<Light2D> ListOfLights, Light2D GlobalLight, Color TargetColor, bool Increse)
    {
        float MaxIntensity = 0;
        float MaxIntensityGlobal = 0;

        bool AnyVisible = false;
        bool IncreseOrDecrease(float Intensity, float DefaultValue, float TargetValue, bool isGlobalLight = false)
        {
            if (Increse)
            {
                if (!isGlobalLight) MaxIntensity = TargetValue;
                else MaxIntensityGlobal = TargetValue;

                return Intensity < TargetValue;
            }
            else
            {
                if (!isGlobalLight) MaxIntensity = DefaultValue;
                else MaxIntensityGlobal = DefaultValue;

                return Intensity > DefaultValue;
            }
        }

        while (!AnyVisible)
        {
            AnyVisible = true;
            foreach (Light2D Light in ListOfLights)
            {
                float Intensity = Light.GetComponent<Light2D>().intensity;

                if (IncreseOrDecrease(Intensity, 0, TargetIntensity))
                {
                    Light.GetComponent<Light2D>().intensity = Mathf.MoveTowards(Intensity, MaxIntensity, Time.deltaTime);
                    AnyVisible = false;
                }
            }

            if (!IncreseOrDecrease(GlobalLight.intensity, 1, GlobalTargetIntensity, true))
            {
                GlobalLight.intensity = Mathf.MoveTowards(GlobalLight.intensity, MaxIntensityGlobal, Time.deltaTime);
                AnyVisible = false;
            }

            if (Vector4.Distance(GlobalLight.color, TargetColor) > 0.001f)
            {
                GlobalLight.color = Color.Lerp(GlobalLight.color, TargetColor, Time.deltaTime * 2);
                AnyVisible = false;
            }

            yield return null;
        }
    }

    void OnRestart()
    {
        StopAllCoroutines();
        DayTime = false;
        MakeDay();
    }

    void OnStart()
    {
        InvokeRepeating(nameof(ChangeDayTime), 30f, 30f);
    }

    void OnQuit()
    {
        CancelInvoke();
        MakeDay();
    }
}
