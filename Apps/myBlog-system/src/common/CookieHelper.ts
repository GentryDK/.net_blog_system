// Date：这是JavaScript的内置对象，用于处理日期和时间。你可以使用它来创建日期对象，获取当前日期和时间，以及进行日期和时间的计算。
// window.document.cookie：这是浏览器提供的Web API，用于操作cookie。你可以使用它来读取、写入和删除cookie

class CookieHelper {
  setCookie = (key: string, value: any, keepTime: any = null) => {
    //登录时间
    var loginTime = new Date();
    if (keepTime != null) {
      //设置保存时间
      keepTime = keepTime == null ? "999" : keepTime;
      loginTime.setTime(loginTime.getTime() + 24 * 60 * 60 * 1000 * keepTime);
      var expires = "expires" + loginTime.toUTCString();
      window.document.cookie = `${key}=${value};${expires};domain=dkedu.club;path=/`;
    } else {
      //domain是设置该cookie所在的域名，这里我们想让这个token在bbssystem的网站下，所以我们更改了域名
      //默认是在当前网页下
      window.document.cookie = `${key}=${value};domain=dkedu.club;path=/`;
    }
  };

  //获取到cookie中的token信息(只获取到第一条的value)
  getCookie = (key: string) => {
    var cookieValue = null;
    //document 是一个内置对象，代表了当前的HTML文档。这个对象是 window 对象的一部分，因此你可以直接使用 document，也可以使用 window.document，两者是等价的
    if (document.cookie.length > 0) {
      let cArr = window.document.cookie.split(";");
      for (let i = 0; i < cArr.length; i++) {
        const newArr = cArr[i].split("=");
        if (newArr[0].trim() === key) {
          cookieValue = newArr[1].trim();
          break;
        }
      }
    }
    return cookieValue;
  };

  deleteCookie = (key: string) => {
    window.document.cookie = `${key}=; Max-Age=0; path=/`;
  };
}

export const cookieHelper = (function () {
  return new CookieHelper();
})();
