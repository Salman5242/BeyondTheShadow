using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Logic
{
    public interface IDraggable
    {
        GameObject gameObject { get; }
        public void OnDragBegin();
        public void OnDragEnd();
    }
}