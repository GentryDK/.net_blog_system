import { superAxios } from "@/common/superAxios";

export const registUser = async (params:IregistParams)=>{
    var res = await (await superAxios.useWebAPI()).post({
        url:"user/User",
        params
    })
    return res.data;
}

export const sendEmail = async (params:IemailParams)=>{
    var res = await (await superAxios.useWebAPI()).post({
        url:"user/User/SendEmailCode",
        params
    })
    return res.data;
}

interface IemailParams{
    userName:string,
    email:string
}

interface IregistParams
{
    userName:string,
    email:string,
    password:string,
    emailCode:string
}