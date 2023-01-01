using UnityEngine;
using UnityEngine.UI;



namespace Xonix.PlayerInput
{
    public class FourDirectionInputTranslator : MonoBehaviour
    {
        [SerializeField] private HoldableButton _upArrowButton;
        [SerializeField] private HoldableButton _downArrowButton;
        [SerializeField] private HoldableButton _rightArrowButton;
        [SerializeField] private HoldableButton _leftArrowButton;


        public HoldableButton UpArrowButton => _upArrowButton;
        public HoldableButton DownArrowButton => _downArrowButton;
        public HoldableButton RightArrowButton => _rightArrowButton;
        public HoldableButton LeftArrowButton => _leftArrowButton;
    }
}
