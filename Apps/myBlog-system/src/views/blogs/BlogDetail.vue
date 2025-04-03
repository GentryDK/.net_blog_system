<template>
  <div>
    <Window
      :buttons="windowConfig.buttons"
      :show="windowConfig.show"
      :style="{ width: '100%' }"
      @close="closeWindow"
      ><div class="my-title">
        {{ blogFormData.postTitle }}
      </div>
      <hr />
      <div v-html="renderedContent" class="blog-detail"></div>
    </Window>
  </div>
</template>

<script setup lang="ts">
import Window from "@/components/Window.vue";
import hljs from "highlight.js";
import "highlight.js/styles/atom-one-light.css"; //样式
import { marked } from "marked";
import { reactive, nextTick, computed } from "vue";

//上传的弹窗界面，用于博客设置的参数
const blogFormData = reactive<any>({
  postTitle: "",
  postContent: "",
});

//当前页面显示的信息配置
const windowConfig = reactive({
  title: "添加新帖信息",
  show: false,
  buttons: [
    {
      type: "danger",
      text: "确定",
      click: async (e: any) => {
        closeWindow();
      },
    },
  ],
});

const showDetail = (postInfo: any) => {
  blogFormData.postTitle = postInfo.postTitle;
  blogFormData.postContent = postInfo.postContent;
  windowConfig.show = true;

  nextTick(() => {
    //高亮显示
    let blocks = document.querySelectorAll("pre code");
    blocks.forEach((block) => {
      hljs.highlightBlock(block as HTMLElement);
    });
  });
};

//将上面的init方法暴露出去
defineExpose({ showDetail });

//关闭当前页面
const closeWindow = async () => {
  windowConfig.show = false;
};

const renderedContent = computed(() => {
  return marked(blogFormData.postContent);
});
</script>

<style lang="scss" scoped>
.my-title {
  font-size: 22px;
}

.blog-detail {
  text-align: left;
  padding: 0 1em;
  blockquote {
    padding: 0 1em;
    color: #6a737d;
    border-left: 0.25em solid #dfe2e5;
  }

  img {
    max-width: 80%;
  }
}

hr {
  border: 0;
  height: 1px;
  background: #ddd;
  margin: 20px 0;
}
</style>
