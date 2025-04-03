import { superAxios } from "@/common/superAxios";

export const updateUserAsync = async (params: IUpdateUserInfoParans) => {
  const formData = new FormData();
  formData.append("UserName", params.userName);
  formData.append("Email", params.email);

  if (params.introduction) {
    formData.append("Introduction", params.introduction);
  }

  if (params.headImgFile) {
    formData.append("HeadImgFile", params.headImgFile);
  }

  const res = await (
    await superAxios.useWebAPI()
  ).postUpload({
    url: "MyBlog/User/UpdateUser",
    params: formData,
  });
  return res.data;
};

export const getUserInfoAsync = async () => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlog/User/GetUserInfo",
  });
  return res.data;
};

export const updateUserPasswordAsync = async (password: string) => {
  var res = await (
    await superAxios.useWebAPI()
  ).post({
    url: `MyBlog/User/UpdateUserPassword?password=${password}`,
  });
  return res.data;
};

export const getUsersAsync = async (params: IGetUsersParans) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlog/User/Users",
    params,
  });
  return res.data;
};

export const AdminUpdateUserAsync = async (updateUserDto: any, file: any) => {
  const formData = new FormData();
  formData.append("UserId", updateUserDto.UserId);
  formData.append("UserName", updateUserDto.UserName || "");
  formData.append("Email", updateUserDto.Email || "");
  formData.append("Password", updateUserDto.Password || "");
  formData.append("RoleId", updateUserDto.RoleId || null);

  if (file) {
    formData.append("file", file);
  }

  var res = await (
    await superAxios.useWebAPI()
  ).postUpload({
    url: "MyBlog/User/AdminUpdateUser",
    params: formData,
  });
  return res;
};

export const BanUserAsync = async (params: any) => {
  var res = await (
    await superAxios.useWebAPI()
  ).get({
    url: "MyBlog/User/BaneUser",
    params,
  });
  return res;
};

interface IGetUsersParans {
  pageIndex: number;
  pageSize: number;
  userName?: string | null;
}

interface IUpdateUserInfoParans {
  userName: string;
  email: string;
  introduction: string;
  headImgFile: File | null;
}
