using System.ComponentModel.DataAnnotations;

namespace LibraryInventory.Core.Settings;

public class LibrarySettings
{
    [Range(1, 90)] public int MaxCheckoutDays { get; set; } = 14;
    [Range(1, 20)] public int MaxBooksPerMember { get; set; } = 3;
    [RegularExpression("^(Standard|Extended)$")] public string DueDatePolicy { get; set; } = "Standard";
    [Range(1, 4)] public int ExtendedLoanMultiplier { get; set; } = 2;
}
