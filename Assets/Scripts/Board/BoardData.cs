using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardData", menuName = "Data/BoardData/Data")]
public class BoardData : ScriptableObject
{
    [Header("Prefabs\n_____________")]
    public BoardDataPrefabTemplate PrefabPack;
    public Veggie[] VeggiesPrefabs;

    [HideInInspector]
    public Transform TileParent;

    [Space(20)]

    [Header("Board Rect\n_____________")]
    public uint width = 7;
    public uint height = 7;
    [Space(5)]
    public Vector2 position;

    [Space(20)]

    [Header("Layout\n_____________")]
    public Tile[] BoardLayout;
    public ArrayLayout<bool> ArrayLayout;

    private void OnValidate()
    {
        if (ArrayLayout.rows.Length == height && ArrayLayout.rows[0].row.Length == width)
            return;

        if (width >= 1 && height >= 1)
            ArrayLayout = new ArrayLayout<bool>(width, height);
    }
}

