import { superAxios } from "@/common/superAxios";

export const UploadFile = async (params:any)=>{
    var res = await (await superAxios.useWebAPI()).post({
        url:"user/User/upload",
        params
    });
    return res.data;
}