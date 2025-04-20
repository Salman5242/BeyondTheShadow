using Ghosts.EnvIneraction;
using System.Collections;
using Utilities;
using UnityEngine;

public class PoltergeistUnique : MonoBehaviour
{
    public bool MakePolterAbility = false;
    private float _abilityChance = 3.5f;
    private float _abilityCheckTime = 15;

    private WaitForSeconds WaitForAbilityCD;
    private GhostEnvInteraction _ghostInteraction;

    private void Start()
    {
        WaitForAbilityCD = new WaitForSeconds(_abilityCheckTime);

        _ghostInteraction = GetComponentInParent<GhostEnvInteraction>();
        StartCoroutine(nameof(CheckForAbility));
    }

    private IEnumerator CheckForAbility()
    {
        while (true)
        {
            if (RandomGenerator.CalculateChance(_abilityChance) || MakePolterAbility)
            {
                CastAbility();
                MakePolterAbility = false;
            }
            yield return WaitForAbilityCD;
        }
    }
    private void CastAbility()
    {
        _ghostInteraction.InteractWithEverything();
    }
}
