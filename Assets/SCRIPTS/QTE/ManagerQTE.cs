using UnityEngine;

public class ManagerQTE : MonoBehaviour

{
    public GameObject qteUI; 
    public FishQTE fishQTE;

    private SpeciesSO currentSpecies;

    void Start()
    {
        qteUI.SetActive(false);
        GameManager.Instance.OnFishTouch += StartQTE;
        fishQTE.OnQTEFinished += HandleQTEResult;
    }

    void StartQTE(SpeciesSO species)
    {
        currentSpecies = species;
        qteUI.SetActive(true);
        GameManager.Instance.TriggerStopScroll();
        fishQTE.StartQTE();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartQTE(null);
    }

    void HandleQTEResult(bool success)
    {
        if (success)
        {
            Debug.Log("Handle QTE Succes");
            GameManager.Instance.FishCaught(currentSpecies);
            GameManager.Instance.TriggerStartScroll();

        }
        else
        {
            Debug.Log("Handle QTE Failed");

            GameManager.Instance.FishNotCaught(currentSpecies);
            GameManager.Instance.TriggerStartScroll();

        }

        GameManager.Instance.TriggerStartScroll();

        qteUI.SetActive(false);
    }
}