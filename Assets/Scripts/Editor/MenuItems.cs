using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class MenuItems
{
    [MenuItem("CONTEXT/Component/Select All/Any Depth/Children", false, 1)]
    static void SelectAllAtAnyDepth0(MenuCommand menuCommand) { SelectAllAnyDepth(menuCommand, 0); }

    [MenuItem("CONTEXT/Component/Select All/Any Depth/Siblings", false, 1)]
    static void SelectAllAtAnyDepth1(MenuCommand menuCommand) { SelectAllAnyDepth(menuCommand, 1); }

    [MenuItem("CONTEXT/Component/Select All/Any Depth/Cousins", false, 2)]
    static void SelectAllAtAnyDepth2(MenuCommand menuCommand) { SelectAllAnyDepth(menuCommand, 2); }

    [MenuItem("CONTEXT/Component/Select All/Any Depth/2nd Cousins", false, 3)]
    static void SelectAllAtAnyDepth3(MenuCommand menuCommand) { SelectAllAnyDepth(menuCommand, 3); }

    [MenuItem("CONTEXT/Component/Select All/Any Depth/3rd Cousins", false, 4)]
    static void SelectAllAtAnyDepth4(MenuCommand menuCommand) { SelectAllAnyDepth(menuCommand, 4); }

    [MenuItem("CONTEXT/Component/Select All/Any Depth/4th Cousins", false, 5)]
    static void SelectAllAtAnyDepth5(MenuCommand menuCommand) { SelectAllAnyDepth(menuCommand, 5); }

    [MenuItem("CONTEXT/Component/Select All/Any Depth/5th Cousins", false, 6)]
    static void SelectAllAtAnyDepth6(MenuCommand menuCommand) { SelectAllAnyDepth(menuCommand, 6); }

    [MenuItem("CONTEXT/Component/Select All/Any Relative")]
    static void SelectAllAtAnyDepth7(MenuCommand menuCommand) { SelectAllAnyDepth(menuCommand, 10000); }

    [MenuItem("CONTEXT/Component/Select All/Scene")]
    static void SelectAllScene(MenuCommand menuCommand)
    {
        Component context = menuCommand.context as Component;
        List<GameObject> selection = new List<GameObject>();

        Array.ForEach(context.gameObject.scene.GetRootGameObjects(), x => GetAllAnyDepth(x.transform, context.GetType(), ref selection));

        Selection.objects = selection.ToArray();
    }

    static void SelectAllAnyDepth(MenuCommand menuCommand, int rootHeight)
    {
        Component context = menuCommand.context as Component;

        Transform root = context.transform;
        for (int i = 0; i < rootHeight; i++)
        {
            
            if (root.parent == null)
            {
                break;
            } else root = root.parent;
        }

        List<GameObject> selection = new List<GameObject>();
        GetAllAnyDepth(root, context.GetType(), ref selection);

        Selection.objects = selection.ToArray();
    }

    static void GetAllAnyDepth(Transform root, System.Type type, ref List<GameObject> all)
    {
        if (root.GetComponent(type) != null)
        {
            all.Add(root.gameObject);
        }

        foreach (Transform child in root)
            GetAllAnyDepth(child, type, ref all);
    }

    [MenuItem("CONTEXT/Component/Select All/Same Depth/Siblings", false, 1)]
    static void SelectAllAtDepth1(MenuCommand menuCommand) { SelectAllSameDepth(menuCommand, 1); }

    [MenuItem("CONTEXT/Component/Select All/Same Depth/Cousins", false, 2)]
    static void SelectAllAtDepth2(MenuCommand menuCommand) { SelectAllSameDepth(menuCommand, 2); }

    [MenuItem("CONTEXT/Component/Select All/Same Depth/2nd Cousins", false, 3)]
    static void SelectAllAtDepth3(MenuCommand menuCommand) { SelectAllSameDepth(menuCommand, 3); }

    [MenuItem("CONTEXT/Component/Select All/Same Depth/3rd Cousins", false, 4)]
    static void SelectAllAtDepth4(MenuCommand menuCommand) { SelectAllSameDepth(menuCommand, 4); }

    [MenuItem("CONTEXT/Component/Select All/Same Depth/4th Cousins", false, 5)]
    static void SelectAllAtDepth5(MenuCommand menuCommand) { SelectAllSameDepth(menuCommand, 5); }

    [MenuItem("CONTEXT/Component/Select All/Same Depth/5th Cousins", false, 6)]
    static void SelectAllAtDepth6(MenuCommand menuCommand) { SelectAllSameDepth(menuCommand, 6); }

    

    static void SelectAllSameDepth(MenuCommand menuCommand, int rootHeight)
    {
        Component context = menuCommand.context as Component;

        Transform root = context.transform;
        for (int i = 0; i < rootHeight; i++)
        {
            root = root.parent;
            if (root == null)
            {
                Debug.LogError("SelectAllSameDepth has no root at height of " + rootHeight);
                return;
            }
        }

        List<GameObject> selection = new List<GameObject>();
        GetAllAtDepth(root, rootHeight, context.GetType(), ref selection);

        Selection.objects = selection.ToArray();
    }

    static void GetAllAtDepth(Transform root, int depth, System.Type type, ref List<GameObject> all)
    {
        if (depth == 0)
        {
            if (root.GetComponent(type) != null)
            {
                all.Add(root.gameObject);
            }
        }
        else
            foreach (Transform child in root)
                GetAllAtDepth(child, depth - 1, type, ref all);
    }

    #region Create Prefabs

    private const string SteamVRPlayerRigPath = "Assets/Prefabs/SteamVR Player Rig.prefab";
    private const string ContextSwitcher = "Assets/Prefabs/Context Switcher.prefab";
    private const string FreezeAndReset = "Assets/Prefabs/Freeze And Reset.prefab";

    [MenuItem("GameObject/ETBIM/SteamVR Player Rig", false, 10)]
    static void CreateSteamVRPlayerRig(MenuCommand menuCommand)
    {
        CreateCustomGameObject(menuCommand, AssetDatabase.LoadAssetAtPath<GameObject>(SteamVRPlayerRigPath));
    }

    [MenuItem("GameObject/ETBIM/Context Switcher", false, 10)]
    static void CreateContextSwitcher(MenuCommand menuCommand)
    {
        CreateCustomGameObject(menuCommand, AssetDatabase.LoadAssetAtPath<GameObject>(ContextSwitcher));
    }

    [MenuItem("GameObject/ETBIM/Freeze And Reset", false, 10)]
    static void CreateFreezeAndReset(MenuCommand menuCommand)
    {
        CreateCustomGameObject(menuCommand, AssetDatabase.LoadAssetAtPath<GameObject>(FreezeAndReset));
    }

    static void CreateCustomGameObject(MenuCommand menuCommand, GameObject prefab)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    #endregion
}
