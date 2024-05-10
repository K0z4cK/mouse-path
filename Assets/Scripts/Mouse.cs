using Map;
using Map.Path;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mouse : MonoBehaviour
{
    List<Vector3> path;

    public void Init()
    {
        print(MapManager.Instance.GetNearestTile(transform.position));
        print(MapManager.Instance.GetLastTile());

        GetComponent<SpriteRenderer>().enabled = true;

        path = PathFinding.Instance.GetVectorPath(MapManager.Instance.GetNearestTile(transform.position), MapManager.Instance.GetLastTile());
        if (path != null)
            transform.DOPath(path.ToArray(), 30f);
        else
            print("path null");
    }
}
