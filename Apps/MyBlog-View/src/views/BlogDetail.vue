<template>
<div>
    <div class="blog-detail">
      <div class="blog-detail-container">
        <div class="container-content">
          <div class="blog-title">{{ post?.postTitle }}</div>
          <div class="blog-info">
            <div class="time">creation time：{{ transDate(post?.createDate as string) }}</div>
            <div class="nick-name">
              author：<router-link :to="'/'" class="a-link-detail">{{post?.createUserName}}</router-link>
            </div>
            <div class="category">
              categorized column：
              <router-link :to="{path:'/category/'+String(post?.postTypeId)}" class="category-link">
                {{ post?.postTypeName }}
                </router-link>
            </div>
          </div>
          <div class="content" v-html="renderedContent"></div>
        </div>



    <!-- 评论部分 -->
    <div class="container-comment" v-if="post?.discuss==='T'">
      <div class="comment-title">COMMENT {{ comments.postReplyCount }}</div>

            <!-- 对当前文章的评论输入框 -->
            <div class="comment-input">
        <textarea
          v-model="commentText"
          placeholder="Write your comment..."
        ></textarea>
        <button :disabled="!commentText" @click="submitComment">
          Post a comment
        </button>
      </div>

      <!-- 评论列表 -->
      <ul v-infinite-scroll="loadReply" class="comment-list" :infinite-scroll-disabled="disabled" style="overflow: auto">
        <li
          v-for="(comment, index) in comments.replies"
          :key="comment.id"
          class="comment"
        >
        <div class="comment-item">
          <div class="comment-header">
            <Avatar :size="50" :cover="getUpdatedCoverUrl(comment.headUrl)"></Avatar>
            <div class="comment-info">
              <div class="user-name">{{ comment.userName }}</div>
              <div class="time">{{ transDate(comment.creationTime) }}</div>
            </div>
          </div>
          <div class="reply-button">
            <button @click="showReplyInput(comment)">Reply</button>
          </div>
        </div>
          <div class="comment-content">{{ comment.replyContent }}</div>

          <!-- 回复部分 -->
           <div v-if="comment.replyDtos && comment.replyDtos.length > 0" class="reply-list">
            <div
              v-for="(reply, replyIndex) in comment.replyDtos"
              :key="reply.id"
              class="reply-item"
            >
              <div class="reply-header">
                <!-- <img :src="reply.user.avatar" alt="avatar" class="avatar" /> -->
                 <Avatar :size="40" :cover="getUpdatedCoverUrl(reply.headUrl)"></Avatar>
                 <div class="replyInfo">
                  <div class="reply-user">{{ reply.userName }}</div>
                  <div class="reply-time">{{ transDate(reply.creationTime) }}</div>
                 </div>
              </div>
              <div class="reply-content">{{ reply.replyContent }}</div>
            </div>

        <div class="page-panel">
          <Pagination
      v-if="(comment.commentReplyCount ?? 0) > comments.commentReplySize"
      :total="comment.commentReplyCount"
      :page-size="comments.commentReplySize"
      :pageNo="comment.commentReplyIndex"
        @pageChange="handlePageChange(comment.id as string, $event)">
        </Pagination>
      </div>
          </div>
        </li>
      </ul>

      <!-- 回复输入框 -->
      <div v-if="isReplying" class="reply-input">
        <textarea
          v-model="replyText"
          :placeholder="`response: ${replyToUser}`"
        ></textarea>
        <button :disabled="!replyText" @click="submitReply">
          Submit a response
        </button>

        <button  @click="hideWindow">
          Hide
        </button>
      </div>
    </div>

        
      </div>
      <div class="right" id="right" :style="{ left: leftNum + 'px' }">
        <div
          class="container"
          id="right-container"
          :style="{ top: marginTop + 'px' }"
        >

        <div class="category-path">
          <div class="part-title">DIRECTORY</div>
          <div class="toc-list">
            <template v-if="tocArray.length == 0">
              <div class="no-data">Directory not parsed</div>
            </template>
            <template v-else>
              <!-- tocArray 是存储解析后的目录数据的 数组，每个 item 代表一个标题。
                   item.id 是每个标题的唯一 ID，它用于 定位到页面上的对应元素
                   @click.prevent：阻止默认行为（比如 <a> 标签的跳转） -->
              <div v-for="item in tocArray" :key="item.id">
                <a
                  href="javascript:void(0);"
                  class="toc-item"
                  :style="{ 'padding-left': item.level * 15 + 'px' }"
                  @click.prevent="scrollTo(item.id)"
                  >{{ item.title }}
                </a>
              </div>
            </template>
          </div>
        </div>

        <div class="category-path">
          <div class="part-title">
            <span style="color: #454c62;">CATEGORIZED</span>
            <router-link to="../category" class="a-link" style="color: #359ae2;">More&gt;&gt;</router-link>
          </div>
          <div class="category-list">
                <div v-for="item in categoryList" key:item.order>
                    <CategoryItem
                             :postTypeId="item.id" 
                             :cover="getUpdatedCoverUrl(item.cover)"
                             :name="item.postTypeName"
                             :count="item.count"></CategoryItem>
                </div>
            </div>
        </div>
        </div> 
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {getPostAsync} from '@/httpRequest/PostRequest';
import { getPostTypesAsync } from "@/httpRequest/PostTypeRequest";
import { useUserStore } from '@/stores/userStore';
import {getPostRepliesAsync,getCommentRepliesAsync,AddReplyAsync,IAddReplyParans} from "@/httpRequest/ReplyRequest";
import { superAxios } from "@/common/superAxios";
import {transDate,getUpdatedCoverUrl} from '@/utility/common'
import {useRoute} from "vue-router";
import { marked } from "marked";
import { ElMessage } from "element-plus";
import {ref,onMounted,computed,nextTick,watch,reactive} from "vue";
import hljs from "highlight.js";
import "highlight.js/styles/atom-one-light.css";
import Avatar from '@/components/Avatar.vue';

//获取右侧的位置
const leftNum = ref((window.innerWidth - 1350) / 2 + 1350 - 350);
//获取右侧位置
const marginTop = ref(0);

interface Post {
    id:string;
    postCover: string;
    postTitle: string;
    summary: string;
    discuss:string;
    postContent:string;
    postTypeId:string;
    postTypeName:string;
    createUserName: string;
    createDate: string;
    }

interface Reply {
  id?: string;
  replyContent: string;
  headUrl: string;
  userName: string;
  postId: string;
  creationTime: string;
  replyUserName?: string;
  quoteReplyId?: string;

  commentReplyIndex:number;
  commentReplyCount: number;
  replyDtos?: Reply[];
}


const route = useRoute();
const userStore = useUserStore();
const currentUser = userStore.userInfo;

const blogId = ref(route.params.blogId);
const post = ref<Post>();
const categoryList = ref();
const local = ref<string>("");

//控制无线滚动
const loadingComments = ref(false);
const noMore = computed(() => comments.replies.length >= comments.postReplyCount);
const disabled = computed(() => loadingComments.value || noMore.value);



// 评论和回复的数据结构
const commentText = ref('');
const replyText = ref('');
const replyToUser = ref('');
const isReplying = ref(false);
const quoteReplyId = ref('');
//防止多次提交评论
const isSubmitting = ref(false);

const comments = reactive<{
  replies:Reply[];
  postReplyCount:number;
  postReplyIndex:number;
  postReplySize:number;

  commentReplySize:number;
}>({
  replies: [],

  postReplyCount: 0,
  postReplyIndex:1,
  postReplySize:5,

  commentReplySize:5
});

// 提交对本帖的评论
const submitComment = async () => {

  if (!currentUser || !currentUser.userName) {
    ElMessage("You are not logged in");
    return;
  }

  if (!commentText.value.trim() || !blogId.value) return;

  if (isSubmitting.value) return;
    isSubmitting.value = true;

  const params: IAddReplyParans = {
    replyContent: commentText.value,
    headUrl: currentUser.headUrl,
    userName: currentUser.userName, 
    postId: String(blogId.value),
  };

  try {
    await AddReplyAsync(params);
    commentText.value = "";
    await loadComments(); 
    const message = "Comment posted successfully";
    ElMessage({ message, type: "success" });
  } catch (error) {
    console.error("Comment failed", error);
  }

};

// 提交回复
const submitReply = async () => {
  // 检查用户是否登录（currentUser 或 userInfo 中应该包含用户信息）
  if (!currentUser || !currentUser.userName) {
    ElMessage("You are not logged in");
    return;
  }

  if (!replyText.value.trim() || !blogId.value) return;

  const params: IAddReplyParans = {
    replyContent: replyText.value,
    headUrl: currentUser.headUrl,
    userName: currentUser.userName,
    postId: String(blogId.value),
    replyUserName: replyToUser.value,
    quoteReplyId:quoteReplyId.value
  };
  try {
    isReplying.value = false;
    await AddReplyAsync(params);
    replyText.value = "";
    const message = "Reply posted successfully";
    ElMessage({ message, type: "success" });
    await loadCommentReplies(quoteReplyId.value);
  } catch (error) {
    console.error("Failed to submit reply", error);
  }
};


//更新当前页码
const handlePageChange = async (commentId: string, newPageNo: number) => {
  console.log(commentId);
  const comment = comments.replies.find(c => c.id === commentId);
  if (!comment) return;

  // 更新该评论的分页信息
  comment.commentReplyIndex = newPageNo;
  try {
    await loadCommentReplies(commentId);
  } catch (error) {
    console.error("分页加载评论失败", error);
  }
};

// 点击回复按钮时显示回复输入框
const showReplyInput = (comment: any) => {
  quoteReplyId.value = comment.id;
  isReplying.value =!isReplying.value;
  replyToUser.value = comment.userName;
  quoteReplyId.value = comment.id;
};

//隐藏输入框
const hideWindow = () => {
  isReplying.value = false;
};

//加载评论
const loadComments = async () => {
  if (!post.value?.id) return;

  try {
    const response = await getPostRepliesAsync({
      postReplyIndex: comments.postReplyIndex,
      postReplySize: comments.postReplySize,
      postId: post.value.id,
      // commentReplyIndex: comments.commentReplyIndex,
      // commentReplySize: comments.commentReplySize
    });

    // 将新的评论追加到现有的评论数组中
    if (response.replyDtos) {
      comments.replies.push(...response.replyDtos);  // 使用 push 合并评论数据
      comments.replies.sort((a, b) => 
        new Date(b.creationTime).getTime() - new Date(a.creationTime).getTime()
      );
    }
    comments.postReplyCount = response.postReplyCount || 0;
  } catch (error) {
    console.error("加载评论失败", error);
  }
};

// 加载评论的回复
const loadCommentReplies = async (commentId: string) => {
  try {
    const comment = comments.replies.find(c => c.id === commentId);

    if (!comment) {
      console.warn(`未找到评论：${commentId}`);
      return; // 如果评论不存在，则提前返回
    }

      const response = await getCommentRepliesAsync({
      commentReplyIndex: comment.commentReplyIndex,
      commentReplySize: comments.commentReplySize,
      replyId: commentId,
    });

    if (comment && response.replyDtos) {
      comment.replyDtos = response.replyDtos;
      comment.commentReplyCount = response.postReplyCount ?? 0;
    }
    else{
      console.log(response.replyDtos);
    }
    
  } catch (error) {
    console.error("加载评论回复失败", error);
  }
};


// 处理滚动加载评论
const loadReply = async () => {
  loadingComments.value = true
  setTimeout(async () => {
    // 增加页码
    comments.postReplyIndex += 1
    loadingComments.value = false
    await loadComments();
  }, 2000)
};


const getBlogDetail = async () => {
    if (!blogId.value) return;
    const postId = String(blogId.value); 
    let result = await getPostAsync(postId);
    if (!result) {
    return;
    }
    post.value = result;
    highlightCode();
    makeToc();
};

const renderedContent = computed(() => {
  return post.value?.postContent ? marked(post.value.postContent) : "";
});

//获取全部分类
const loadCategoryList = async () => {
  let result = await getPostTypesAsync({
    pageIndex: 1,
    pageSize: 5,
  });
  categoryList.value = result;
};

const getImgUrl = async () =>{
    const apiRoot = await superAxios.getValue("ImageUrl");
  if (apiRoot) {
    local.value = apiRoot;
  }
}

//从文章内容中解析出标题结构，并生成目录
const tocArray = ref<{ id: string; title: string; level: number }[]>([]);

const makeToc = () => {
  //nextTick 作用是 等待 DOM 更新完成后再执行代码
  nextTick(() => {
    //获取 包裹文章内容的 div 容器文章的正文（包括 h1-h6 这些标题）都在这个 div 里面
    const contentDom = document.querySelector("#content");
    if (!contentDom) return;

    tocArray.value = []; // 清空旧的目录结构
    //存储可识别的标题标签（h1 - h6），用于筛选文章中的标题
    const tocTags = ["H1", "H2", "H3", "H4", "H5", "H6"];
    
    // 获取所有标题标签
    const headers = contentDom.querySelectorAll("h1, h2, h3, h4, h5, h6");
    //遍历标题并添加到 tocArray
    headers.forEach((item, index) => {
      //获取标签名并转大写，例如 h2 → "H2"
      const tagName = item.tagName.toUpperCase();
      if (tocTags.includes(tagName)) {
        //给标题生成唯一 id 比如第一个 h1 可能是 toc1，第二个 h2 是 toc2
        const id = "toc" + (index + 1);
        //如果原来的 h2 没有 id，就给它 动态添加 id id就类似class，但是id是每个元素只能有一个不能重复
        item.setAttribute("id", id);
        tocArray.value.push({
          id: id,
          //textContent?.trim()获取标题的文本内容，并去掉前后空格
          title: item.textContent?.trim() || "无标题",
          level: Number(tagName.substring(1)),
        });
      }
    });
  });
};

// 监听 postContent 变化，确保解析目录
watch(renderedContent, (newValue) => {
  if (newValue) {
    makeToc();
  }
});

const highlightCode = () => {
  nextTick(() => {
    const blocks = document.querySelectorAll("pre code");
    blocks.forEach((block) => {
      hljs.highlightElement(block as HTMLElement);
    });
  });
};


const scrollTo = (id: string) => {
  //获取对应的标题标签
  const target = document.getElementById(id);
  if (target) {
    //让页面 平滑滚动 到指定位置
    target.scrollIntoView({ behavior: "smooth", block: "start" });
  }
};

onMounted(async()=>{
    await getImgUrl();
    await getBlogDetail();
    await loadCategoryList();
    
    //加载评论
    await loadComments();
  })
</script>

<style lang="scss" scoped>
.blog-detail {
  display: flex;
  position: relative;
  margin-top: 30px;
  .right {
    margin-top: 30px;
    margin-left: 10px;
    top: 90px;
    padding-top: 0px;
    position: fixed;
    .container {
      width: 300px;
      padding: 10px 5px;
      border-radius: 20px;
      background: #f5f6f7;
      .toc-list {
        max-height: 400px;
        overflow: auto;
        .no-data {
          text-align: center;
          color: rgb(148, 146, 146);
          line-height: 40px;
          font-size: 13px;
        }
        .toc-item {
          line-height: 30px;
          overflow: hidden;
          white-space: nowrap;
          text-overflow: ellipsis;
          cursor: pointer;
          color: #555666;
        }
        .toc-item:hover {
          color: #a6a3a3;
        }
      }
    }
  }
  img {
    max-width: 100%;
  }
}
</style>