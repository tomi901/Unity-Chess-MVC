using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Simius.UI
{
    public class MenuManager : MonoBehaviour, IReadOnlyList<GameObject>
    {

        [SerializeField]
        private int activeIndex = 0;

        [SerializeField]
        private GameObject[] menus = default;

        public int Count => menus.Length;
        public GameObject this[int index] => menus[index];


        private void Start()
        {
            UpdateActive();
        }


        private void UpdateActive() => SelectMenu(activeIndex);


        public void SelectMenu(int index) => SelectMenu(GetMenu(index));

        public void SelectMenu(GameObject selectMenu)
        {
            foreach (GameObject menu in menus)
            {
                menu.SetActive(menu == selectMenu);
            }
        }


        public GameObject GetMenu(int index)
        {
            return (menus != null && index >= 0 && index < menus.Length) ? menus[index] : null;
        }


        private void OnValidate()
        {
            UpdateActive();
        }


        public IEnumerator<GameObject> GetEnumerator() => (IEnumerator<GameObject>)menus.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => menus.GetEnumerator();
    }
}