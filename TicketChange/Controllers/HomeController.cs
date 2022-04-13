using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Globalization;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Web;
using TicketChange.Models;
using Newtonsoft.Json.Linq;

namespace TicketChange.Controllers
{
    public class HomeController : Controller
    {
        private TicketChangeEntities db = new TicketChangeEntities();
        string websiteName = "TicketTok!";

        public ActionResult Index() //首頁
        {
            var topsixEvents = (from detail in db.t_Event
                                select detail).Take(6);
            ViewBag.websiteName = websiteName;
            return View(topsixEvents);
        }
        public ActionResult About() //
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //對話輸入資料庫
        public void SetChatText(int roomID, string chatText)
        {
            int talkerID = Convert.ToInt32(Session["AccountData"].GetType().GetProperty("id").GetValue(Session["AccountData"]));
            db.ChatTexts.Add(new ChatTexts { RoomID = roomID, TalkerID = talkerID, ChatText = chatText });
            db.SaveChanges();
        }
        //顯示聊天室
        public ActionResult GetChatRoom()
        {
            JObject Rooms = new JObject();
            int talkerID = Convert.ToInt32(Session["AccountData"].GetType().GetProperty("id").GetValue(Session["AccountData"]));
            var rooms = db.ChatRoom.Where(r => r.TalkerID == talkerID);
            foreach (var room in rooms)
            {
                if (room.OnCall)
                {
                    List<int> talkersID = db.ChatRoom.Where(r => r.RoomID == room.RoomID).Select(r => r.TalkerID).ToList();
                    Rooms.Add(new JProperty("RoomName", String.Join("、", talkersID) + "的聊天室"));
                    Rooms.Add(new JProperty("RoomID", room.RoomID));
                }
            }
            return Json(Rooms, JsonRequestBehavior.AllowGet);
        }
        //取得聊天內容
        public ActionResult GetChatTexts()
        {

            var textsdata = new JObject()
            {
                new JProperty("",""),
                new JProperty("",""),
                new JProperty("",""),
                new JProperty("",""),
                new JProperty("","")
            };
            return Json(textsdata, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Concert() //演唱會
        //{

        //    var totalEvents = from detail in db.t_Event
        //                      select detail;

        //    ViewBag.websiteName = websiteName;
        //    return View(totalEvents);
        //}

        [HttpGet]
        public ActionResult Concert(string searchString) //演唱會
        {

            var result = from detail in db.t_Event
                         select detail;

            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(x => x.Subject.Contains(searchString));
            }
            ViewBag.websiteName = websiteName;
            return View(result);
        }

        public ActionResult Calendar() //行事曆
        {
            ViewBag.websiteName = websiteName;
            ViewBag.Message = "Your application description page.";
            var calendarlist = db.t_Event.Select(x => x).ToList();
            JArray Jcal = new JArray();
            foreach (var calendar in calendarlist)
            {
                if (calendar.DateEnd != null)
                {
                    DateTime start = (DateTime)calendar.DateStart;
                    DateTime end = (DateTime)calendar.DateEnd;
                    TimeSpan ts = end.Subtract(start);
                    decimal tsCount = ts.Days;
                    if (tsCount >= 1)
                    {
                        Jcal.Add(new JObject(
                        new JProperty("title", calendar.Subject),
                         new JProperty("start", calendar.DateStart),
                         new JProperty("end", (DateTime)(calendar.DateEnd == null ? calendar.DateStart : calendar.DateEnd)),
                          new JProperty("description", calendar.Description),
                          new JProperty("color", "SteelBlue") //一天以上(音樂祭/音樂節)，藍色條
                  ));
                    }
                    else if (calendar.DateEnd != null)
                    {
                        Jcal.Add(new JObject(
                           new JProperty("title", calendar.Subject),
                           new JProperty("start", calendar.DateStart),
                           new JProperty("end", (DateTime)(calendar.DateEnd == null ? calendar.DateStart : calendar.DateEnd)),
                           new JProperty("description", calendar.Description),
                           new JProperty("color", "FireBrick") //當天結束，知道結束時間，紅色
                        ));
                    }
                }
                else
                {
                    Jcal.Add(new JObject(
                        new JProperty("title", calendar.Subject),
                        new JProperty("start", calendar.DateStart),
                        new JProperty("end", (DateTime)(calendar.DateEnd == null ? calendar.DateStart : calendar.DateEnd)),
                        new JProperty("description", calendar.Description),
                        new JProperty("color", "YellowGreen") //當天結束，結束時間為null，綠色
                     ));
                }

            }
            string SJcal = JsonConvert.SerializeObject(Jcal);
            ViewBag.Jcal = SJcal;
            return View();

        }
        public ActionResult About_Test() //關於我們
        {
            ViewBag.websiteName = websiteName;
            return View();
        }

        public ActionResult Concert_Detail(int id) //
        {

            ////精靈模型

            //var events = from detail in db.t_Event
            //             where detail.EventID == id
            //             select detail;

            var events = db.t_Event.Where(e => e.EventID == id).FirstOrDefault();

            ViewBag.websiteName = websiteName;
            return View(events);
        }
        public ActionResult Image(string title) //抓資料庫的照片
        {

            var imgstring = db.t_Event.Where(x => x.Subject == title).Select(x => x.Image).FirstOrDefault();
            Object jj = new { IMG = imgstring };
            return Json(jj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Chat()
        {


            return PartialView();
        }


    }
}