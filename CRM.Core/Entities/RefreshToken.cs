
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Core.Entities
{
  public class RefreshToken
  {
    [Key]
    [Required]
    [Column("user_id")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [Column("refresh_token_string")]
    public string RefreshTokenString { get; set; } = null!;
  }
}
