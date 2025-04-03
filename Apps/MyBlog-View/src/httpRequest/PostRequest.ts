import { superAxios } from "@/common/superAxios";

export const getPostsAsync = async (params:IGetPostsParams)=>{
    var res = await (await superAxios.useWebAPI()).get({
        url:"MyBlogView/Post/GetPosts",
        params
    })
    return res.data;
}

export const getPostAsync = async (postId: string) => {
    var res = await (await superAxios.useWebAPI()).get({
        url: "MyBlogView/Post/GetPost",
        params: { postId },
    });
    return res.data;
};


interface IGetPostsParams {
    pageIndex: number;
    pageSize: number;
    postTitle?: string;
    postTypeId?: string;
  }