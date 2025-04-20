using Items.Logic;
using System;

namespace Items.Logic
{
    public interface IDroppable : IPickupable
    {
        public void DropItem();
    }
}
