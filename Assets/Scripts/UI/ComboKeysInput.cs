using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Simius
{
    public class ComboKeysInput : MonoBehaviour, IReadOnlyList<ComboKeysInput.Shortcut>
    {

        [System.Serializable]
        public struct Shortcut
        {
            public bool ctrl, shift;
            
            public KeyCode key;

            public UnityEvent onPressEvent;
            public event UnityAction OnPress
            {
                add => onPressEvent.AddListener(value);
                remove => onPressEvent.RemoveListener(value);
            }
        }

        [SerializeField]
        private Shortcut[] shortcuts = default;
        public Shortcut this[int index] => shortcuts[index];

        public int Count => shortcuts.Length;


        public bool CtrlPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        public bool ShiftPressed => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);


        private void Update()
        {
            bool ctrl = CtrlPressed, shift = ShiftPressed;

            foreach (Shortcut shortcut in shortcuts)
            {
                if ((!shortcut.ctrl || ctrl) && (!shortcut.shift || shift) && Input.GetKeyDown(shortcut.key))
                {
                    shortcut.onPressEvent.Invoke();
                }
            }
        }


        public IEnumerator<Shortcut> GetEnumerator() => (IEnumerator<Shortcut>)shortcuts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => shortcuts.GetEnumerator();

    }
}
