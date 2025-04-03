<template>
  <div>
    <Window
      :show="windowConfig.show"
      :style="{ width: '100%' }"
      @close="closeWindow"
      ><div class="my-title">
        <Table
          :dataSource="tableData"
          :columns="columns"
          :showPagination="true"
          :fetch="loadReplisList"
          :options="tableOptions"
        >
          <!-- 封面 -->
          <template #cover="{ row }">
            <div class="cover">
              <Cover
                :cover="`${row.headUrl}?t=${new Date().getTime()}`"
              ></Cover>
            </div>
          </template>

          <!-- 文章信息 -->
          <template #replyInfo="{ index, row }">
            <div>名称:{{ row.userName }}</div>
          </template>

          <!-- 状态 -->
          <template #replyContent="{ row }">
            <div>{{ row.replyContent }}</div>
          </template>

          <!-- 时间 -->
          <template #time="{ index, row }">
            <div>创建时间:{{ formatDate(row.creationTime) }}</div>
          </template>

          <template #op="{ row }">
            <div class="op">
              <a href="javascript:void(0)" @click="del(row)">删除</a>
            </div>
          </template>
        </Table>
      </div>
    </Window>
  </div>
</template>

<script setup lang="ts">
import Window from "@/components/Window.vue";
import { defineExpose, onMounted, reactive, ref } from "vue";
import { formatDate } from "@/utils/DateTime";
import Confirm from "@/utils/Confirm";
import { ElMessage } from "element-plus";
import { getPostRepliesAsync, delReplyAsync } from "@/httpRequest/ReplyRequest";

const tableOptions = {
  extHeight: 50,
};

const windowConfig = reactive({
  title: "添加新帖信息",
  show: false,
});

const postId = ref("");

const tableData = reactive({
  list: [],
  totalCount: 0,
  pageSize: 15,
  pageIndex: 1,
});

const showReplyManager = (id: string) => {
  console.log("postId:" + id);
  postId.value = id;
  windowConfig.show = true;
};

//关闭当前页面
const closeWindow = async () => {
  windowConfig.show = false;
  postId.value = "";
  tableData.list = [];
  tableData.totalCount = 0;
};

//删除
const del = async (data: any) => {
  Confirm(`你确认要删除${data.userName}的评论?`, async () => {
    console.log("id:" + data.id);
    await delReplyAsync(data.id as string);
    // // 删除完毕后重新请求数据

    ElMessage({ message: "删除成功", type: "success" });
    await loadReplisList();
  });
};

const loadReplisList = async () => {
  if (postId.value == null || postId.value == "") {
    return;
  }
  let res = await getPostRepliesAsync({
    index: tableData.pageIndex,
    size: tableData.pageSize,
    postId: postId.value,
  });
  tableData.list = res.replyDtos;
  tableData.totalCount = res.postReplyCount;
};

const columns = [
  {
    label: "头像",
    prop: "cover",
    width: 80,
    scopedSlots: "cover",
  },
  {
    label: "用户信息",
    prop: "replyInfo",
    scopedSlots: "replyInfo",
  },
  {
    label: "回复内容",
    prop: "replyContent",
    scopedSlots: "replyContent",
  },
  {
    label: "时间",
    prop: "time",
    width: 200,
    scopedSlots: "time",
  },
  {
    label: "操作",
    prop: "op",
    width: 200,
    scopedSlots: "op",
  },
];

// 暴露给父组件
defineExpose({ showReplyManager });

onMounted(async () => {
  await loadReplisList();
});
</script>

<style lang="scss" scoped></style>
