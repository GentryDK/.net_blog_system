import { superAxios } from "@/common/superAxios";

//获取帖子评论
export const getPostRepliesAsync = async (params:IGetPostRepliesParams)=>{
    var res = await (await superAxios.useWebAPI()).get({
        url:"MyBlogView/Reply/GetPostReply",
        params
    })
    return res.data;
}

//获取评论回复
export const getCommentRepliesAsync = async (params:IGetCommentRepliesParams)=>{
    var res = await (await superAxios.useWebAPI()).get({
        url:"MyBlogView/Reply/GetCommentReply",
        params
    })
    return res.data;
}


//添加回复
export const AddReplyAsync = async (params: IAddReplyParans) => {
    const formData = new FormData();
    if (params.replyUserName){
        formData.append("ReplyUserName", params.replyUserName);
    }
    if (params.quoteReplyId){
        formData.append("QuoteReplyId", params.quoteReplyId);
    }
    formData.append("ReplyContent", params.replyContent);
    formData.append("HeadUrl", params.headUrl);
    formData.append("UserName", params.userName);
    formData.append("PostId", params.postId);

    const res = await (
      await superAxios.useWebAPI()
    ).post({
      url: "MyBlogView/Reply/AddPostReply",
      params: formData,
    });
    return res.data;
  };



interface IGetPostRepliesParams {
    postReplyIndex: number;
    postReplySize: number;
    postId: string;
    commentReplyIndex?: number;
    commentReplySize?: number;
  }

  interface IGetCommentRepliesParams {
    commentReplyIndex: number;
    commentReplySize: number;
    replyId: string;

  }

export interface IAddReplyParans {
    replyContent: string;
    headUrl: string;
    userName: string;
    postId: string;
    replyUserName?: string;
    quoteReplyId?: string;
  }



  