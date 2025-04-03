import { superAxios } from "@/common/superAxios";

//获取帖子评论
export const getPostRepliesAsync = async (params: IGetPostRepliesParams) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlog/Reply/GetReplies",
    params,
  });
  return res.data;
};

export const delReplyAsync = async (replyId: string) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlog/Reply/DeleteReply",
    params: { replyId },
  });
  return res.data;
};

interface IGetPostRepliesParams {
  index: number;
  size: number;
  postId: string;
}
