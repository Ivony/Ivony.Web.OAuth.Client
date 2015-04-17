using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Web.OAuth
{

  /// <summary>
  /// 提供 OAuth 2.0 协议支持
  /// </summary>
  public abstract class OAuthClient
  {
    /// <summary>
    /// 客户端 ID
    /// </summary>
    protected abstract string ClientID { get; }
    
    /// <summary>
    /// 客户端加密字符串
    /// </summary>
    protected abstract string SecurityKey { get; }



    protected HttpClient HttpClient { get; private set; }

    protected OAuthClient()
    {
      HttpClient = new HttpClient();
    }




    private string accessToken;

    /// <summary>
    /// 获取或设置 OAuth 访问标识
    /// </summary>
    protected string AccessToken
    {
      get { return accessToken; }
      set
      {
        if ( accessToken != null )
          throw new InvalidOperationException();

        accessToken = value;
      }
    }


    protected async Task<HttpResponseMessage> PostWithBasicAuthorization( string url, HttpContent content )
    {
      return await HttpClient.SendAsync( CreatePostRequestWithBasicAuthorization( url, content ) );
    }



    /// <summary>
    /// 创建一个 POST 请求，使用基本身份验证
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="data">请求数据</param>
    /// <returns>请求对象</returns>
    protected HttpRequestMessage CreatePostRequestWithBasicAuthorization( string url, HttpContent data )
    {
      var request = new HttpRequestMessage( HttpMethod.Post, url );
      request.Content = data;
      request.Headers.Authorization = new AuthenticationHeaderValue( "Basic", ToBase64String( ClientID + ":" + SecurityKey ) );

      return request;
    }

    protected HttpRequestMessage CreateRequestWithBearerAuthorization( string url, string accessToken = null )
    {
      var request = new HttpRequestMessage( HttpMethod.Get, url );
      ApplyAccessToken( request, accessToken );

      return request;
    }


    /// <summary>
    /// 在 HTTP 请求上附加访问标识
    /// </summary>
    /// <param name="request">HTTP 请求对象</param>
    protected void ApplyAccessToken( HttpRequestMessage request, string accessToken )
    {
      request.Headers.Authorization = new AuthenticationHeaderValue( "Bearer", accessToken ?? AccessToken );
    }


    protected static string ToBase64String( string data )
    {
      return Convert.ToBase64String( Encoding.UTF8.GetBytes( data ) );
    }

  }
}
