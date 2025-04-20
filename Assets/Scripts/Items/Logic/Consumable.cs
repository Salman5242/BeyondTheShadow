using Infrastructure;
using Infrastructure.Services;
using Player.Inventory;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    private Inventory _inventory;
    private LevelSetUp _levelSetUp;
    
    void Start()
    {
        _levelSetUp = AllServices.Container.Single<LevelSetUp>();
        if (_levelSetUp.MainPlayer == null) _levelSetUp.OnLevelSetedUp += SetUp;
        else SetUp();
    }

    private void OnDestroy()
    {
        _levelSetUp.OnLevelSetedUp -= SetUp;
    }


    public void Consume()
    {
        _inventory.DropMainItem(false);
        Destroy(this.gameObject);
    }

    private void SetUp()
    {
        _inventory = _levelSetUp.MainPlayer.GetComponent<Inventory>();
    }
}
