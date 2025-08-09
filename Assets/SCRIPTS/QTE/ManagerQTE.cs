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
            GameManager.Instance.FishCaught(currentSpecies);
        }
        else
        {
            GameManager.Instance.FishNotCaught(currentSpecies);
        }

        qteUI.SetActive(false);
    }
}