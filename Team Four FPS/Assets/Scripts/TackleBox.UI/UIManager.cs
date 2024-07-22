using System.Collections;
using System.Collections.Generic;
using TackleBox.Audio;
using Unity.VisualScripting;
using UnityEngine;

namespace TackleBox.UI
{
    public class UIManager : MonoBehaviour
    {
        static UIManager _instance;
        [SerializeField] List<GameObject> _MenuList = new List<GameObject>();
        public GameObject activeMenu;

        public List<GameObject> GetUIList()
        {
            return _MenuList;
        }

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Search for an existing instance in the scene
                    _instance = FindObjectOfType<UIManager>();

                    if (_instance != null) 
                        Debug.LogError("UIManager instance not found! Try adding one to the scene first.");
                }
                return _instance;
            }
        }
    }
}