using UnityEngine;

namespace Items.Logic
{
    public interface IClickable
    {
        GameObject gameObject { get; }
        public void OnClick();
    }
}