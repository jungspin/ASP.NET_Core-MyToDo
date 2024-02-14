using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyToDo.Controllers.Filter
{
    /**
     * 클라이언트와 연결이 끊어진 경우 로그인페이지로 되돌리기 위한 필터
     */
    public class CheckSession : ActionFilterAttribute, IActionFilter
    {

        public static readonly string MySession = "MySession";
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var check = context.HttpContext;
            if (check.Session.GetString(MySession) == null)
            {
                // 이때 안내창 뜨면 좋겠는데
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "Login",
                    Controller = "Member",
                })) ;
            }
        }
    }
}
