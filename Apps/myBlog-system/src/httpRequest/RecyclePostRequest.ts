import { superAxios } from "@/common/superAxios";

export const getRecycleBinPostAsync = async (
  params: IGetRecycleBinPostAsync
) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlog/RecyclePost/GetRecycleBinPost",
    params,
  });
  return res.data;
};

export const PermanentlyDeletePostAsync = async (params: string) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: `MyBlog/RecyclePost/PermanentlyDeletePost?postId=${params}`,
  });
  return res;
};

export const recoverDeletedPostAsync = async (params: string) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: `MyBlog/RecyclePost/RecoverDeletedPost?postId=${params}`,
  });
  return res;
};

export const recoverDeletedPost1Async = async (postId: string) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: `MyBlog/RecyclePost/RecoverDeletedPost`,
    params: { postId },
  });
  return res;
};

interface IGetRecycleBinPostAsync {
  pageIndex: number;
  pageSize: number;
  postTitle?: string;
}
