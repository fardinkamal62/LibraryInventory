using System.ComponentModel.DataAnnotations;

namespace LibraryInventory.Core.Settings;

public class JwtSettings
{
    [Required] public string Issuer { get; set; } = "LibraryInventory";
    [Required] public string Audience { get; set; } = "LibraryInventory.Api";
    [Required, MinLength(32)] public string Key { get; set; } = string.Empty;
    [Range(1, 1440)] public int ExpiryMinutes { get; set; } = 60;
}
