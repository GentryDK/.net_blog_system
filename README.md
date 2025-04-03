I am DK, a developer from China.

This is a personal blog forum project I developed myself, based on .NET WebAPI (I use abpVnext) . The frontend uses Vue3, and it has already been deployed.
When you carefully examine the directory structure, you can find that it is a microservices project.
this project includes a complete backend management system.a permission system,a login system a sensitive-word detection system,and more. it is a comprehensive forum project that contains all the basic funtions of a forum.

Address: http://www.dkedu.club:8081/

Now.I will briefly outline the technologies included in this project, the requirements, and the precautions to take during its use.

1.This project utilizes Redis, so you must have Redis installed to run it. It is used for caching multiple features.
2.I use Ocelot as the Gateway.if you deploy this project you must edit appsetting.json to adapt to each service.
3.I use abpVnext.and each service interacts via RPC. Therefore,you must correctly configute the 'RemoteServices' section for every service in the appsettings.json file, For detailed instructions.Please refer to the ABP vNext offical web.
4.In the frontend.l use the Vue3 and Element-Plus frameworks.lf your frontend encounters an error,please check the configuration in your main file and ensure it is correct.
5.In the login service, I have implemented a function for email verification codes. It uses QQ email. If you want to use this feature, the 'Mail' section in the appsettings file of the login service is where you configure this option. The 'UserName' specifies your sender email address, and the 'Password' is your key."

"Mail": {
  "Host": "xxx.qq.com",
  "Port": 587,
  "UserName": "xxx",
  "Password": "xxx"
}

Last! if you don't know how to deploy the project.you can refer to the Docker section of my blog!

I hope you like my project.thank~

----------------------------------------------------------------------------------------------------------------------------------------------------------------

这是我自己开发的个人博客论坛项目，基于.NET WebAPI（我使用了abpVnext）。前端使用Vue3，目前已经完成部署。 当你仔细查看目录结构时，你会发现它是一个微服务项目。 该项目包括一个完整的后台管理系统、权限系统、登录系统、敏感词检测系统等。它是一个完整的论坛项目，包含了论坛的基本功能。
地址：http://www.dkedu.club:8081/

接下来，我将简要概述此项目所使用的技术、所需的条件以及使用过程中的注意事项：

1.该项目使用了Redis，因此您必须安装Redis才能运行。Redis被用于缓存多个功能。
2.我使用了Ocelot作为网关。如果您要部署此项目，您必须编辑appsetting.json文件以适配每个服务。
3.我使用了abpVnext，每个服务之间通过RPC进行交互。因此，您必须正确配置每个服务的appsetting.json文件中的“RemoteServices”部分。详细操作请参考abpVnext官方网站。
4/在前端，我使用了Vue3和Element-Plus框架。如果前端报错，请检查主文件中的配置，并确保其正确。
5.在登录服务中，我实现了邮件验证码功能。该功能使用了QQ邮箱。如果您想使用该功能，需要在登录服务的appsetting文件中的“Mail”部分进行配置。“UserName”指定您的发送邮箱地址，“Password”是您的密钥。
"Mail": {
  "Host": "xxx.qq.com",
  "Port": 587,
  "UserName": "xxx",
  "Password": "xxx"
}

最后，如果您不知道如何部署该项目，可以查看我的博客中关于Docker部分的内容！
希望您喜欢我的项目，谢谢！
