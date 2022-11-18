using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mastodon;
using Mastodon.Model;
using Mastodon.Api;
using Mastodon.Common;
using Newtonsoft.Json;

public class MastoSlinger : MonoBehaviour
{

    //create and implement a slinger interface that conveys game moves
    void Start()
    {
        ExampleGame();
        //doMastoThings();
    }

    public void ExampleGame()
    {
        var state = new History(2);
        Debug.Log(state.ToString());
        state.LogMove(new Homeworld(new Pyramid(1, Color.Blue), new Pyramid(3, Color.Green), Color.Yellow, 1));
        Debug.Log(state.ToString());
    }

   public async void doMastoThings()
    {
        var domain = "botsin.space";
        var clientName = "Mastomeworlds";
        var userName = "ender";
        var password = "zugzwang";

        var scopes = new[] { Scope.Read, Scope.Write, Scope.Follow };

        PlayerPrefs.DeleteKey("registration");
        PlayerPrefs.DeleteKey("token");

        if (!PlayerPrefs.HasKey("registration") )
        {

            var saveauth = await Apps.Register(domain, clientName, scopes: scopes);

            
            string saveReg = JsonConvert.SerializeObject(saveauth);

            PlayerPrefs.SetString("registration", saveReg);
        }
        var oauth = JsonConvert.DeserializeObject<AppRegistration>(PlayerPrefs.GetString("registration"));
        if (!PlayerPrefs.HasKey("token"))
        {
            var url = "https://"+ domain + $"/oauth/authorize?response_type=code&client_id={oauth.ClientId}&redirect_uri={oauth.RedirectUri}&scope={oauth.Scope}";
            Debug.Log(url);
            UnityEngine.Application.OpenURL(url);
            Debug.Log("launched?");
            return;
            //OAuth.GetAccessTokenByCode(domain, oauth.ClientId, oauth.ClientSecret, oauth.RedirectUri, "", scopes);

            var firstoken = await OAuth.GetAccessTokenByPassword(domain, oauth.ClientId, oauth.ClientSecret, userName, password, scopes);

            var saveToke = JsonConvert.SerializeObject(firstoken);

            PlayerPrefs.SetString("token", saveToke);
        }
        return;
        Auth token = JsonConvert.DeserializeObject<Auth>(PlayerPrefs.GetString("token"));
        var timeline = await Timelines.Home(domain, token.AccessToken);
        var notify = await Notifications.Fetching(domain, token.AccessToken);
        var toot = await Statuses.Posting(domain, token.AccessToken, "Toot!");
        Debug.Log("Done!");

        

        //var authClient = new Mastodon.Model.AppRegistration("mastodon.gamedev.place");
        //var appRegistration = await authClient.CreateApp("Your app name", Scope.Read | Scope.Write | Scope.Follow);
    }
}
