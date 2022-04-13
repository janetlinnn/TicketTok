using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TicketChange.Models;

namespace TicketChange.Controllers
{

    public class LoginController : Controller
    {
        #region 取得通知
        public ActionResult GetNotice()
        {
            int myID = Convert.ToInt32(Session["AccountData"].GetType().GetProperty("id").GetValue(Session["AccountData"]));
            JArray datas = new JArray();
            using (TicketChangeEntities db = new TicketChangeEntities()) {
                var articles = db.Article.Where(a => a.ID == myID);

                foreach (var article in articles)
                {
                    var comments = db.Comment.Where(c => c.ArtID == article.ArtID && !c.Read);
                    foreach (var comment in comments)
                    {
                        JObject data = new JObject();
                        data.Add(new JProperty("ID", comment.ID));
                        datas.Add(data);
                    }
                } 
            }
            return Json(datas,JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 帳戶資料修改
        public ActionResult Register()
        {
            ViewBag.websiteName = "帳戶資料修改";
            return View();
        }
        #endregion
        #region 我的文章
        public ActionResult MyArticle()
        {
            if (Session["AccountData"] == null) { return RedirectToAction("NewestList", "Forum"); }
            var userid = Convert.ToInt32(Session["AccountData"].GetType().GetProperty("id").GetValue(Session["AccountData"]).ToString());
            using (TicketChangeEntities db = new TicketChangeEntities())
            {
                return View(db.Article.Where(a=>a.ID==userid).OrderByDescending(a=>a.ArtTime).ToList());
            }
        }
        #endregion
        #region Google登入
        public ActionResult GoogleLoginDirect(string callback)
        {
            //跳轉至第三方API
            string LoginUrl = GetLoginUrl("https://accounts.google.com/o/oauth2/auth", new NameValueCollection
            {
                { "response_type", "code" },
                { "client_id", "877925943142-nd87r5sc0q4s2cjumiob069rf7vjhdrg.apps.googleusercontent.com" },
                { "redirect_uri", HttpUtility.UrlEncode("https://localhost:44326/Login/GoogleCallback") },
                { "state", callback },
                { "scope", "profile%20email" }
            });
            return Redirect(LoginUrl);
        }
        public ActionResult GoogleCallback(string code, string state)
        {
            WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };
            //取回令牌
            string ApiUrl_Token = "https://oauth2.googleapis.com/token";
            NameValueCollection nvc = new NameValueCollection
                {
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", "https://localhost:44326/Login/GoogleCallback" },
                    { "client_id", "877925943142-nd87r5sc0q4s2cjumiob069rf7vjhdrg.apps.googleusercontent.com" },
                    { "client_secret", "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" }
                };
            JObject ToKenObj = JObject.Parse(Encoding.UTF8.GetString(wc.UploadValues(ApiUrl_Token, "POST", nvc)));

            //使用令牌取回使用者資料
            string ApiUrl_Field = GetLoginUrl("https://www.googleapis.com/oauth2/v1/userinfo", new NameValueCollection
                {
                    { "fields", "id,email,name,picture" },
                    { "access_token", (string)ToKenObj["access_token"] }
                });
            JObject GoogleField = JObject.Parse(wc.DownloadString(ApiUrl_Field));

            //使用者資料匯入資料庫、Session
            InnerUserBase(
                "GOOGLE",
                GoogleField["name"].ToString(),
                GoogleField["email"].ToString(),
                GoogleField["picture"].ToString(),
                "~/img/LoginLogo/google.png");

            return Redirect(state);
        }
        #endregion
        #region FB登入
        public ActionResult FBLoginDirect(string callback)
        {
            //跳轉至第三方API
            string LoginUrl = GetLoginUrl("https://www.facebook.com/v13.0/dialog/oauth", new NameValueCollection
            {
                { "response_type", "code" },
                { "client_id", "1133671850724221" },
                { "redirect_uri", HttpUtility.UrlEncode("https://localhost:44326/Login/FBCallback") },
                { "state", callback }
            });
            return Redirect(LoginUrl);
        }
        public ActionResult FBCallback(string code, string state)
        {
            WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };
            //取回令牌
            string ApiUrl_Token = GetLoginUrl("https://graph.facebook.com/v13.0/oauth/access_token", new NameValueCollection
                {
                    { "code", code },
                    { "client_id", "1133671850724221" },
                    { "redirect_uri", HttpUtility.UrlEncode("https://localhost:44326/Login/FBCallback") },
                    { "client_secret", "a66c79cdd722265a8ded9a510163186e" },
                    { "scope", "email,public_profile" }
                });
            JObject ToKenObj = JObject.Parse(wc.DownloadString(ApiUrl_Token));

            //使用令牌取回使用者資料
            string ApiUrl_Field = GetLoginUrl("https://graph.facebook.com/me", new NameValueCollection
                {
                    { "fields", "id,name,email,picture,hometown,birthday" },
                    { "access_token", (string)ToKenObj["access_token"] }
                });
            JObject FacebookField = JObject.Parse(wc.DownloadString(ApiUrl_Field));

            //使用者資料匯入資料庫、Session
            InnerUserBase(
                "FB",
                FacebookField["name"].ToString(),
                FacebookField["email"].ToString(),
                FacebookField["picture"]["data"]["url"].ToString(),
                "~/img/LoginLogo/facebook.png");

            //跳轉回目前頁面
            return Redirect(state);
        }
        #endregion
        #region Line登入
        public ActionResult LineLoginDirect(string callback)
        {
            //跳轉至第三方API
            string LoginUrl = GetLoginUrl("https://access.line.me/oauth2/v2.1/authorize", new NameValueCollection
            {
                { "response_type", "code" },
                { "client_id", "1656974896" },
                { "redirect_uri", HttpUtility.UrlEncode("https://localhost:44326/Login/LineCallback") },
                { "state", callback },
                { "scope", "openid%20profile%20email" },
                { "nonce", "MSIT38" }
            });
            return Redirect(LoginUrl);
        }
        public ActionResult LineCallback(string code, string state)
        {
            WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };
            NameValueCollection nvc = new NameValueCollection();
            //取回令牌
            string ApiUrl_Token = "https://api.line.me/oauth2/v2.1/token";
            nvc.Add("grant_type", "authorization_code");
            nvc.Add("code", code);
            nvc.Add("redirect_uri", "https://localhost:44326/Login/LineCallback");
            nvc.Add("client_id", "1656974896");
            nvc.Add("client_secret", "ff28cd5965b254a70ca84573ba1fe6cd");
            JObject ToKenObj = JObject.Parse(Encoding.UTF8.GetString(wc.UploadValues(ApiUrl_Token, "POST", nvc)));
            nvc.Clear();

            //使用令牌取回使用者資料
            string ApiUrl_Verify = "https://api.line.me/oauth2/v2.1/verify";
            nvc.Add("id_token", ToKenObj["id_token"].ToString());
            nvc.Add("client_id", "1656974896");
            JObject LineField = JObject.Parse(Encoding.UTF8.GetString(wc.UploadValues(ApiUrl_Verify, "POST", nvc)));

            //使用者資料匯入資料庫、Session
            InnerUserBase(
                "Line",
                LineField["name"].ToString(),
                LineField["email"].ToString(),
                LineField["picture"].ToString(),
                "~/img/LoginLogo/line.png");

            return Redirect(state);
        }
        #endregion
        #region 登出
        public ActionResult Logout(string callback)
        {
            Session.Remove("AccountData");
            return Redirect(callback);
        }
        #endregion
        #region {{專用函式庫}} 網址串接GET參數:GetLoginUrl、基本資料匯入資料庫與SESSION:InnerUserBase
        private String GetLoginUrl(String uri, NameValueCollection nvc)
        {
            uri += "?";
            foreach (string key in nvc)
            {
                uri += key + "=" + nvc[key] + "&";
            }
            return uri.Substring(0, uri.Length - 1);
        }
        private void InnerUserBase(string provider, string name, string email, string picture, string loginMode)
        {
            using (TicketChangeEntities db = new TicketChangeEntities())
            {
                var userID = from x in db.User where x.Email == email select x.ID;

                if (userID.Count() == 0)
                {
                    db.Set<User>().Add(new User()
                    {
                        Name = name,
                        NickName = name,
                        Email = email,
                        Picture = picture,
                        LoginDate = DateTime.Now,
                        AddDate = DateTime.Now
                    });
                    db.SaveChanges();
                }

                if (db.LoginAPI.Where(x => x.APIprovider == provider && x.ID == userID.FirstOrDefault()).Count() == 0)
                {
                    db.Set<LoginAPI>().Add(new LoginAPI()
                    {
                        ID = userID.First(),
                        APIprovider = provider,
                    });
                }
                db.User.Find(userID.First()).LoginDate = DateTime.Now;
                db.SaveChanges();

                Session["AccountData"] = new
                {
                    id = userID.First(),
                    loginMode = loginMode,
                    name = name,
                    email = email,
                    pictureUrl = db.User.Where(x => x.ID == userID.FirstOrDefault()).Select(x => x.Picture).First().ToString(),
                    loginDate = db.User.Where(x => x.ID == userID.FirstOrDefault()).Select(x => x.LoginDate).First().ToString(),
                };
            }
        }
    }
    #endregion
}