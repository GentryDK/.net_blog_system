import { superAxios } from "@/common/superAxios";

//添加敏感词库
export const uploadSensitiveWords = async (params: File) => {
  const formData = new FormData();
  formData.append("sensitiveFile", params);
  const res = await (
    await superAxios.useWebAPI()
  ).postUpload({
    url: "MyBlog/Sensitivewords/UploadSentitiveWords",
    params: formData,
  });
  return res.data;
};
