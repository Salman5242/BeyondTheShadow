using Items.Logic;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Player.Inventory
{
    public class Inventory : MonoBehaviour
    {
        public IPickupable MainItem { get; private set; }

        public bool IsInventoryFull
        {
            get { return _currItemAmmount >= _maxItemAmmount; }
            private set { }
        }

        [SerializeField]
        private Transform _slot;

        [SerializeField]
        private int _maxItemAmmount;

        [SerializeField]
        private float _dropItemOffset = 0.5f;
        [SerializeField]
        private Vector2 _dropItemForce = new Vector2(3f, 1f);


        private List<IPickupable> _pickupableSlots = new List<IPickupable>();
        private int _currItemAmmount = 0;

        private int currMainItemSlot = 0;


        private void Start()
        {
            ResizeSlots();
            MainItem = null;
        }


        public bool AddItem(IPickupable itemToAdd)
        {
            if (_currItemAmmount < _maxItemAmmount)
            {
                int slotToPassNum = 0;
                if (_pickupableSlots[currMainItemSlot] == null && MainItem == null)
                {
                    slotToPassNum = currMainItemSlot;
                }
                else
                {
                    for (int slotNum = 0; slotNum < _maxItemAmmount; slotNum++)
                    {
                        if (_pickupableSlots[slotNum] == null)
                        {
                            slotToPassNum = slotNum;
                            break;
                        }
                    }
                }

                PickItemRb(itemToAdd);
                _pickupableSlots[slotToPassNum] = itemToAdd;
                _currItemAmmount++;

                if (slotToPassNum == currMainItemSlot)
                {
                    ChangeMainItem(slotToPassNum);
                }
                return true;
            }
            else return false;
        }

        public void ChangeMainItem(int slotNum)
        {
            if (_pickupableSlots[currMainItemSlot] != null)
            {
                IDisababled disabable = _pickupableSlots[currMainItemSlot].gameObject.GetComponent<IDisababled>();
                if (disabable != null) disabable.DisableItem();
                else
                    _pickupableSlots[currMainItemSlot].gameObject.SetActive(false);
            }

            currMainItemSlot = slotNum;
            if (_pickupableSlots[currMainItemSlot] != null)
            {
                MainItem = _pickupableSlots[slotNum];

                IDisababled disabable = _pickupableSlots[currMainItemSlot].gameObject.GetComponent<IDisababled>();
                if (disabable != null) disabable.EnableItem();
                else
                    _pickupableSlots[currMainItemSlot].gameObject.SetActive(true);
            }
            else
            {
                MainItem = null;
            }
        }

        public void SwitchMainItemSlot()
        {
            int newItemSlot = currMainItemSlot + 1;
            if (newItemSlot >= _maxItemAmmount) newItemSlot = 0;

            ChangeMainItem(newItemSlot);
        }

        public void DropMainItem(bool shouldThrow = true)
        {
            if (MainItem != null)
            {
                DropItemRb(MainItem, shouldThrow);

                IDroppable droppable = MainItem.gameObject.GetComponent<IDroppable>();
                if (droppable != null) droppable.DropItem();  
                _currItemAmmount--;
                _pickupableSlots[currMainItemSlot] = null;
                MainItem = null;
            }
        }

        private void PickItemRb(IPickupable item)
        {
            Rigidbody itemRB = item.gameObject.GetComponent<Rigidbody>();
            itemRB.isKinematic = true;
            itemRB.velocity = Vector3.zero;
            itemRB.angularVelocity = Vector3.zero;

            // Set Slot as a parent
            item.gameObject.transform.SetParent(_slot);

            // Reset position and rotation
            item.gameObject.transform.localPosition = Vector3.zero;
            item.gameObject.transform.localEulerAngles = Vector3.zero;

            IDisababled disabable = item.gameObject.GetComponent<IDisababled>();
            if (disabable != null) disabable.DisableItem();
            else
                item.gameObject.SetActive(false);
        }

        private void DropItemRb(IPickupable item, bool shouldThrow = true)
        {
            item.gameObject.transform.SetParent(_slot.parent);
            //EditorApplication.isPaused = true;
            if (shouldThrow)
                item.gameObject.transform.localPosition = new Vector3(0f, item.gameObject.transform.localPosition.y, _dropItemOffset);
            //  EditorApplication.isPaused = true;
            item.gameObject.transform.SetParent(null);
            if (shouldThrow)
            {
                Rigidbody itemRB = item.gameObject.GetComponent<Rigidbody>();
                itemRB.isKinematic = false;
                itemRB.AddForce(item.gameObject.transform.forward * _dropItemForce.x + item.gameObject.transform.up * _dropItemForce.y, ForceMode.VelocityChange);
            }
        }

        private void ResizeSlots()
        {
            for (int i = 0; i < _maxItemAmmount; i++)
            {
                _pickupableSlots.Add(null);
            }
        }


    }
}