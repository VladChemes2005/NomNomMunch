using UnityEngine;

[CreateAssetMenu(fileName = "PrefabTemplate", menuName = "Data/BoardData/PrefabTemplate")]
public class BoardDataPrefabTemplate : ScriptableObject
{
    public GameObject TilePrefab;
    public FlipTile FlipTilePrefab;
    public Ice IcePrefab;
    public Bug BugPrefab;
}

