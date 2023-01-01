using UnityEngine.EventSystems;
using System;



namespace UnityEngine.UI
{
    public class HoldableButton : Button
    {
        public event Action OnHoldStart;
        public event Action OnHoldEnd;



        public override void OnPointerDown(PointerEventData eventData)
        {
            OnHoldStart?.Invoke();

            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            OnHoldEnd?.Invoke();
            
            base.OnPointerUp(eventData);
        }
    } 
}
