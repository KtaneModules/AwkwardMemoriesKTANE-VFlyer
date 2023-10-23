using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniMemoryScript : MonoBehaviour {
	public KMSelectable[] btnSelectables;
	public TextMesh displayMesh;
	public TextMesh[] btnLabels;
	public FakeStatusLight fakeStatusLight;
	public List<int[]> storedIdxLabels = new List<int[]>();
	public List<int> storedIdxExpected = new List<int>();
	public int stagesCompleted = 0;

	public void ClearMemory()
    {
		storedIdxLabels.Clear();
		storedIdxExpected.Clear();
	}

	public virtual void GenerateNewStage()
    {
		storedIdxLabels.Add(Enumerable.Range(0, btnLabels.Length).ToArray().Shuffle());
    }

	public delegate void CauseStrike();
}
