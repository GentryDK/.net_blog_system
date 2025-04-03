import { superAxios } from "@/common/superAxios";

export const checkLogin = async (params:IloginParams)=>{
    var res = await (await superAxios.useWebAPI()).get({
        url:"user/User",
        params
    })
    return res.data;
}

interface IloginParams
{
    email:string;
    password:string;
}

