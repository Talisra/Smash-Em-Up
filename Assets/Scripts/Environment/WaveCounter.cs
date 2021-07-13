using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCounter : MonoBehaviour
{
    public SevenSegment segments;

    private void Update()
    {
        segments.SetNumber(WaveManager.Instance.Wave);
    }
}
