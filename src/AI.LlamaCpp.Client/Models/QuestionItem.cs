namespace AI.LlamaCpp.Client.Models;

internal class QuestionItem
{
    public string ItemName { get; set; } 

    public string Domain { get; set; } 

    public string Topic { get; set; } 

    public string ItemStatus { get; set; } 

    public string TestType { get; set; } 

    public string FormHistory { get; set; } 

    public string Rationale { get; set; } 

    public string EvidenceStatement1 { get; set; } 

    public string EvidenceStatement2 { get; set; } 

    public string EvidenceStatement3 { get; set; } 

    public double? Difficulty { get; set; }

    public string DifficultyDescription { get; set; } 

    public double? DifficultyRating { get; set; }

    public double? Discrimination { get; set; }

    public string DiscriminationDescription { get; set; } 

    public double? DiscriminationRating { get; set; }

    public double? AverageSecondsToAnswer { get; set; }

    public string SecondsDescription { get; set; } 

    public string Question { get; set; } 

    public string OptionA { get; set; } 

    public string OptionB { get; set; } 

    public string OptionC { get; set; } 

    public string OptionD { get; set; } 

    public string OptionE { get; set; } 

    public string Answer { get; set; } 
}
