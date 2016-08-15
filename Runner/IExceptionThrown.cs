namespace Jobs.Runner
{
    public interface IExceptionThrown
    {
        #region events

        event JobExceptionThrownEventHandler ExceptionThrown;

        #endregion
    }
}