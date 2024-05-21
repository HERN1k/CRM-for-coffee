namespace LogLib.Types
{
  public interface ILoggerLib
  {
    void Write();
    void Write(object input);
    void WriteLog(string method);
    Task WriteWarningLog(string message);
    Task WriteErrorLog(Exception exception);
  }
}
