using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace _Editor._Helper
{
    class GenericAssetSelector<CLASS> : OdinSelector<object>
    {
        public Action<CLASS> SelectionConfirmedCallback;

        private GenericAssetSelector() { }
        public GenericAssetSelector(Action<CLASS> selectionConfirmedCallback)
        {
            SelectionConfirmedCallback = selectionConfirmedCallback;

            SelectionConfirmed += (IEnumerable<object> p_selectedItems) =>
            {
                var enumerator = p_selectedItems.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is CLASS)
                    {
                        SelectionConfirmedCallback?.Invoke((CLASS)enumerator.Current);
                    }
                }
            };
        }

        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            tree.Config.DrawSearchToolbar = true;
            tree.Selection.SupportsMultiSelect = false;
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(CLASS).IsAssignableFrom(p))
                .SelectMany(t => AssetDatabase.FindAssets("t:" + t.Name))
                .Select(assetGUID => AssetDatabase.GUIDToAssetPath(assetGUID))
                .ForEach((string p_path) =>
                {
                    tree.AddAssetAtPath(p_path, p_path);
                });
        }
    }
}
