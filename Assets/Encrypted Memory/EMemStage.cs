using System.Linq;

public class EMemStage {
    public int[] storedLabels;
    public string displayStored;
    public int expectedIdxPos;
    public EMemStage(string displayToStore = "", int labelAmounts = 4)
    {
        storedLabels = Enumerable.Range(0, labelAmounts).ToArray().Shuffle();
        displayStored = displayToStore;
    }
}
