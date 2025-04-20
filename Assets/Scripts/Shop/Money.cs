using UnityEngine;
using TMPro;
using Infrastructure.Services;
using Infrastructure;

public class Money : MonoBehaviour
{
    private DataSaveLoader _dataSaveLoader;

    [SerializeField] private float _money;

    private void Start()
    {
        _dataSaveLoader = AllServices.Container.Single<DataSaveLoader>();
        _money = _dataSaveLoader._storedInfo.Money;
    }
    public bool CheckForEnoughMoney(float amount)
    {
        if (_money >= amount) return true;
        else return false;
    }

    public void SpendMoney(float amount)
    {
        _money -= amount;
        _dataSaveLoader.SaveMoney(_money);
    }

    public float GetMoney()
    {
        return _money;
    }

    public void LoadMoney()
    {
        _money = _dataSaveLoader._storedInfo.Money;
    }
}
