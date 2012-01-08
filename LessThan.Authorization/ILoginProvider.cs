using System.Collections.Specialized;

namespace LessThan.Authorization
{
    public interface ILoginProvider
    {
        string Login(string callBackUrl);
        User CallBack(string callBackUrl, NameValueCollection queryParameters);
    }
}
