import { superAxios } from "@/common/superAxios";

export const getPostsAsync = async (params: IGetPostsParams) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlog/Post/GetPosts",
    params,
  });
  return res.data;
};

//添加新的帖子
export const addPostAsync = async (params: IAddPostParans) => {
  const formData = new FormData();
  if (params.CoverFile) {
    formData.append("CoverFile", params.CoverFile); // 使用大写属性名
  }
  if (params.Id) {
    formData.append("Id", params.Id);
  }
  formData.append("PostTitle", params.PostTitle);
  formData.append("summary", params.summary);
  formData.append("PostContent", params.PostContent);
  formData.append("PostTypeId", params.PostTypeId);
  formData.append("PostTypeName", params.PostTypeName);
  formData.append("Discuss", params.Discuss);
  formData.append("State", params.State.toString());
  const res = await (
    await superAxios.useWebAPI()
  ).postUpload({
    url: "MyBlog/Post/AddPost",
    params: formData,
  });
  return res.data;
};

export const setPostPictureAsync = async (param: File) => {
  const formData = new FormData();
  formData.append("pictureFile", param);
  const res = await (
    await superAxios.useWebAPI()
  ).postUpload({
    url: "MyBlog/Post/Picture",
    params: formData,
  });
  return res.data;
};

export const getTotalPostsAsync = async () => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlog/Post/TotalPost",
  });
  return res.data;
};

export const autoSaveAsync = async (params: any) => {
  const formData = new FormData();
  formData.append("Id", params.Id || "");
  formData.append("PostTitle", params.PostTitle || "");
  formData.append("summary", params.summary || "");
  formData.append("PostContent", params.PostContent || "");
  formData.append("PostTypeId", params.PostTypeId || "");
  formData.append("PostTypeName", params.PostTypeName || "");
  formData.append("Discuss", params.Discuss || "");
  formData.append("State", params.State?.toString() || ""); // 确保 State 不为 undefined

  var res = await (
    await superAxios.useWebAPI()
  ).postUpload({
    url: "MyBlog/Post/AutoSave",
    params: formData, // 使用 FormData 传递数据
  });
  return res.data;
};

export const removePostAsync = async (pId: string) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: `MyBlog/Post/RemovePost?postId=${pId}`,
  });
  return res.data;
};

interface IAddPostParans {
  Id: string;
  PostTitle: string;
  summary: string;
  PostContent: string;
  PostTypeId: string;
  PostTypeName: string;
  Discuss: string;
  State: number;
  CoverFile: File;
}

interface IGetPostsParams {
  pageIndex: number;
  pageSize: number;
  PostTitle?: string;
  State?: number;
  PostTypeId?: string;
}
