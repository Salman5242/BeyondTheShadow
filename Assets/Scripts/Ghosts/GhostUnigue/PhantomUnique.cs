using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomUnique : MonoBehaviour
{
    private PhotoReward _ghostPhotoReward;
    private MeshHandler _ghostMeshHandler;
    void Start()
    {
        _ghostPhotoReward = GetComponentInParent<PhotoReward>();
        _ghostMeshHandler = GetComponentInParent<MeshHandler>();

        _ghostPhotoReward.OnPhotoCheck += DisableMesh;
    }

    private void OnDestroy()
    {
        _ghostPhotoReward.OnPhotoCheck -= DisableMesh;
    }

    private void DisableMesh()
    {
        _ghostMeshHandler.MeshDisappear();
    }
}
