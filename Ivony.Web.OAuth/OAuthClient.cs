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
  public abstract class OAuthClient
  {
    protected abstract string ClientID { get; }
    protected abstract string SecurityKey { get; }



    protected HttpClient HttpClient { get; private set; }

    protected OAuthClient()
    {
      HttpClient = new HttpClient();
    }


    protected async Task<JObject> GetAccessToken( string url, string code, string redirectUri )
    {
      var data = new FormUrlEncodedContent( new Dictionary<string, string>
      { 
        { "grant_type","authorization_code"},
        { "code", code},
        { "redirect_uri", redirectUri},
      } );


      var request = CreatePostRequestWithBasicAuthorization( url, data );
      var response = await HttpClient.SendAsync( request );

      if ( response.StatusCode != HttpStatusCode.OK )
        throw new Exception();


      var result = JObject.Parse( await response.Content.ReadAsStringAsync() );

      AccessToken = result["access_token"].Value<string>();
      return result;
    }



    protected string AccessToken { get; set; }

    protected HttpRequestMessage CreatePostRequestWithBasicAuthorization( string url, FormUrlEncodedContent data )
    {
      var request = new HttpRequestMessage( HttpMethod.Post, url );
      request.Content = data;
      request.Headers.Authorization = new AuthenticationHeaderValue( "Basic", ToBase64String( ClientID + ":" + SecurityKey ) );

      return request;
    }

    protected HttpRequestMessage CreateRequestWithBearerAuthorization( string url, string accessToken = null )
    {
      var request = new HttpRequestMessage( HttpMethod.Get, url );
      request.Headers.Authorization = new AuthenticationHeaderValue( "Bearer", accessToken ?? AccessToken );

      return request;
    }

    protected static string ToBase64String( string data )
    {
      return Convert.ToBase64String( Encoding.UTF8.GetBytes( data ) );
    }

  }
}
