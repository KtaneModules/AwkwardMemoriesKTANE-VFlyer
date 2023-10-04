using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EncryptedMemoryScript : MonoBehaviour {
	public KMBombModule modSelf;
	public KMAudio mAudio;
	public KMSelectable[] buttonsSelectable;
	public MeshRenderer[] progressRenderers;
	public TextMesh displayMesh;
	public TextMesh[] btnMeshes;
	public Material[] matsStatus;
	List<EMemStage> storedMemoryStages = new List<EMemStage>();
	int moduleID;
	static int modIDCnt;
	enum MemoryInfo
    {
		None,
		Label,
		Position,
		Stage
    }
	[SerializeField]
	private Vector3 offsetHide = new Vector3(0, -.008f, 0);
	Vector3[] storedInitialPos;
	int stagesCompleted = 0;
	bool interactable = false;
	// Use this for initialization
	void QuickLog(string toLog, params object[] args)
    {
		Debug.LogFormat("[{0} #{1}] {2}", modSelf.ModuleDisplayName, moduleID, string.Format(toLog, args));
    }

	void Start () {
		storedInitialPos = buttonsSelectable.Select(a => a.transform.localPosition).ToArray();
		for (var x = 0; x < buttonsSelectable.Length; x++)
		{
			buttonsSelectable[x].transform.localPosition = storedInitialPos[x] + offsetHide;
			var y = x;
			buttonsSelectable[x].OnInteract += () => {
				if (interactable)
					HandleIdxPress(y);
				return false;
			};
		}
		for (var x = 0; x < btnMeshes.Length; x++)
			btnMeshes[x].text = "";
		displayMesh.text = "";
		for (var x = 0; x < progressRenderers.Length; x++)
			progressRenderers[x].material = matsStatus[0];
		moduleID = ++modIDCnt;
		modSelf.OnActivate += () => { HandleStageGen(true); StartCoroutine(DisplayStage()); };
	}
	void HandleIdxPress(int idx)
    {
		mAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, buttonsSelectable[idx].transform);
		buttonsSelectable[idx].AddInteractionPunch(0.5f);
		buttonsSelectable[idx].transform.localPosition = storedInitialPos[idx] + offsetHide / 4;
		interactable = false;
		var refStage = storedMemoryStages.ElementAt(stagesCompleted);
		var inputCorrect = refStage == null || refStage.expectedIdxPos == idx;
		if (inputCorrect)
        {
			QuickLog("Correctly pressed Position {0}, Label {1}.", idx, refStage.storedLabels[idx]);
			stagesCompleted++;
			if (stagesCompleted >= 5)
			{
				QuickLog("5 completed stages in a row. Get some rest for now.");
				StartCoroutine(ChangeStage(false));
				modSelf.HandlePass();
				return;
			}
        }
		else
        {
			QuickLog("Incorrectly pressed Position {0}, Label {1}. Resetting.", idx, refStage.storedLabels[idx]);
			modSelf.HandleStrike();
		}
		HandleStageGen(!inputCorrect);
		StartCoroutine(ChangeStage());
	}

	void HandleStageGen(bool resetFully = false)
    {
		if (resetFully)
		{
			stagesCompleted = 0;
			storedMemoryStages.Clear();
		}
		var newMemStage = new EMemStage();

		var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		var digits = "0123456789";
		var base36Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		var allowedStrFormat = new[] { 
			new[] { "LP", "0123" }, // Label/Position #
			new[] { "PL", "S", "0123", "C" }, // Position/Label Correctly Pressed on Stage #
			new[] { "P", "S", "0123", "L", "0123" }, // Position of Label # on Stage #
			new[] { "L", "S", "0123", "P", "0123" }, // Label of Positon # on Stage #
		};
		var strOutput = "";
        var allowedIdxFormatsAllStages = new[] {
			new[] { 0 },
			new[] { 0, 1 },
			new[] { 0, 1, 2, 3 },
			new[] { 0, 1, 2, 3 },
			new[] { 1, 2, 3 } };

		
		var pickedStrFormat = allowedStrFormat[allowedIdxFormatsAllStages[stagesCompleted].PickRandom()];
		var stageDetected = false;
		foreach (string option in pickedStrFormat)
		{
			if (!stageDetected)
			{
				var pckedOpt = option.PickRandom();
				strOutput += pckedOpt;
				if (pckedOpt == 'S')
					stageDetected = true;
			}
			else
            {
				var remainingOptions = option.Substring(0, stagesCompleted);
				var pckedOpt = remainingOptions.PickRandom();
				strOutput += pckedOpt;
				stageDetected = false;
			}
		}
		newMemStage.displayStored = strOutput;
		storedMemoryStages.Add(newMemStage);
		var typeProcess = new List<MemoryInfo>();
		var processedDigits = new List<int>();
		var targetStageIdx = stagesCompleted;
		foreach (char processChr in newMemStage.displayStored)
        {
			var targetStage = storedMemoryStages[targetStageIdx];
			if (digits.Contains(processChr))
            {
				var idxDigit = digits.IndexOf(processChr);
				processedDigits.Insert(0, idxDigit);
				/*var lastItemProcess = typeProcess[pointer];
				switch (lastItemProcess)
                {
					case MemoryInfo.Label:
						targetValue = Enumerable.Range(0, 4).Single(a => targetStage.storedLabels[a] == idxDigit);
						break;
					case MemoryInfo.Position:
						targetValue = idxDigit;
						break;
					case MemoryInfo.Stage:
						targetStageIdx = idxDigit;
						break;
                }
				pointer++;*/
            }
			else switch(processChr)
                {
					/*case 'C':
                        {
							var lastItemProcess = typeProcess[pointer];
							switch (lastItemProcess)
							{
								case MemoryInfo.Label:
									targetValue = targetStage.storedLabels[targetStage.expectedIdxPos];
									break;
								case MemoryInfo.Position:
									targetValue = targetStage.expectedIdxPos;
									break;
							}
							pointer++;
						}
						break;*/
					case 'S':
						typeProcess.Insert(0, MemoryInfo.Stage);
						break;
					case 'L':
						typeProcess.Insert(0, MemoryInfo.Label);
						break;
					case 'P':
						typeProcess.Insert(0, MemoryInfo.Position);
						break;
                }
        }
		Debug.Log(typeProcess.Join());
		Debug.Log(processedDigits.Join());
		var targetIdxPos = -1;
		var targetLabel = -1;
		for (var x = 0; x < typeProcess.Count; x++)
        {
			if (x >= processedDigits.Count)
            {
				var focusedStage = storedMemoryStages[targetStageIdx];
				switch (typeProcess[x])
				{
					case MemoryInfo.Label:
						if (targetIdxPos == -1)
							targetLabel = focusedStage.storedLabels[focusedStage.expectedIdxPos];
						else
							targetLabel = focusedStage.storedLabels[targetIdxPos];
						targetIdxPos = Enumerable.Range(0, 4).Single(a => newMemStage.storedLabels[a] == targetLabel);
						break;
					case MemoryInfo.Position:
						if (targetLabel == -1)
							targetIdxPos = focusedStage.expectedIdxPos;
						else
							targetIdxPos = Enumerable.Range(0, 4).Single(a => focusedStage.storedLabels[a] == targetLabel);
						break;
				}
			}
			else
            {
				switch (typeProcess[x])
                {
					case MemoryInfo.Label:
						targetLabel = processedDigits[x];
						break;
					case MemoryInfo.Position:
						targetIdxPos = processedDigits[x];
						break;
					case MemoryInfo.Stage:
						targetStageIdx = processedDigits[x];
						break;
                }
			}
        }
		if (targetIdxPos != -1)
			newMemStage.expectedIdxPos = targetIdxPos;
		else
			newMemStage.expectedIdxPos = Enumerable.Range(0, 4).Single(a => newMemStage.storedLabels[a] == targetLabel);
		// Encrypt the freaking display.
		var alteredDisplay = strOutput.Select(a => digits.Contains(a) ? digits[digits.IndexOf(a) + 1] : a).Join("");
		var pickedOffset = Random.value < 0.5f ? 5 : 10;
		switch (stagesCompleted)
        {
			default:
				break;
			case 1:
				alteredDisplay = alteredDisplay.Select(a => digits.Contains(a) ? digits[(digits.IndexOf(a) + pickedOffset) % digits.Length] : a).Join("");
				break;
			case 2:
				alteredDisplay = alteredDisplay.Select(a => alphabet.Contains(a) ? alphabet[(alphabet.IndexOf(a) + pickedOffset) % alphabet.Length] : a).Join("");
				break;
			case 3:
				alteredDisplay = alteredDisplay.Select(a => digits.Contains(a) ? digits[(digits.IndexOf(a) + pickedOffset) % digits.Length] : alphabet.Contains(a) ? alphabet[(alphabet.IndexOf(a) + pickedOffset) % alphabet.Length] : a).Join("");
				break;
			case 4:
				alteredDisplay = alteredDisplay.Select(a => base36Digits.Contains(a) ? base36Digits[(base36Digits.IndexOf(a) + pickedOffset) % base36Digits.Length] : a).Join("");
				break;

		}
		newMemStage.displayStored = alteredDisplay;

		QuickLog("Stage {0}:", stagesCompleted + 1);
		QuickLog("Labels from left to right: {0}", newMemStage.storedLabels.Select(a => a + 1).Join(", "));
		QuickLog("Display: {0}", newMemStage.displayStored);
		if (stagesCompleted > 0)
			QuickLog("Unencrypted Display: {0}", strOutput.Select(a => digits.Contains(a) ? digits[digits.IndexOf(a) + 1] : a).Join(""));
		QuickLog("The module expects the following to be pressed: Position {0}, Label {1}", newMemStage.expectedIdxPos + 1, newMemStage.storedLabels[newMemStage.expectedIdxPos] + 1);
    }
	IEnumerator ButtonAdjust(int idx, bool hide = false)
    {
		if (hide)
		{
			for (float t = 0; t < 1f; t += Time.deltaTime)
			{
				buttonsSelectable[idx].transform.localPosition = storedInitialPos[idx] + offsetHide * t;
				yield return null;
			}
			buttonsSelectable[idx].transform.localPosition = storedInitialPos[idx] + offsetHide;
		}
		else
		{
			for (float t = 0; t < 1f; t += Time.deltaTime)
			{
				buttonsSelectable[idx].transform.localPosition = storedInitialPos[idx] + offsetHide * (1f - t);
				yield return null;
			}
			buttonsSelectable[idx].transform.localPosition = storedInitialPos[idx];
		}
	}

	IEnumerator ChangeStage(bool actuallyChangeStage = true)
    {
		displayMesh.text = "";
		for (var x = 0; x < progressRenderers.Length; x++)
			progressRenderers[x].material = matsStatus[x < stagesCompleted ? 1 : 0];
		yield return new WaitForSeconds(1f);
		var btnsAnim = Enumerable.Range(0, 4).Select(a => ButtonAdjust(a, true)).ToArray();
		var running = true;
		var iterCount = 0;
		while (running)
		{
			running = false;
			for (var x = 0; x < btnsAnim.Length && x * 5 <= iterCount; x++)
				running |= btnsAnim[x].MoveNext();
			yield return null;
			iterCount++;
		}
		if (actuallyChangeStage)
			yield return DisplayStage(stagesCompleted);
	}

	IEnumerator DisplayStage(int stageIdx = 0)
    {
		var grabbedStage = storedMemoryStages.ElementAtOrDefault(stageIdx);
		if (grabbedStage == null) yield break;
		var btnsAnim = Enumerable.Range(0, 4).Select(a => ButtonAdjust(a, false)).ToArray();
		var running = true;
		var iterCount = 0;
		for (var x = 0; x < btnMeshes.Length; x++)
			btnMeshes[x].text = (grabbedStage.storedLabels[x] + 1).ToString();
		while (running)
		{
			running = false;
			for (var x = 0; x < btnsAnim.Length && x * 5 <= iterCount; x++)
				running |= btnsAnim[x].MoveNext();
			yield return null;
			iterCount++;
		}
		interactable = true;
		var displayedObtained = grabbedStage.displayStored;
		if (displayedObtained.Length <= 2)
			displayMesh.text = displayedObtained;
		else
        {
			for (var cnt = -1; interactable;cnt++)
            {
				if (cnt == -1)
					displayMesh.text = displayedObtained.ElementAtOrDefault(cnt + 1).ToString();
				else
				{
					var ch1 = displayedObtained.ElementAtOrDefault(cnt).ToString();
					var ch2 = displayedObtained.ElementAtOrDefault(cnt + 1).ToString();
					displayMesh.text = ch1 + ch2;
				}
				if (cnt >= displayedObtained.Length + 2)
					cnt = -2;
				yield return new WaitForSeconds(0.5f);
            }
        }
	}
}
