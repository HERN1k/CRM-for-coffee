﻿using CRM.Core.Entities.BaseEntities;
using CRM.Core.Interfaces.Entity;

namespace CRM.Core.Entities
{
  public class EntityUser : BaseEntity, IEntityWithId
  {
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FatherName { get; set; } = null!;
    public int Age { get; set; }
    public string Gender { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Post { get; set; } = null!;
    public bool IsConfirmed { get; set; }
    public DateTime RegistrationDate { get; set; }
    public EntityRefreshToken RefreshToken { get; set; } = null!;
  }
}