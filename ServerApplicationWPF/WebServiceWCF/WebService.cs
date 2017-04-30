using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceWCF
{
    public class WebService : IHttpHandler
    {
        public bool IsReusable { get { return true; } }
        public void ProcessRequest(HttpContext context)
        {
            Uri uri = context.Request.Url;
            // compare URI to resource templates and find match

            // figure out which HTTP method is being used
            switch (context.Request.HttpMethod)
            {
                // dispatch to internal methods based on URI and HTTP method
                // and write the correct response status & entity body
                case "GET":
                    HttpContext.Current.Response.Write("ciao");
                    break;
                case "POST":
                    /*Bookmark newBookmark = ReadBookmarkFromRequest(context.Request);
                    string id = CreateNewBookmark(username, newBookmark);
                    WriteLocationHeader(id);
                    SetResponseStatus(context.Response, "201", "Created");
                    */
                    break;
                default:
                    //HttpContext.Current.Response.
                    break;
            }
        }


    }
}