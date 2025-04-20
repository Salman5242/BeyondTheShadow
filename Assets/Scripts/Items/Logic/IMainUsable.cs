using Items.Logic;

namespace Items.Logic
{
    public interface IMainUsable : IPickupable
    {
        public void OnMainUse();
    }
}