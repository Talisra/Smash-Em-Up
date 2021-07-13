using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIObstacleManager : MonoBehaviour
{
    public static AIObstacleManager Instance { get; private set; }

    public AttachPoint[] attachPoints_left;
    public AttachPoint[] attachPoints_right;
    private List<AttachPoint[]> all_points = new List<AttachPoint[]>();
    private float summonOffset; // INIT at Start
    private float aioScale = 5; // the size of AI_Obstacle

    private int pointsActive = 0;

    public void Start()
    {
        if (Instance == null)
            Instance = this;
        summonOffset = 0.25f * aioScale; // 0.25: the value to make a space of "one cube" from aio texture.
        all_points.Add(attachPoints_left);
        all_points.Add(attachPoints_right);
    }

    public void AIOmessage()
    {
        pointsActive--;
        if (pointsActive == 0)
        {
            //Debug.Log("completed obstacle wave at AIO Manager");
            WaveManager.Instance.CompleteObstacleWave();
        }
    }

    public List<AttachPoint> ChainPointGenerator(int chains)
    {
        List<AttachPoint> aps = new List<AttachPoint>();
        for (int i=0; i<chains; i++)
        {
            int pointIdx = Random.Range(0, all_points.Count);
            int index = Random.Range(0, all_points[pointIdx].Length);
            while (aps.Contains(all_points[pointIdx][index]))
            {
                pointIdx = Random.Range(0, all_points.Count);
                index = Random.Range(0, all_points[pointIdx].Length);
            }
            aps.Add(all_points[pointIdx][index]);
        }
        return aps;
    }

    public List<int> ChainAmountGenerator(int chains, int min, int max)
    {
        List<int> amounts = new List<int>();
        for(int i=0; i<chains; i++)
        {
            amounts.Add(Random.Range(min, max+1));
        }
        return amounts;
    }

    public void ClearAll()
    {
        foreach (AttachPoint[] arr in all_points)
        {
            foreach (AttachPoint point in arr)
            {
                ClearPoint(point);
            }
        }
    }

    public void ObstacleWave(int chains, int minObstacles, int maxObstacles)
    {
        StartCoroutine(BuildChain(
            ChainPointGenerator(chains),
            ChainAmountGenerator(chains, minObstacles, maxObstacles)));
    }

    public IEnumerator BuildChain(List<AttachPoint> points, List<int> amounts)
    {
        yield return new WaitForSeconds(0.3f); // start delay to let the last obstacles fade
        int counter = 0;
        while (counter < points.Count)
        {
            pointsActive++;
            StartCoroutine(BuildAt(points[counter], amounts[counter]));
            yield return new WaitForSeconds(1.85f);
            counter++;
        }
    }

    public IEnumerator BuildAt(AttachPoint point, int amount)
    {
        if (point.obstacles.Count > 0)
        {
            Debug.Log("BuildAt() could not execute because the point occupied!");
            yield break;
        }
        List<AIobstacle> aios = Summon(amount, point.transform.position.x > 0? 1: -1);
        yield return new WaitForSeconds(0.15f);
        point.AttachObstacles(aios);
        for(int i=0; i<aios.Count; i++)
        {
            if (i != 0)
            {
                aios[i].SetParentAIO(aios[i-1]);
            }
        }
    }

    public void ClearPoint(AttachPoint point)
    {
        point.DetachAll();
    }

    public List<AIobstacle> Summon(int amount, int side) // -1 left, 0 top (doesn't really matter), 1 right
    {
        List<Vector3> targets = CreateSummonTargets(amount, side);
        List<AIobstacle> aios = new List<AIobstacle>();
        for (int i=0; i<amount; i++)
        {
            GameObject go = PrefabPooler.Instance.Get("AI_Obstacle", targets[i], Quaternion.identity);
            AIobstacle aio = go.GetComponent<AIobstacle>();
            aio.SpawnIn();
            aios.Add(aio);
        }
        return aios;
    }

    private List<Vector3> CreateSummonTargets(int amount, int side)
    {
        List<Vector3> rv = new List<Vector3>();
        List<float> gameArea = GameManager.Instance.GetGameArea();
        Vector3 initialTarget = new Vector3(
            side > 0 ? Random.Range( - 5, gameArea[2] - aioScale * 1.5f) :
                       Random.Range(gameArea[0], 5),
            Random.Range(gameArea[1] + aioScale * 1.5f, gameArea[3] - aioScale * 1.5f),
            0);
        rv.Add(initialTarget);
        rv.AddRange(CreateSummonVectors(initialTarget, amount - 1, side));
        return rv;
    }

    private List<Vector3> CreateSummonVectors(Vector3 initialVector, int amount, int side)
    {
        List<Vector3> rv = new List<Vector3>();
        if (side > 0) // right
        {
            float initX = initialVector.x;
            for(int i=0; i<amount; i++)
            {
                initX -= (aioScale + summonOffset);
                rv.Add(new Vector3(initX, initialVector.y, 0));
            }
        }
        else // left
        {
            float initX = initialVector.x;
            for (int i = 0; i < amount; i++)
            {
                initX += (aioScale + summonOffset);
                rv.Add(new Vector3(initX, initialVector.y, 0));
            }
        }
        return rv;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            List<AttachPoint> attachPoints = new List<AttachPoint>();
            List<int> amountPerChain = new List<int>();
            amountPerChain.Add(3);
            amountPerChain.Add(3);
            amountPerChain.Add(3);
            attachPoints.Add(attachPoints_left[0]);
            attachPoints.Add(attachPoints_left[1]);
            attachPoints.Add(attachPoints_left[2]);
            StartCoroutine(BuildChain(attachPoints, amountPerChain));
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            foreach(AttachPoint[] arr in all_points)
            {
                foreach(AttachPoint point in arr)
                {
                    ClearPoint(point);
                }
            }

        }
    }

}

