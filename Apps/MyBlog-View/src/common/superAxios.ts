import axios from "axios";
import { cookieHelper } from "../common/CookieHelper";

class AxiosHelper {
  private baseUrl: string;

  //注意：这里是构造函数
  constructor() {
    this.baseUrl = "/";
    //这个表示容许请求时候携带cookie信息，因为后端有些方法是受到权限验证的，需要携带jwt的token验证才能调用方法
    //这里就是refreshToken需要，所以需要设置为true，这里设置后后端也需要设置才能接收，同时只有hearders像上面那样设置好才行
    //具体cookie的值是在getCookie()这个方法中去查看
    //因为这个类是单例的，所以构造函数只会执行一次，我们希望每次访问都会在头部请求放值所以就写了一个方法然后在UseWebAPI执行
    axios.defaults.withCredentials = true;

    //限制访问次数，仿制无限循环访问
    let i = 0;
    //拦截器每次http访问都会执行，这里是设置拦截器执行内容
    //这里是响应拦截器(其中形参接收两个委托，第一个是访问成功执行，第二个是失败执行的，这里我们只用第一个去接受后端返回的错误代码)
    axios.interceptors.response.use(async (res: any) => {

      //这里是当发现错误为203的时候，表示当前token已经过期了，我们需要去刷新token
      //于是先访问RefreshToken获得刷新用的token,如果不为null,则去访问之前获取token的方法去重新获取token
      if (res.status == 203) {
        if (i++ <= 0) {
          var rToken = await (
            await this.useWebAPI()
          ).get({
            url: "user/User/RefreshToken",
          });
          if (rToken != null) {
            cookieHelper.setCookie("token", rToken.data);
            console.log(rToken.data);
            //这里设置为null是因为下面需要获取到刚刚访问的url去再次访问
            this.baseUrl = "";
            await this.getCookie();
            //重新发送一个GET请求到之前失败的请求URL。
            //具体来说，当你的token过期并且你成功获取了一个新的token后，
            //你会重新尝试访问之前因为token过期而失败的URL
            await this.get({
              url: res.request.responseURL,
            });
          }
        }
      }
      return res;
    });
  }

  async useWebAPI(apiRootKey: string = "Default") {
    await this.getCookie();
    this.baseUrl = await this.getValue(apiRootKey);
    return this;
  }

  async useLocal() {
    this.baseUrl = await this.getValue("Local");
    return this;
  }

  async post(data: any) {
    var res = await axios.post(`${this.baseUrl}${data.url}`, data.params);
    return res;
  }

  // 'Content-Type': 'multipart/form-data' 的作用是告诉服务器客户端发送的是一个多部分表单数据请求，
  // 其中可以包含文件和其他字段。这种格式适用于上传文件时使用，而普通的 POST 请求通常使用
  // application/json 或 application/x-www-form-urlencoded
  async postUpload(data: any) {
    var res = await axios.post(`${this.baseUrl}${data.url}`, data.params, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return res;
  }

  async get(data: any) {
    var res = await axios.get(`${this.baseUrl}${data.url}`, {
      params: data.params,
    });
    return res;
  }

  async put(data: any) {
    var res = await axios.put(`${this.baseUrl}${data.url}`, data.params);
    return res;
  }

  async deleted(data: any) {
    var res = await axios.delete(`${this.baseUrl}${data.url}`, {
      params: data.params,
    });
    return res;
  }

  async getCookie() {
    var token = cookieHelper.getCookie("token");
    axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  }

  async getValue(
    key: string,
    section: "ApiRoots" | "WebUrl" = "ApiRoots",
    defaultValue: string = ""
  ): Promise<string> {
    const resStr = sessionStorage.getItem("appsettings");
    if (resStr === null) {
      try {
        const res = await axios.get("/appsettings.json");
        sessionStorage.setItem("appsettings", JSON.stringify(res.data));
        return res.data[section][key] ?? defaultValue;
      } catch (error) {
        console.error("Error fetching appsettings.json:", error);
        return defaultValue;
      }
    } else {
      const res = JSON.parse(resStr);
      return res[section][key] ?? defaultValue;
    }
  }
}

export const superAxios = (function () {
  return new AxiosHelper();
})();
