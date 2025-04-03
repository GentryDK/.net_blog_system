//defineStore：Pinia 提供的 API，用于定义状态管理的 Store
import { defineStore } from 'pinia';
import { reactive } from 'vue';

//'user' 是这个 Store 的 唯一 ID，用于在应用中区分不同的 Store
export const useUserStore = defineStore('user', () => {
    const userInfo = reactive({
        userName: "",
        headUrl: "",
    });

// 定义 setUserInfo 方法：
// 用于修改 userInfo 中的 userName 和 headUrl。
// 由于 userInfo 是 reactive 的，所以修改后会 自动触发响应式更新，不需要 this.setState 之类的操作（类似 Vuex 里的 mutations）
    const setUserInfo = (data: { userName: string; headUrl: string;}) => {
        userInfo.userName = data.userName;
        userInfo.headUrl = data.headUrl;
    };

    return { userInfo, setUserInfo };
});
