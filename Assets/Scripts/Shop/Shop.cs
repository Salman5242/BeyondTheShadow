using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Infrastructure.Services;
using Infrastructure;

public class Shop : MonoBehaviour
{
    [SerializeField] private ItemsInventory _inventory;

    [SerializeField] private Image _itemMenu;
    [SerializeField] private TextMeshProUGUI _shopMoneyTXT;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private TextMeshProUGUI _amount;

    [SerializeField] private ItemsList _itemsList;

    [SerializeField] private int _maxItems = 20;

    private GameFactory _gameFactory;
    private Money _money;
    private int _curItem = 0;

    private void Start()
    {
        _gameFactory = AllServices.Container.Single<GameFactory>();

        LoadMoney();
    }
    public void OpenItemMenu(int itemId)
    {
        _itemMenu.gameObject.SetActive(true);

        _name.text = _itemsList.ItemsInfo[itemId].Name;
        _description.text = _itemsList.ItemsInfo[itemId].Description;
        _price.text = "Price: " + _itemsList.ItemsInfo[itemId].Price.ToString() + " $";
        _amount.text = _inventory._purchasedItemsAmount[itemId].ToString() + " / " + _maxItems.ToString();

        _curItem = itemId;
    }

    public void BuyItem()
    {
        if (_money.CheckForEnoughMoney(_itemsList.ItemsInfo[_curItem].Price) && _inventory._purchasedItemsAmount[_curItem] < _maxItems)
        {
            _money.SpendMoney(_itemsList.ItemsInfo[_curItem].Price);
            _inventory.AddItem(_curItem);
            _shopMoneyTXT.text = "Money: " + _money.GetMoney().ToString() + " $";
            _amount.text = _inventory._purchasedItemsAmount[_curItem].ToString() + " / " + _maxItems.ToString();
        }
    }

    public void LoadMoney()
    {
        if (_money == null)
        {
            if (_gameFactory.GetMainHero() != null) _money = _gameFactory.GetMainHero().GetComponent<Money>();
        }

        if (_money != null) _shopMoneyTXT.text = "Money: " + _money.GetMoney().ToString() + " $";
    }

    public float CalculateItemsCost(int[] items)
    {
        float cost = 0;
        for (int i = 0; i < items.Length; i++)
        {
            cost += items[i] * _itemsList.ItemsInfo[i].Price;
        }
        return cost;
    }
}
