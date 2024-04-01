using RPG.Combat;
using RPG.Inventories;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace RPG.Shops
{
    public class ShopDataEditor : OdinMenuEditorWindow
    {
        private CreateNewShopConfig createNewShopConfig;
        [MenuItem("Tools/Shop Data Editor")]
        private static void OpenWindow()
        {
            var window = GetWindow<ShopDataEditor>();
        }
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            createNewShopConfig = new CreateNewShopConfig();
            tree.Add("Create New", createNewShopConfig);
            tree.AddAllAssetsAtPath("Shop Data", "Assets/Game/Shops/Data", typeof(ShopConfig));
            return tree;
        }
        protected override void OnBeginDrawEditors()
        {
            // Get the currently selected object from the tree
            OdinMenuTreeSelection selected = MenuTree.Selection;
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();
                if (SirenixEditorGUI.ToolbarButton("Delete Current"))
                {
                    ShopConfig config = selected.SelectedValue as ShopConfig;
                    string path = AssetDatabase.GetAssetPath(config);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
        protected override void OnDestroy()
        {
            // Make sure we are not leaving the CreateNewShopConfig class
            // in memory
            base.OnDestroy();
            if (createNewShopConfig.shopConfig != null)
            {
                DestroyImmediate(createNewShopConfig.shopConfig);
            }
        }
        public class CreateNewShopConfig
        {
            public CreateNewShopConfig()
            {
                shopConfig = ScriptableObject.CreateInstance<ShopConfig>();
            }
            public string shopName;
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public ShopConfig shopConfig;

            [Button("Save")]
            private void CreateNewShop()
            {
                AssetDatabase.CreateAsset(shopConfig, "Assets/Game/Shops/Data/" + shopName + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}