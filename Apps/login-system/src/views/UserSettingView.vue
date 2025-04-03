<template>
<div>
    <el-page-header style="margin-top: 15px;">
    <template #content>
      <span class="text-large font-600 mr-3" style="font-weight: bold;"> 用户设置 </span>
    </template>
    <div class="mt-4 text-sm font-bold">
        <div class="container">
    <div class="avatarUpload" @click="triggerFileInput">
      <input type="file" class="hidden" ref="fileInput" @change="onFileChanged" />
      <el-icon><Top /></el-icon>
    </div>

    <div class="upload-text ">
      <p style="  font-weight: bold;">点击这里上传头像</p>
    </div>
  </div>
    </div>
  </el-page-header>
</div>
  </template>
  
  <script lang="ts" setup>
import { ref } from 'vue';
import { UploadFile } from "@/httpRequest/avatarUploadRequest"

const fileInput = ref<any>(null);

const triggerFileInput = () => {
  fileInput.value.click();
};

const onFileChanged = async (event: Event) => {
  const target = event.target as HTMLInputElement;
  if (target.files && target.files.length > 0) {
    const file = target.files[0];

    // 检查文件大小是否超过 2MB
    const maxSizeInMB = 2;
    if (file.size > maxSizeInMB * 1024 * 1024) {
      alert("文件大小不能超过 2MB");
      return;
    }

    // 检查文件格式是否为 JPG
    const allowedTypes = ["image/jpeg"];
    if (!allowedTypes.includes(file.type)) {
      alert("文件格式必须为 JPG");
      return;
    }

    const forms = new FormData();
    forms.append("file", file);

    const res = await UploadFile(forms);
    // 处理上传结果
  }
};
  </script>

<style lang="scss"  scoped>
.container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 80vh;
}

.avatarUpload {
  border: 2px dashed #ccc;
  border-radius: 8px;
  padding: 50px;
  cursor: pointer;
  text-align: center;
}

.avatarUpload:hover {
  background-color: #f0f0f0;
}

.hidden {
  display: none;
}

.upload-text {
  margin-top: 20px; /* 调整这个值来设置间距 */
  text-align: center;
}
</style>