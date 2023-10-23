using MemoryAny;
using System.Linq;

public class MemoryRuleRS
{
	public RuleType storedRule;
	public OverridePress storedOverride;
	public int[] args;

	public MemoryRuleRS(RuleType newRule, params int[] variables)
    {
		storedRule = newRule;
		storedOverride = OverridePress.None;
		args = variables.ToArray();
    }
	public MemoryRuleRS(RuleType newRule, OverridePress itemOverride, params int[] variables)
    {
		storedRule = newRule;
		storedOverride = itemOverride;
		args = variables.ToArray();
    }
}
