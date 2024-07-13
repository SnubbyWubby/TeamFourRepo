using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TackleBox.UI
{
    public class UIMenu : MonoBehaviour
    {
        public List<GameObject> MenuElements = new List<GameObject>();

        public GameObject GetElement(string elementName)
        {
            foreach (GameObject element in MenuElements)
            {
                if (element != null && element.name == elementName)
                    return element;
            }


            return null;
        }
    }
}