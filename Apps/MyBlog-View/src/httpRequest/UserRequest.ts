import { superAxios } from "@/common/superAxios";

export const getActiveAdminUsersAsync = async (params: IGetActiveUsersParans) => {
    var res = await (
      await superAxios.useWebAPI()
    ).get({
      url: "MyBlogView/User/GetActiveAdminUser",
      params,
    });
    return res.data;
  };

  export const getUserInfoAsync = async () => {
    var res = await (
      await superAxios.useWebAPI()
    ).get({
      url: "MyBlogView/User/GetUserInfo",
    });
    return res.data;
  };

  interface IGetActiveUsersParans {
    pageIndex: number;
    pageSize: number;
  }
  