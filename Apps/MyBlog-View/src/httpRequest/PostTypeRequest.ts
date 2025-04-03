import { superAxios } from "@/common/superAxios";

//获取全部帖子类型
export const getPostTypesAsync = async (params: IGetPostTypesParams) => {
    var res = await (
      await superAxios.useWebAPI()
    ).get({
      url: "MyBlogView/PostType/GetPostTypes",
      params,
    });
    return res.data;
  };

  export const getPostTypeAsync = async (postTypeId: string) => {
    var res = await (
      await superAxios.useWebAPI()
    ).get({
      url: "MyBlogView/PostType/GetPostType",
      params:{postTypeId},
    });
    return res.data;
  };

  export const getPostTypeCountAsync = async () => {
    var res = await (
      await superAxios.useWebAPI()
    ).get({
      url: "MyBlogView/PostType/GetPostTypeCount"
    });
    return res.data;
  };

  interface IGetPostTypesParams {
    pageIndex: number;
    pageSize: number;
  }