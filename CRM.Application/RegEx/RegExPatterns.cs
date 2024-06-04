﻿namespace CRM.Application.RegEx
{
  public static class RegExPatterns
  {
    // Entity User
    public static string FirstName { get; } = "^[A-Za-z].{1,31}$";
    public static string LastName { get; } = "^[A-Za-z].{1,31}$";
    public static string FatherName { get; } = "^[A-Za-z].{1,31}$";
    public static string Password { get; } = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,50}$";
    public static string Gender { get; } = "^Male$|Female$";
    public static string PhoneNumber { get; } = @"^\+380\d\d\d\d\d\d\d\d\d$";
    public static string Email { get; } = @"^(?=.{5,30}$)([a-zA-Z0-9._%+-]+)@([a-zA-Z0-9.-]+)\.([a-zA-Z]{2,})$";
    public static string Post { get; } = "^Admin$|Manager$|Worker$";

    // Entity ProductCategory, Product, AddOn
    public static string ProductName { get; } = "^.{2,75}$";

    // URL
    public static string URL { get; } = "^.{5,75}$";
  }
}
