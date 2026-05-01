using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models.Dtos;

public class UserCreateDto
{
    [Required]
    [StringLength(100)]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }
}
