using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TicketChange.Models;


namespace TicketChange.Controllers
{
    public class ForumController : Controller
    {

        private TicketChangeEntities db = new TicketChangeEntities();
        public DateTime CreateTime { get; set; }

        #region 文章列表 List
        public ActionResult ArticleList(int aid = 1)
        {
            // _ID變數，目前位於第幾頁？
            // PageSize變數，每一頁，要展示幾筆記錄？            
            int PageSize = 5;

            // RecordCount變數，符合條件的總共有幾筆記錄？
            int RecordCount = db.Article.Count();

            // NowPageCount，目前正在觀賞這一頁的紀錄
            int NowPageCount = 0;
            if (aid > 0)
            {
                NowPageCount = (aid - 1) * PageSize;    // PageSize，每頁展示5筆紀錄（上面設定過了）
            }
            var ListAll = (from art in db.Article
                           join us in db.User on art.ID equals us.ID
                           orderby art.ArtID // 若寫 descending ，則是反排序（由大到小）
                           select art).Skip(NowPageCount).Take(PageSize);    // .Skip() 從哪裡開始（忽略前面幾筆記錄）。 .Take()呈現幾筆記錄

            if (ListAll == null)
            {   // 找不到任何記錄
                return HttpNotFound();
            }
            else
            {

                #region    

                // Pages變數，「總共需要幾頁」才能把所有紀錄展示完畢？
                int Pages;
                if ((RecordCount % PageSize) > 0)
                {   //-- %，除法，傳回餘數
                    Pages = ((RecordCount / PageSize) + 1);   //-- ( / )除法。傳回整數。  如果無法整除，有餘數，則需要多出一頁來呈現。 
                }
                else
                {
                    Pages = (RecordCount / PageSize);   //-- ( /)除法。傳回整數
                }


                System.Text.StringBuilder sbPageList = new System.Text.StringBuilder();
                if (Pages > 0)
                {   //有傳來「頁數(p)」，而且頁數正確（大於零），出現<上一頁>、<下一頁>這些功能
                    sbPageList.Append("<div id='pageBtn' align='center'>");

                    if (aid > 1)
                    {   //======== 分頁功能（上一頁 / 下一頁）=========start===                
                        sbPageList.Append("<a href='?aid=" + (aid - 1) + "'><i class='fas fa-angle-left'></i></a>" + "&nbsp;&nbsp;&nbsp;");
                    }

                    int block_page = 0;
                    block_page = aid / 10;   //--只取除法的整數成果（商），若有餘數也不去管它。


                    for (int K = 0; K <= 10; K++)
                    {
                        if ((block_page * 10 + K) <= Pages)
                        {   //--- Pages 資料的總頁數。共需「幾頁」來呈現所有資料？
                            if (((block_page * 10) + K) == aid)
                            {   //--- id 就是「目前在第幾頁」
                                sbPageList.Append("<b>" + aid + "</b>" + "&nbsp;&nbsp;&nbsp;");
                            }
                            else
                            {
                                if (((block_page * 10) + K) != 0)
                                {
                                    sbPageList.Append("<a href='?aid=" + (block_page * 10 + K) + "'>" + (block_page * 10 + K) + "</a>");
                                    sbPageList.Append("&nbsp;&nbsp;&nbsp;");
                                }
                            }
                        }
                    }  //for迴圈 end

                    if (aid < Pages)
                    {
                        sbPageList.Append("<a href='?aid=" + (aid + 1) + "'><i class='fas fa-angle-right'></i></a>");
                    }

                }
                #endregion

                ViewBag.PageList = sbPageList.ToString();


                return View(ListAll.ToList());
            }


        }
        #endregion

        #region 最新排序
        public ActionResult NewestList(int aid = 1)
        {
            int PageSize = 5;

            // RecordCount變數，符合條件的總共有幾筆記錄？
            int RecordCount = db.Article.Count();

            // NowPageCount，目前正在觀賞這一頁的紀錄
            int NowPageCount = 0;
            if (aid > 0)
            {
                NowPageCount = (aid - 1) * PageSize;
            }

            var ListAll = (from art in db.Article
                           join us in db.User on art.ID equals us.ID
                           orderby art.ArtTime descending // 若寫 descending ，則是反排序（由大到小）
                           select art).Skip(NowPageCount).Take(PageSize);    // .Skip() 從哪裡開始（忽略前面幾筆記錄）。 .Take()呈現幾筆記錄

            if (ListAll == null)
            {   // 找不到任何記錄
                return HttpNotFound();
            }
            else
            {

                #region    // 畫面下方的「分頁列」。「每十頁」一間隔，分頁功能

                // Pages變數，「總共需要幾頁」才能把所有紀錄展示完畢？
                int Pages;
                if ((RecordCount % PageSize) > 0)
                {   //-- %，除法，傳回餘數
                    Pages = ((RecordCount / PageSize) + 1);   //-- ( / )除法。傳回整數。  如果無法整除，有餘數，則需要多出一頁來呈現。 
                }
                else
                {
                    Pages = (RecordCount / PageSize);   //-- ( /)除法。傳回整數
                }



                System.Text.StringBuilder sbPageList = new System.Text.StringBuilder();
                if (Pages > 0)
                {
                    sbPageList.Append("<div id='pageBtn' align='center'>");

                    if (aid > 1)
                    {
                        sbPageList.Append("<a href='?aid=" + (aid - 1) + "'><i class='fas fa-angle-left'></i></a>" + "&nbsp;&nbsp;&nbsp;");
                    }

                    int block_page = 0;
                    block_page = aid / 10;


                    for (int K = 0; K <= 10; K++)
                    {
                        if ((block_page * 10 + K) <= Pages)
                        {   //--- Pages 資料的總頁數。共需「幾頁」來呈現所有資料？
                            if (((block_page * 10) + K) == aid)
                            {   //--- id 就是「目前在第幾頁」
                                sbPageList.Append("<b>" + aid + "</b>" + "&nbsp;&nbsp;&nbsp;");
                            }
                            else
                            {
                                if (((block_page * 10) + K) != 0)
                                {
                                    sbPageList.Append("<a href='?aid=" + (block_page * 10 + K) + "'>" + (block_page * 10 + K) + "</a>");
                                    sbPageList.Append("&nbsp;&nbsp;&nbsp;");
                                }
                            }
                        }
                    }  //for迴圈 end

                    if (aid < Pages)
                    {
                        sbPageList.Append("<a href='?aid=" + (aid + 1) + "'><i class='fas fa-angle-right'></i></a>");
                    }

                }

                #endregion

                ViewBag.PageList = sbPageList.ToString();

                return View("ArticleList", ListAll.ToList());
            }



        }

        #endregion

        #region 文章分類

        public ActionResult ArticleCategoery(int? cat)
        {
            List<Article> articles = (from i in db.Article
                                      join us in db.User on i.ID equals us.ID
                                      orderby i.ArtID
                                      where i.ArtCategory == cat
                                      select i).ToList();

            return View("ArticleList", articles);

        }

        #endregion

        #region 文章搜尋

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ArticleList(Article art)
        {
            string aTitle = art.ArtTitle;
            ViewData["sw"] = aTitle;

            var ListAll = db.Article.Select(x => x);
            if (!string.IsNullOrWhiteSpace(aTitle))
            {
                ListAll = ListAll.Where(x => x.ArtTitle.Contains(aTitle));
            }

            //if ((art != null) && (ModelState.IsValid))
            //{
            //    return View("ArticleList", ListAll.ToList());
            //}
            //else
            //{
            //    return HttpNotFound();
            //}

            if ((art != null) && (ModelState.IsValid))
            {
                return View("ArticleList", ListAll.ToList());
            }
            else
            {
                return HttpNotFound();
            }

        }
        #endregion

        #region 新增文章 Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(ForDetial article)
        {

            if ((article != null) && (ModelState.IsValid))
            {

                // add article
                article.ArtTime = DateTime.Now;
                Article newArt = article.GetArticle();
                db.Article.Add(newArt);
                db.SaveChanges();

                //using (var context = new ForumEntities())
                //{
                //    context.Article.Add(article);
                //    context.SaveChanges();
                //    int id = article.ArtID; // Yes it's here
                //}

                // 撈出文章id(在迴圈外面撈)
                int artID = newArt.ArtID;

                // get input tag's array 
                string[] tag = article.Tag.Split(',');

                bool check = false;  // check tag 是否存在的flag
                int tagID = 0;
                for (int i = 0; i < tag.Length; i++)
                {
                    var temp = tag[i];
                    //check tagName是否存在
                    check = CheckTagName(tag[i]);
                    // 不存在則新增，存在則跳過
                    if (!check)
                    {
                        
                        Tag modelTag = new Tag();
                        modelTag.TagContent = temp;
                        db.Tag.Add(modelTag);
                        db.SaveChanges();

                        tagID = modelTag.TagID;
                    }
                    else   // 撈出已存在的tag id
                    {
                        var togData = db.Tag.FirstOrDefault(r => r.TagContent == temp);
                        if (togData != null)
                        {
                            tagID = togData.TagID;
                        }
                        else
                        {

                        }
                    }

                    // 存到 Article_Tag
                    Article_Tag modelAT = new Article_Tag();
                    modelAT.ArtID = artID;
                    modelAT.TagID = tagID;
                    db.Article_Tag.Add(modelAT);
                    db.SaveChanges();

                }
                return RedirectToAction("ArticleList");
            }
            else
            {
                return View(article);
            }
        }
        public bool CheckTagName(string TagName)
        {
            return db.Tag.Any(u => u.TagContent == TagName);
        }
        //public ActionResult Create(string ArtTitle, string ArtContent, int ArtCategory)
        //{
        //    Article art = new Article();


        //    if ((art != null) && (ModelState.IsValid))
        //    {
        //        art.ArtTitle = ArtTitle;
        //        art.ArtContent = ArtContent;
        //        art.ArtTime = DateTime.Now;
        //        art.ArtCategory = ArtCategory;
        //        db.Article.Add(art);

        //        db.SaveChanges();
        //        return RedirectToAction("ArticleList");
        //    }
        //    else
        //    {
        //        return View(art);
        //    }

        //}

        //public ActionResult Create(Article article)
        //{

        //    if ((article != null) && (ModelState.IsValid))
        //    {
        //        article.ArtTime = DateTime.Now;
        //        db.Article.Add(article);
        //        db.SaveChanges();
        //        return RedirectToAction("ArticleList");
        //    }
        //    else
        //    {
        //        return View(article);
        //    }
        //}


        #endregion

        #region 標籤相關文章列表 ArticleTagList
        //[HttpPost] //應該要留
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        public ActionResult ArticleTagList(string TagContent)
        {
            int tagID;
            var tagIDCheck = db.Tag.Where(x => x.TagContent == TagContent).ToList();
            if (tagIDCheck.Count > 0)
            {
                tagID = db.Tag.Where(x => x.TagContent == TagContent).ToList()[0].TagID;
            }
            else
            {
                tagID = 0;
            }

            var ArticleList = (from a in db.Article_Tag
                               join o in db.Article on a.ArtID equals o.ArtID
                               where a.TagID == tagID
                               select o);
            //from c in Customerso.ArtTime,o.ArtCategory,ID=0}).ToList();
            //from c in Customers
            //join o in Orders on c.CustomerID equals o.CustomerID
            ViewBag.PageList = "";
            ViewBag.tagName = TagContent + " <span class='search-count'></span>";

            return View("ArticleList", ArticleList);

        }
        #endregion

        #region 文章內容 Detalis
        [HttpGet]
        public ActionResult Details(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            //}
            //var SinglePage = from i in db.Articles
            //              where i.ArtID == id
            //              select i;
            //if (SinglePage == null)
            //{
            //    return HttpNotFound();
            //}
            //else
            //{
            //    return View(SinglePage.FirstOrDefault());
            //}

            //var commentList = new List<Comment>();

            //    commentList = (from c in db.Comment

            //                  select c ).ToList();

            Article art = db.Article.Find(id);
            if (art == null)
            {
                return HttpNotFound();
            }
            var TagIDs = db.Article_Tag.Where(x => x.ArtID == art.ArtID).Select(a => a.TagID).ToList();
            string tagContext = String.Join(" ", db.Tag.Where(x => TagIDs.Contains(x.TagID)).Select(x => x.TagContent).ToList());
            User user = db.User.Find(art.ID);
            var cmts = db.Comment.Where(c => c.ArtID == id).ToList();

            return View(new ForDetial(art, tagContext, user, cmts));
        }


        #endregion

        #region 刪除文章 Delete
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteArt(int? aId)
        {

            var art = db.Article.Find(aId);
            db.Article.Remove(art);
            db.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        #endregion

        #region 編輯文章 Edit

        public ActionResult Edit(int aId)
        {
            var art = db.Article.Where(a => a.ArtID == aId).FirstOrDefault();
            List<int> tagID = db.Article_Tag.Where(at => at.ArtID == aId).Select(t => t.TagID).ToList();
            var tag = db.Tag.Where(t => tagID.Contains(t.TagID)).Select(t => t.TagContent).ToList();
            var finalArt = new ForDetial(art, String.Join(",", tag));
            return View(finalArt);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(ForDetial art)
        {
            Article Oart = db.Article.Find(art.ArtID);
            Oart.ArtTitle = art.ArtTitle;
            Oart.ArtContent = art.ArtContent;
            Oart.ArtCategory = art.ArtCategory;

            List<string> tags = art.Tag ==  null ? new List<string>() : art.Tag.Split(',').Distinct().ToList();
            foreach (var Oat in db.Article_Tag.Where(at => at.ArtID == art.ArtID))
            {
                db.Article_Tag.Remove(Oat);
            }
            db.SaveChanges();
            foreach (var tag in tags)
            {
                var OtagId = db.Tag.Where(t => t.TagContent == tag).Select(t => t.TagID);
                if (OtagId.Count() > 0)
                {
                    db.Article_Tag.Add(new Article_Tag { TagID = OtagId.First(), ArtID = art.ArtID });
                }
                else
                {
                    db.Tag.Add(new Tag { TagContent = tag });
                    db.SaveChanges();
                    var NewTagId = db.Tag.Where(t => t.TagContent == tag).Select(t => t.TagID);
                    db.Article_Tag.Add(new Article_Tag { TagID = NewTagId.First(), ArtID = art.ArtID });
                }
            }
            db.SaveChanges();
            return RedirectToAction("Details", new { id = art.ArtID });
        }

        //public ActionResult Edit(int? aId)
        //{
        //    //if (aId == null)
        //    //{
        //    //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    //}
        //    //Article art = db.Article.Find(aId);
        //    //if (art == null)
        //    //{
        //    //    return HttpNotFound();
        //    //}
        //    //return View("ArticleList");

        //    //var art = db.Article.Where(m => m.ArtID == aId).FirstOrDefault();
        //    //return View(art);

        //    if (aId == null)
        //    {
        //        //沒有輸入文章編號 就會報錯
        //        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        //    }

        //    Article art = db.Article.Find(aId);
        //    if (art == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    else
        //    {
        //        return View(art);
        //    }

        //}

        //[HttpPost, ActionName("Edit")]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //public ActionResult Edit(Article articles)
        //{
        //    //int aId = articles.ArtID;
        //    //var art = db.Article.Where(m => m.ArtID == aId).FirstOrDefault();
        //    //art.ArtTitle = articles.ArtTitle;
        //    //art.ArtContent = articles.ArtContent;
        //    //db.SaveChanges();
        //    //return RedirectToAction("Details", new {id = art.ArtID});
        //    if (articles == null)
        //    {
        //        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // 第一種
        //        //_db.Entry(_userTable).State = System.Data.Entity.EntityState.Modified;
        //        //_db.SaveChanges();

        //        // 第二種
        //        Article art = db.Article.Find(articles.ArtID);
        //        if (art == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        else
        //        {
        //            art.ArtTitle = articles.ArtTitle;
        //            art.ArtContent = articles.ArtContent;

        //            db.SaveChanges();
        //        }

        //        return RedirectToAction("Details", new { id = art.ArtID });
        //    }
        //    else
        //    {
        //        return View(articles);
        //    }

        //}

        #endregion

        #region 新增留言 Comment



        public ActionResult AddComment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddComment(int ArtId, string UserId, string ComContent,bool sendEmail=false)
        {
            var currentDateTime = DateTime.Now;
            var com = new Comment()
            {
                ArtID = ArtId,
                ComContent = ComContent,
                ComTime = currentDateTime,
                ID = Convert.ToInt32(UserId),
                SendEmail=sendEmail,
            };

            db.Comment.Add(com);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = ArtId });
        }

        #endregion


        #region 刪除留言 Delete
        public ActionResult DeleteCom(int? aId)
        {

            var art = db.Comment.Find(aId);
            db.Comment.Remove(art);
            db.SaveChanges();
            return RedirectToAction("Details");
        }
        #endregion


        public ActionResult Chat()
        {


            return PartialView();
        }

        //public ActionResult TagList()
        //{

        //    var result = (from i in db.Tag
        //                  select i).Take(10);

        //    return View(result);
        //}



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
    #region Model轉型
    public class ForDetial : Article
    {
        public Article GetArticle()
        {
            return new Article()
            {
                ArtTitle = ArtTitle,
                ArtTime = ArtTime,
                ArtContent = ArtContent,
                Article_Tag = Article_Tag,
                ArtCategory = ArtCategory,
                ArtID = ArtID,
                ID = ID
            };
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ForDetial()
        {
            this.Article_Tag = new HashSet<Article_Tag>();
        }
        public ForDetial(Article art, string tag = "", User user = null, List<Comment> cmts = null)
        {
            ArtID = art.ArtID;
            ArtTitle = art.ArtTitle;
            Article_Tag = art.Article_Tag;
            Tag = tag;
            ArtContent = art.ArtContent;
            ArtTime = art.ArtTime;
            ID = art.ID;
            ArtCategory = art.ArtCategory;
            User = user;
            Comments = cmts;
        }
        public string Tag { get; set; }
        public override User User { get; set; }
        public List<Comment> Comments { get; set; }
    }
    #endregion
}