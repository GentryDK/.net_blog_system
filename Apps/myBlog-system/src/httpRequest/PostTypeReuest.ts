import { superAxios } from "@/common/superAxios";

//获取全部帖子类型
export const getPostTypesAsync = async (params: IGetPostTypesParams) => {
  console.log(params);

  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlog/PostType/GetPostTypes",
    params,
  });
  return res.data;
};

export const getPostTypeCountAsync = async () => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlogView/PostType/GetPostTypeCount",
  });
  return res.data;
};

//添加新的帖子类型
export const addPostTypeAsync = async (params: IAddPostTypeParans) => {
  const formData = new FormData();
  if (params.CoverFile) {
    formData.append("CoverFile", params.CoverFile);
  }
  formData.append("PostTypeId", params.PostTypeId);
  formData.append("PostTypeName", params.PostTypeName);
  formData.append("Cover", params.Cover);
  formData.append("TypeBrief", params.TypeBrief);

  const res = await (
    await superAxios.useWebAPI()
  ).postUpload({
    url: "MyBlog/PostType/AddPostType",
    params: formData,
  });

  return res.data;
};

export const updatePostTpyeOrder = async (params: any) => {
  var res = await (
    await superAxios.useWebAPI()
  ).post({
    url: "MyBlog/PostType/UpdatePostTypeOrder",
    params,
  });
  return res.data;
};

export const removePostType = async (sid: string) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: `MyBlog/PostType/RemovePostTpye?postTypeId=${sid}`,
  });
  return res.data;
};

interface IAddPostTypeParans {
  PostTypeId: string;
  PostTypeName: string;
  Cover: string;
  TypeBrief: string;
  CoverFile: File;
}

interface IGetPostTypesParams {
  pageIndex: number;
  pageSize: number;
}
