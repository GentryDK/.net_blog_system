<template>
  <el-upload
    class="upload-demo"
    drag
    :http-request="uploadFile"
    multiple
    accept=".txt"
  >
    <el-icon class="el-icon--upload"><upload-filled /></el-icon>
    <div class="el-upload__text">
      Drop file here or <em>click to upload</em>
    </div>
    <template #tip>
      <div class="el-upload__tip">只能上传 TXT 文件，大小不超过 25MB</div>
    </template>
  </el-upload>
</template>

<script setup lang="ts">
import { UploadFilled } from "@element-plus/icons-vue";
import { ElMessage } from "element-plus";
import type { UploadRequestOptions } from "element-plus";
import { uploadSensitiveWords } from "@/httpRequest/SensitiveWords";

//UploadRequestOptions 是 Element Plus 里 el-upload 组件的 http-request 方法所接受的参数类型
const uploadFile = async (option: UploadRequestOptions) => {
  const file = option.file;

  // 1. 限制文件类型为 txt
  if (file.type !== "text/plain") {
    ElMessage.error("只能上传 TXT 文件！");
    return;
  }

  // 2. 限制文件大小不要超过 25MB
  if (file.size > 25 * 1024 * 1024) {
    ElMessage.error("文件大小不能超过 25MB！");
    return;
  }

  try {
    const response = await uploadSensitiveWords(file);
    ElMessage.success("上传成功");
  } catch (error) {
    ElMessage.error("上传失败，请检查文件格式");
    console.error(error);
  }
};
</script>

<style lang="scss" scoped></style>
