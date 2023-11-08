namespace Mango.Web.Service.IService
{
    public interface ITokenProvider
    {
        // with cookies
        void SetTokent(string token);
        string? GetToken();
        void CleanToken();
    }
}
