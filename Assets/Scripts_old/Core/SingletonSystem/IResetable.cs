
/// <summary>
/// Implement this interface on the TaleSingleton<> and it will be invoked when we do Relogin or when the App is Quitting.
///
/// Make sure that the implementation is full proof and can be invoked more than once.
///
/// Generally good stuff to reset is calling cancle on Task Cancellation Tokens. Making initial values default again
/// and making null anything that should be null, leaving no data from previous runs available.
/// </summary>
public interface IResetable
{
    //Unity has a function called Reset() I would have used that name, oh well
    void ResetAppLogic();
}
