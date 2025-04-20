using Infrastructure;
using Infrastructure.Services;
using System.Collections;
using TMPro;
using UnityEngine;

public class SanityViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _sanityTXT;

    private LevelSetUp _levelSetUp;
    private SanityHandler _sanityHandler;

    private WaitForSeconds WaitOneSec = new WaitForSeconds(1f);
    private void Start()
    {
        _levelSetUp = AllServices.Container.Single<LevelSetUp>();
        if (_levelSetUp.MainPlayer == null) _levelSetUp.OnLevelSetedUp += SetUp;
        else SetUp();
    }

    private void OnDestroy()
    {
        _levelSetUp.OnLevelSetedUp -= SetUp;
    }
    private IEnumerator UpdateSanity()
    {
        while (true)
        {
            UpdateSanityTXT();
            yield return WaitOneSec;
        }
    }

    private void UpdateSanityTXT()
    {
        _sanityTXT.text = ((int)_sanityHandler.Sanity).ToString();
    }

    private void SetUp()
    {
        _sanityHandler = _levelSetUp.MainPlayer.GetComponent<SanityHandler>();
        StartCoroutine(nameof(UpdateSanity));
    }
}
