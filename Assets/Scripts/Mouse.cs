using Map;
using Map.Path;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Mouse : MonoBehaviour
{
    List<Vector3> path = new List<Vector3>();
    Vector3 poi;
    private float _mouseSpeed;


    public void Init()
    {
        //PathFinding.Instance.GetVectorPath(MapManager.Instance.GetMouseTile(), MapManager.Instance.GetCheeseTile());
        _mouseSpeed = PlayerPrefs.GetFloat("Speed");
        poi = MapManager.Instance.GetCheeseTile().centerPoint.position;

        PathFinding.Instance.SetPathStart(MapManager.Instance.GetMouseTile(), MapManager.Instance.GetCheeseTile());
        //SetSearchingPath();
        //transform.DOPath(path.ToArray(), path.Count * 0.5f);

        StartCoroutine(MoveCoroutine());
        /*path = PathFinding.Instance.GetVectorPath(MapManager.Instance.GetNearestTile(transform.position), MapManager.Instance.GetLastTile());
        if (path != null)
            transform.DOPath(path.ToArray(), 10f);
        else
            print("path null");*/
    }

    private void SetSearchingPath()
    {
        Vector3 nextPos = MapManager.Instance.GetMouseTile().centerPoint.position;
        path.Add(nextPos);
        while (nextPos != poi)
        {
            nextPos = PathFinding.Instance.GetNextNode(nextPos).tile.centerPoint.position;
            path.Add(nextPos);
        } 
    }

    IEnumerator MoveCoroutine()
    {
        yield return new WaitForSeconds(1f);
        while (transform.position != poi)
        {
            Vector3 nextPos = PathFinding.Instance.GetNextNode(MapManager.Instance.GetNearestTile(transform.position).centerPoint.position).tile.centerPoint.position;
            transform.DOMove(nextPos, _mouseSpeed).SetEase(Ease.Linear);
            yield return new WaitForSeconds(_mouseSpeed); 
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
