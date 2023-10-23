using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MemoryAny;
namespace MemoryAny
{
    public enum RuleType
    {
		None,
		Label,
		Pos,
		CorrectPosOfStageX,
		CorrectLabelOfStageX,
		LabelOfPosXOfStageY,
		PosOfLabelXOfStageY,
	}
	public enum OverridePress
    {
		None,
		LabelEquals,
		PosEquals
    }
}


public class MemoryceptionScript : MonoBehaviour {

	public KMBombModule modSelf;
	public KMAudio mAudio;
	public KMRuleSeedable ruleSeed;
	public TextMesh displayMesh;
	public MeshRenderer[] stageLeds;

	public MiniMemoryScript[] miniMemories;
	MemoryRuleRS[][] storedNormalRulesCombined;
	MemoryRuleRS[][] storedRulesMini;

	int moduleID;
	static int modIDCnt;
	void QuickLog(string toLog, params object[] args)
    {
		Debug.LogFormat("[{0} #{1}] {2}", modSelf.ModuleDisplayName, moduleID, string.Format(toLog, args));
    }
	void HandleRuleSeed()
    {
		var randomizer = new MonoRandom(1);
		if (ruleSeed != null)
			randomizer = ruleSeed.GetRNG();
		if (randomizer.Seed == 1)
        {
			storedRulesMini = new MemoryRuleRS[][] {
				new[] { new MemoryRuleRS(RuleType.Label, 2), new MemoryRuleRS(RuleType.Label, 0), new MemoryRuleRS(RuleType.Label, 1) },
				new[] { new MemoryRuleRS(RuleType.CorrectPosOfStageX, 0), new MemoryRuleRS(RuleType.Label, 2), new MemoryRuleRS(RuleType.Pos, 1) },
				new[] { new MemoryRuleRS(RuleType.CorrectPosOfStageX, 1), new MemoryRuleRS(RuleType.CorrectPosOfStageX, 0), new MemoryRuleRS(RuleType.Label, 0) },
				new[] { new MemoryRuleRS(RuleType.CorrectPosOfStageX, 0), new MemoryRuleRS(RuleType.CorrectPosOfStageX, 1), new MemoryRuleRS(RuleType.CorrectPosOfStageX, 2) },
			};
			storedNormalRulesCombined = new MemoryRuleRS[][] {
				new[] { new MemoryRuleRS(RuleType.Pos, 0), new MemoryRuleRS(RuleType.Pos, 1), new MemoryRuleRS(RuleType.Pos, 2) },
				new[] { new MemoryRuleRS(RuleType.CorrectPosOfStageX, 0), new MemoryRuleRS(RuleType.PosOfLabelXOfStageY, 2, 0), new MemoryRuleRS(RuleType.CorrectLabelOfStageX, 0) },
				new[] { new MemoryRuleRS(RuleType.CorrectPosOfStageX, 1), new MemoryRuleRS(RuleType.Label, 1), new MemoryRuleRS(RuleType.CorrectPosOfStageX, OverridePress.LabelEquals, 0) },
				new[] { new MemoryRuleRS(RuleType.CorrectLabelOfStageX, 2), new MemoryRuleRS(RuleType.CorrectLabelOfStageX, 1), new MemoryRuleRS(RuleType.CorrectLabelOfStageX, 0) },
			};
        }
		else
        {
			var restrictionMiniRules = new RuleType[][] {
				new[] { RuleType.Label, RuleType.Pos },
				new[] { RuleType.Label, RuleType.Pos, RuleType.CorrectPosOfStageX, RuleType.CorrectLabelOfStageX },
				new[] { RuleType.Label, RuleType.Pos, RuleType.CorrectPosOfStageX, RuleType.CorrectLabelOfStageX },
				new[] { RuleType.CorrectPosOfStageX, RuleType.CorrectLabelOfStageX },
			};
			var restrictionCombinedRules = new RuleType[][] {
				new[] { RuleType.Label, RuleType.Pos },
				new[] { RuleType.Label, RuleType.Pos, RuleType.CorrectPosOfStageX, RuleType.CorrectLabelOfStageX },
				new[] { RuleType.Label, RuleType.Pos, RuleType.CorrectPosOfStageX, RuleType.CorrectLabelOfStageX },
				new[] { RuleType.CorrectPosOfStageX, RuleType.CorrectLabelOfStageX },
			};
        }
    }


	// Use this for initialization
	void Start () {
		moduleID = ++modIDCnt;
	}

	// Update is called once per frame
	void Update () {

	}
}

