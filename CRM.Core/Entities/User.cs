
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Core.Entities
{
  public class User
  {
    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [Column("password")]
    public string Password { get; set; } = null!;

    [Required]
    [Column("first_name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [Column("last_name")]
    public string LastName { get; set; } = null!;

    [Required]
    [Column("father_name")]
    public string FatherName { get; set; } = null!;

    [Required]
    [Column("age")]
    public int Age { get; set; }

    [Required]
    [Column("gender")]
    public string Gender { get; set; } = null!;

    [Required]
    [Column("phone_number")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Required]
    [Column("post")]
    public string Post { get; set; } = null!;//isVerifiction

    [Required]
    [Column("is_confirmed")]
    public bool IsConfirmed { get; set; }

    [Required]
    [Column("registration_date")]
    public string RegistrationDate { get; set; } = null!;

    public RefreshToken RefreshToken { get; set; } = null!;
  }
}
