﻿using CRM.Core.Contracts.RestDto;

namespace CRM.Application.Types
{
  public interface IAuthRecoveryService
  {
    void ValidationDataRecoveryPass(RecoveryPasswordRequest request);
    Task СomparisonRecoveryPassData(RecoveryPasswordRequest request);
    string GetNewPassword(int length);
    Task SendRecoveryPassEmail(string password);
    Task SaveNewPassword(string password);
    Task RemoveRefreshToken();
  }
}
