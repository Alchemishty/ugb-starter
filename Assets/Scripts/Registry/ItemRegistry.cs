using UnityEngine;
using HarnessTestBed.Data;

namespace HarnessTestBed.Registry
{
    /// <summary>
    /// Central registry for item data. Singleton, persists across scenes.
    /// </summary>
    public class ItemRegistry : MonoBehaviour
    {
        public static ItemRegistry Instance { get; private set; }

        [SerializeField] private ItemCatalog masterCatalog;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public ItemData GetItem(string itemId)
        {
            if (Instance == null) return null;
            if (masterCatalog == null) return null;

            return masterCatalog.GetById(itemId);
        }

        public ItemCatalog GetCatalog()
        {
            return masterCatalog;
        }
    }
}
